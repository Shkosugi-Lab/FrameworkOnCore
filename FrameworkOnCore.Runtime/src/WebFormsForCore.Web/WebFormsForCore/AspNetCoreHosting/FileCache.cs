#if NETCOREAPP

using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;


namespace System.Web.Hosting
{
	/// <summary>
	/// What the application's files are, kept as IIS's file cache keeps it (its FileCacheModule, its URI cache): a static
	/// file's request asked the file system each time whether its path was a folder (three times: the courtesy redirect,
	/// the default document, the static file handler), whether the file was there, its web.config files' times, and opened
	/// the file twice (its length, its bytes): a file system call is some 20 µs on Windows and they did not go faster side
	/// by side; a static file was answered at a 20th of IIS's rate (IIS answers it from memory, or from http.sys's cache).
	/// Under the application's folder (the one AspNetCoreHost is given), each path's state is kept (that it is a file or a
	/// folder or not there, its attributes, length and time; a folder's files' names; a small file's bytes, as IIS's
	/// 256 KB) until a file or folder there changes (a FileSystemWatcher on the folder and its folders: anything changed,
	/// all of it forgotten). Paths outside it, and everything if the folder cannot be watched, are asked each time.
	/// </summary>
	internal static class FileCache
	{
		internal sealed class Entry
		{
			public long Generation;       // the changes' count when it was found: an entry of an earlier count is not used
			public bool IsFile;
			public bool IsDirectory;
			public FileAttributes Attributes;
			public long Length;
			public DateTime LastWriteTimeUtc;
			public byte[] Content;        // a file's bytes, read once (small files only)
			public bool ContentRead;
			public string[] FileNames;    // a folder's files' names, read once
		}

		const int MaxEntries = 10000;
		const long MaxContentLength = 256 * 1024;      // IIS's file cache: maxCachedFileSize
		const long MaxCachedBytes = 64L * 1024 * 1024;

		static readonly StringComparison PathComparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
		static readonly ConcurrentDictionary<string, Entry> entries = new ConcurrentDictionary<string, Entry>(StringComparer.Ordinal);
		static readonly ConcurrentDictionary<string, (long Generation, object Value)> memos = new ConcurrentDictionary<string, (long, object)>(StringComparer.Ordinal);
		static FileSystemWatcher watcher;
		static string root;
		static long generation;      // counts the changes: what was found before one is not kept after it
		static long cachedBytes;
		// Folders whose writes are not the application's files' changes (the W3C log's, should it be in the folder).
		static string[] ignored = Array.Empty<string>();

		/// <summary>Writes under the folder are not changes of the application's files (what is kept stays).</summary>
		internal static void IgnoreChangesUnder(string folder)
		{
			var full = Path.GetFullPath(folder).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
			lock (entries) ignored = ignored.Append(full).ToArray();
		}

		static bool Ignored(string path)
		{
			foreach (var folder in ignored)
				if (path != null && path.StartsWith(folder, PathComparison)) return true;
			return false;
		}

		/// <summary>Keeps what is found under the folder from now on (the application's folder).</summary>
		internal static void Watch(string physicalPath)
		{
			var folder = Path.GetFullPath(physicalPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
			lock (entries)
			{
				if (root != null && string.Equals(root, folder, PathComparison)) return;
				watcher?.Dispose();
				watcher = null;
				root = null;
				Forget();
				try
				{
					var w = new FileSystemWatcher(folder)
					{
						IncludeSubdirectories = true,
						NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite |
							NotifyFilters.Size | NotifyFilters.Attributes | NotifyFilters.CreationTime,
						InternalBufferSize = 64 * 1024,
					};
					FileSystemEventHandler changed = (sender, e) => { if (!Ignored(e.FullPath)) Forget(); };
					// An ignored folder's own change (its time: a file in it created, renamed, removed) is its files' too.
					w.Changed += (sender, e) => { if (!Ignored(e.FullPath) && !Ignored(e.FullPath + Path.DirectorySeparatorChar)) Forget(); };
					w.Created += changed;
					w.Deleted += changed;
					w.Renamed += (sender, e) => { if (!Ignored(e.FullPath) || !Ignored(e.OldFullPath)) Forget(); };
					w.Error += (sender, e) => Forget();     // events lost (the buffer overflowed): anything may have changed
					w.EnableRaisingEvents = true;
					watcher = w;
					root = folder;
				}
				catch (Exception e) when (e is IOException || e is ArgumentException || e is UnauthorizedAccessException || e is PlatformNotSupportedException)
				{
					// Not watched (inotify's limit reached, a folder not to be watched): nothing kept.
				}
			}
		}

		static void Forget()
		{
			Interlocked.Increment(ref generation);
			entries.Clear();
			memos.Clear();
			Interlocked.Exchange(ref cachedBytes, 0);
		}

		/// <summary>Whether the application's folder is watched (what is kept follows its changes).</summary>
		internal static bool Watching => root != null;

		/// <summary>The count of the changes seen so far: what was made from the files at one count is stale at another.</summary>
		internal static long Generation => Interlocked.Read(ref generation);

		static bool Kept(string path) =>
			root != null && path != null && path.StartsWith(root, PathComparison) && path.IndexOf("..", StringComparison.Ordinal) < 0;

		/// <summary>The path's state, kept for a path under the watched folder (asked anew otherwise).</summary>
		/// <remarks>
		/// What is kept carries the count of the changes it was found at, and is used at that count only. It was checked,
		/// then stored: a change between the two (Forget clearing what was kept) left what was found before the change kept
		/// after it, until the next change (a file's old state, its old bytes, served after its new one).
		/// </remarks>
		internal static Entry Get(string path)
		{
			if (!Kept(path)) return Probe(path, 0);
			Entry entry;
			var before = Interlocked.Read(ref generation);
			if (entries.TryGetValue(path, out entry) && entry.Generation == before) return entry;
			entry = Probe(path, before);
			if (entries.Count >= MaxEntries) Forget();
			// Kept only if nothing changed while it was asked (what was found may be from before the change).
			if (Interlocked.Read(ref generation) == before) entries[path] = entry;
			return entry;
		}

		static Entry Probe(string path, long found)
		{
			var entry = new Entry { Generation = found };
			try
			{
				var info = new FileInfo(path);
				if (info.Exists)
				{
					entry.IsFile = true;
					entry.Attributes = info.Attributes;
					entry.Length = info.Length;
					entry.LastWriteTimeUtc = info.LastWriteTimeUtc;
				}
				else if (Directory.Exists(path))
				{
					entry.IsDirectory = true;
					entry.Attributes = File.GetAttributes(path);
					entry.LastWriteTimeUtc = Directory.GetLastWriteTimeUtc(path);
				}
			}
			catch (Exception e) when (e is IOException || e is ArgumentException || e is UnauthorizedAccessException || e is NotSupportedException)
			{
			}
			return entry;
		}

		internal static bool DirectoryExists(string path) => Get(path).IsDirectory;

		internal static bool FileExists(string path) => Get(path).IsFile;

		/// <summary>A small file's bytes (read once while it does not change); null for a large one or one not read.</summary>
		internal static byte[] Content(string path, Entry entry)
		{
			if (!entry.IsFile || entry.Length > MaxContentLength || !Kept(path)) return null;
			if (Volatile.Read(ref entry.ContentRead)) return entry.Content;
			try
			{
				// The file as it was found (its length and time): one written meanwhile is read again next time.
				using (var snapshot = Open(path, entry))
					return snapshot.Content != null && snapshot.Length == entry.Length && snapshot.LastWriteTimeUtc == entry.LastWriteTimeUtc ? snapshot.Content : null;
			}
			catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
			{
				return null;
			}
		}

		/// <summary>
		/// The file as it is now: its length, time and bytes from one opening of it (as IIS's static file module has a file:
		/// its handle's size and time, the handle's bytes); a small one's bytes (kept with the entry while nothing changes,
		/// if the entry is the file as it was opened), a large one open (the caller's to dispose). Opened to be deleted and
		/// renamed over meanwhile (a deployment replacing it), as IIS opens it. The entry's length and time, the bytes read
		/// by path and the file opened by name when the response was sent were three files when it was replaced meanwhile:
		/// the new bytes with the old Content-Length, ETag and Last-Modified (or a 500: the file shorter than its length).
		/// </summary>
		internal static FileSnapshot Open(string path, Entry entry)
		{
			var before = Interlocked.Read(ref generation);
			if (entry != null && Volatile.Read(ref entry.ContentRead) && entry.Content != null && entry.Generation == before)
				return new FileSnapshot { Length = entry.Length, LastWriteTimeUtc = entry.LastWriteTimeUtc, Content = entry.Content };
			var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 1, FileOptions.SequentialScan);
			try
			{
				var snapshot = new FileSnapshot { Length = stream.Length, LastWriteTimeUtc = File.GetLastWriteTimeUtc(stream.SafeFileHandle) };
				if (snapshot.Length > MaxContentLength)
				{
					snapshot.Stream = stream;
					stream = null;
					return snapshot;
				}
				var content = new byte[snapshot.Length];
				var read = 0;
				while (read < content.Length)
				{
					var n = RandomAccess.Read(stream.SafeFileHandle, content.AsSpan(read), read);
					if (n <= 0) break;
					read += n;
				}
				// Shorter than its length (being written): its bytes as read, not kept.
				if (read < content.Length)
				{
					Array.Resize(ref content, read);
					snapshot.Length = read;
					snapshot.Content = content;
					return snapshot;
				}
				snapshot.Content = content;
				if (entry != null && Kept(path) && entry.Generation == before && entry.Length == snapshot.Length && entry.LastWriteTimeUtc == snapshot.LastWriteTimeUtc &&
					Interlocked.Read(ref generation) == before && Interlocked.Add(ref cachedBytes, content.Length) <= MaxCachedBytes)
				{
					entry.Content = content;
					Volatile.Write(ref entry.ContentRead, true);
				}
				return snapshot;
			}
			finally
			{
				stream?.Dispose();
			}
		}

		/// <summary>A folder's files' names (read once while nothing changes).</summary>
		internal static string[] FileNames(string path)
		{
			var entry = Get(path);
			var names = entry.FileNames;
			if (names != null) return names;
			var before = Interlocked.Read(ref generation);
			names = Directory.EnumerateFiles(path).Select(Path.GetFileName).ToArray();
			if (Kept(path) && entry.Generation == before && Interlocked.Read(ref generation) == before) entry.FileNames = names;
			return names;
		}

		/// <summary>
		/// A value made from the application's files (its web.config files' sections), kept until a file changes; made each
		/// time if the folder is not watched.
		/// </summary>
		internal static T Memo<T>(string key, Func<T> make) where T : class
		{
			if (root == null) return make();
			var before = Interlocked.Read(ref generation);
			(long Generation, object Value) kept;
			// (one made at an earlier count of the changes is not used: as Get's entries)
			if (memos.TryGetValue(key, out kept) && kept.Generation == before) return (T)kept.Value;
			if (memos.Count >= MaxEntries) Forget();
			var made = make();
			if (Interlocked.Read(ref generation) == before) memos[key] = (before, made);
			return made;
		}
	}

	/// <summary>A file as one opening of it has it (FileCache.Open): its length and time, its bytes or the file open.</summary>
	internal sealed class FileSnapshot : IDisposable
	{
		public long Length;
		public DateTime LastWriteTimeUtc;
		public byte[] Content;        // a small file's bytes
		public FileStream Stream;     // a large file, open (sent from: HttpResponse.WriteFileStream)

		public void Dispose() => Stream?.Dispose();
	}
}

#endif
