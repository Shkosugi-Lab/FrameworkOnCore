// libfoccase.so: file names without regard to case, as on Windows, for a .NET Framework application run on Linux.
//
// Preloaded into the process (LD_PRELOAD), it wraps the C library's file functions. A call is made as asked first; only
// when it fails because a name is not there (ENOENT, ENOTDIR) is the path looked up again, each of its names matched
// without regard to case, and the call made once more with the names as they are on disk. A file or folder created
// under a name that exists under another case is that one (as Windows does); a new name goes into the folders as they
// are. Everything in the process goes through it: the application, the libraries it uses, the runtime (assemblies,
// configuration files), the files ASP.NET serves.
//
//   FOC_CASE_ROOTS  the folders it applies to, separated by ':' (the site, its data). Unset: it does nothing.
//   FOC_CASE_LOG    1: each path found under another case is written (once) to the standard error (the case mismatches of
//                   the application, to fix or to know of).
//
// Names are compared as Windows compares file names (NTFS's upcase table, casemap.h: make-casemap.ps1): UTF-8 names,
// character by character in upper case; bytes that are not UTF-8, and characters outside the Basic Multilingual Plane,
// as they are.
// Of several names differing only in case (which Windows cannot have), the first in byte order is taken.

#define _GNU_SOURCE
#include <dirent.h>
#include <dlfcn.h>
#include <errno.h>
#include <fcntl.h>
#include <limits.h>
#include <pthread.h>
#include <stdarg.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/stat.h>
#include <sys/inotify.h>
#include <sys/statfs.h>
#include <sys/types.h>
#include <unistd.h>

#include "casemap.h"

// ---------------------------------------------------------------------------------------------------------------
// Configuration

#define MAX_ROOTS 16
static char roots[MAX_ROOTS][PATH_MAX];
static size_t root_lengths[MAX_ROOTS];
static int root_count;
static int logging;
static pthread_once_t configured = PTHREAD_ONCE_INIT;

static void configure(void)
{
    const char *value = getenv("FOC_CASE_ROOTS");
    logging = getenv("FOC_CASE_LOG") != NULL && strcmp(getenv("FOC_CASE_LOG"), "1") == 0;
    if (value == NULL) return;
    const char *start = value;
    while (*start != '\0' && root_count < MAX_ROOTS)
    {
        const char *end = strchr(start, ':');
        size_t length = end != NULL ? (size_t)(end - start) : strlen(start);
        while (length > 1 && start[length - 1] == '/') length--;
        if (length > 0 && length < PATH_MAX && start[0] == '/')
        {
            memcpy(roots[root_count], start, length);
            roots[root_count][length] = '\0';
            root_lengths[root_count] = length;
            root_count++;
        }
        if (end == NULL) break;
        start = end + 1;
    }
}

// The root an absolute path is under (the length of its prefix), or 0.
static size_t root_of(const char *path)
{
    pthread_once(&configured, configure);
    for (int i = 0; i < root_count; i++)
    {
        size_t length = root_lengths[i];
        if (length == 1) return 1;   // "/"
        if (strncmp(path, roots[i], length) == 0 && (path[length] == '/' || path[length] == '\0')) return length;
    }
    return 0;
}

// ---------------------------------------------------------------------------------------------------------------
// The C library's functions

// The next definition (the C library's), looked up once.
#define REAL(name)                                                                                 \
    static __typeof__(name) *real_##name;                                                          \
    static __typeof__(name) *get_##name(void)                                                      \
    {                                                                                              \
        if (real_##name == NULL) real_##name = (__typeof__(name) *)dlsym(RTLD_NEXT, #name);        \
        return real_##name;                                                                        \
    }
#define LOAD(name) (get_##name())

// A function the C library has in several versions (the old one behaves otherwise: realpath@GLIBC_2.2.5 refuses a NULL
// buffer, which the .NET host passes): the version programs are linked to now, where there is one (x86_64), looked up
// by name. dlsym may give the old one (glibc 2.35 does).
#define REAL_VERSIONED(name, version)                                                              \
    static __typeof__(name) *real_##name;                                                          \
    static __typeof__(name) *get_##name(void)                                                      \
    {                                                                                              \
        if (real_##name == NULL) real_##name = (__typeof__(name) *)dlvsym(RTLD_NEXT, #name, version); \
        if (real_##name == NULL) real_##name = (__typeof__(name) *)dlsym(RTLD_NEXT, #name);        \
        return real_##name;                                                                        \
    }

// The version of struct stat the old stat functions are asked for: the processor's (x86_64 1, aarch64 0), as the
// headers built against (glibc 2.31) define it. A wrong one fails every call (EINVAL).
#ifndef _STAT_VER
#error "_STAT_VER: build against glibc headers that define it (before 2.33: Dockerfile.build)"
#endif

extern int __xstat64(int, const char *, struct stat64 *);
extern int __lxstat64(int, const char *, struct stat64 *);
extern int __fxstatat64(int, int, const char *, struct stat64 *, int);

REAL(open); REAL(open64); REAL(openat); REAL(openat64); REAL(creat); REAL(creat64);
REAL(stat); REAL(stat64); REAL(lstat); REAL(lstat64); REAL(fstatat); REAL(fstatat64);
REAL(__xstat64); REAL(__lxstat64); REAL(__fxstatat64);
REAL(access); REAL(faccessat); REAL(opendir); REAL(mkdir); REAL(rmdir); REAL(unlink); REAL(rename);
REAL(link); REAL(symlink); REAL(readlink); REAL_VERSIONED(realpath, "GLIBC_2.3"); REAL(chmod); REAL(utimensat); REAL(chdir);
REAL(statfs); REAL(statfs64); REAL(fopen); REAL(fopen64); REAL(truncate); REAL(mkfifo); REAL(pathconf);
REAL(inotify_add_watch); REAL_VERSIONED(dlopen, "GLIBC_2.34");

static int exists(const char *path)
{
    struct stat64 s;
    return LOAD(__lxstat64)(_STAT_VER, path, &s) == 0;
}

// ---------------------------------------------------------------------------------------------------------------
// Folder listings, kept while the folder is unchanged (its modification time)

#define CACHE_SIZE 1024
typedef struct
{
    char *path;
    struct timespec modified;
    ino_t inode;
    char *names;      // the names, one after another, each ended by '\0'
    size_t count;
} Listing;

static Listing cache[CACHE_SIZE];
static pthread_mutex_t cache_lock = PTHREAD_MUTEX_INITIALIZER;

static size_t hash(const char *s)
{
    size_t h = 1469598103934665603ULL;
    for (; *s; s++) h = (h ^ (unsigned char)*s) * 1099511628211ULL;
    return h % CACHE_SIZE;
}

// A character of Windows' file names as NTFS compares it: upper case by its table (casemap.h).
static unsigned upcase(unsigned c)
{
    if (c < 0x80) return c >= 'a' && c <= 'z' ? c - 32 : c;
    if (c > 0xFFFF) return c;
    size_t low = 0, high = sizeof upcase_table / sizeof upcase_table[0];
    while (low < high)
    {
        size_t middle = (low + high) / 2;
        unsigned key = upcase_table[middle][0];
        if (key == c) return upcase_table[middle][1];
        if (key < c) low = middle + 1; else high = middle;
    }
    return c;
}

// The next character of a UTF-8 name; a byte that is not UTF-8 stands for itself (compared as it is).
static unsigned next_character(const unsigned char **s)
{
    const unsigned char *p = *s;
    unsigned c = p[0];
    int more = c < 0x80 ? 0 : (c & 0xE0) == 0xC0 ? 1 : (c & 0xF0) == 0xE0 ? 2 : (c & 0xF8) == 0xF0 ? 3 : -1;
    if (more == 0) { *s = p + 1; return c; }
    if (more < 0) { *s = p + 1; return 0x110000 + c; }
    unsigned value = c & (0x3Fu >> more);
    for (int i = 1; i <= more; i++)
    {
        if ((p[i] & 0xC0) != 0x80) { *s = p + 1; return 0x110000 + c; }
        value = (value << 6) | (p[i] & 0x3F);
    }
    *s = p + more + 1;
    return value;
}

// Whether two names are the same name on Windows (NTFS: both in upper case by its table, character by character).
static int same_name(const char *a, const char *b)
{
    const unsigned char *x = (const unsigned char *)a, *y = (const unsigned char *)b;
    while (*x && *y)
    {
        unsigned cx = next_character(&x), cy = next_character(&y);
        if (cx != cy && upcase(cx) != upcase(cy)) return 0;
    }
    return *x == *y;
}

// The name in the folder that matches without regard to case (the first in byte order), copied to found.
static int find_in_folder(const char *folder, const char *name, char *found, size_t size)
{
    struct stat64 s;
    if (LOAD(__xstat64)(_STAT_VER, folder, &s) != 0 || !S_ISDIR(s.st_mode)) return 0;
    int result = 0;
    int ambiguous = 0;
    pthread_mutex_lock(&cache_lock);
    Listing *entry = &cache[hash(folder)];
    if (entry->path == NULL || strcmp(entry->path, folder) != 0 || entry->inode != s.st_ino ||
        entry->modified.tv_sec != s.st_mtim.tv_sec || entry->modified.tv_nsec != s.st_mtim.tv_nsec)
    {
        free(entry->path); free(entry->names);
        entry->path = NULL; entry->names = NULL; entry->count = 0;
        DIR *dir = LOAD(opendir)(folder);
        if (dir != NULL)
        {
            size_t capacity = 4096, used = 0, count = 0;
            char *names = malloc(capacity);
            struct dirent *e;
            while (names != NULL && (e = readdir(dir)) != NULL)
            {
                if (strcmp(e->d_name, ".") == 0 || strcmp(e->d_name, "..") == 0) continue;
                size_t length = strlen(e->d_name) + 1;
                if (used + length > capacity)
                {
                    while (used + length > capacity) capacity *= 2;
                    char *grown = realloc(names, capacity);
                    if (grown == NULL) { free(names); names = NULL; break; }
                    names = grown;
                }
                memcpy(names + used, e->d_name, length);
                used += length;
                count++;
            }
            closedir(dir);
            if (names != NULL)
            {
                entry->path = strdup(folder);
                entry->names = names;
                entry->count = count;
                entry->inode = s.st_ino;
                entry->modified = s.st_mtim;
            }
        }
    }
    if (entry->path != NULL)
    {
        const char *best = NULL;
        const char *n = entry->names;
        for (size_t i = 0; i < entry->count; i++, n += strlen(n) + 1)
        {
            if (!same_name(n, name)) continue;
            if (best != NULL) ambiguous = 1;
            if (best == NULL || strcmp(n, best) < 0) best = n;
        }
        if (best != NULL && strlen(best) < size)
        {
            strcpy(found, best);
            result = 1;
        }
    }
    pthread_mutex_unlock(&cache_lock);
    if (ambiguous && logging) fprintf(stderr, "foccase: %s: several names differ from '%s' only in case; '%s' taken\n", folder, name, found);
    return result;
}

// ---------------------------------------------------------------------------------------------------------------
// Resolution

// The absolute form of a path (relative to the working folder, or to a folder descriptor).
static int absolute(int dirfd, const char *path, char *out)
{
    if (path == NULL || path[0] == '\0') return 0;
    if (path[0] == '/')
    {
        if (strlen(path) >= PATH_MAX) return 0;
        strcpy(out, path);
        return 1;
    }
    char base[PATH_MAX];
    if (dirfd == AT_FDCWD)
    {
        if (getcwd(base, sizeof base) == NULL) return 0;
    }
    else
    {
        char link[64];
        snprintf(link, sizeof link, "/proc/self/fd/%d", dirfd);
        ssize_t length = LOAD(readlink)(link, base, sizeof base - 1);
        if (length <= 0) return 0;
        base[length] = '\0';
    }
    if (strlen(base) + 1 + strlen(path) >= PATH_MAX) return 0;
    snprintf(out, PATH_MAX, "%s/%s", strcmp(base, "/") == 0 ? "" : base, path);
    return 1;
}

// Each path found under another case is written once (the table of those written; when it is full, no more).
#define LOGGED_SIZE 8192
static uint64_t logged[LOGGED_SIZE];
static pthread_mutex_t logged_lock = PTHREAD_MUTEX_INITIALIZER;

static int first_time(const char *path)
{
    uint64_t h = 1469598103934665603ULL;
    for (const char *s = path; *s; s++) h = (h ^ (unsigned char)*s) * 1099511628211ULL;
    if (h == 0) h = 1;
    int first = 0;
    pthread_mutex_lock(&logged_lock);
    for (size_t i = 0, slot = h % LOGGED_SIZE; i < LOGGED_SIZE; i++, slot = (slot + 1) % LOGGED_SIZE)
    {
        if (logged[slot] == h) break;
        if (logged[slot] == 0) { logged[slot] = h; first = 1; break; }
    }
    pthread_mutex_unlock(&logged_lock);
    return first;
}
// The path with each of its names as it is on disk (under a root). Names not found are kept as written (a file to
// create). 1 when the path changed.
static int resolve(int dirfd, const char *path, char *out)
{
    char full[PATH_MAX];
    if (!absolute(dirfd, path, full)) return 0;
    size_t root = root_of(full);
    if (root == 0) return 0;

    char result[PATH_MAX];
    memcpy(result, full, root);
    result[root] = '\0';
    size_t length = root == 1 ? 0 : root;   // "/" is written as the separator of the first name
    const char *rest = full + root;
    int changed = 0;
    int searching = 1;
    while (*rest != '\0')
    {
        while (*rest == '/') rest++;
        if (*rest == '\0') break;
        const char *end = strchr(rest, '/');
        size_t n = end != NULL ? (size_t)(end - rest) : strlen(rest);
        char name[NAME_MAX + 1];
        if (n > NAME_MAX) return 0;
        memcpy(name, rest, n);
        name[n] = '\0';
        rest += n;

        if (strcmp(name, ".") == 0) continue;
        if (strcmp(name, "..") == 0)
        {
            // Up, not above the root.
            char *slash = strrchr(result, '/');
            if (slash == NULL || (size_t)(slash - result) < (root == 1 ? 0 : root)) return 0;
            length = (size_t)(slash - result);
            result[length] = '\0';
            continue;
        }
        if (length + 1 + n >= PATH_MAX) return 0;
        char *folder_end = result + length;
        result[length] = '/';
        memcpy(result + length + 1, name, n + 1);
        if (searching && !exists(result))
        {
            *folder_end = '\0';
            char found[NAME_MAX + 1];
            const char *folder = length == 0 ? "/" : result;
            if (find_in_folder(folder, name, found, sizeof found))
            {
                result[length] = '/';
                strcpy(result + length + 1, found);
                n = strlen(found);
                changed = 1;
            }
            else
            {
                result[length] = '/';
                searching = 0;   // the rest does not exist: as written
            }
        }
        length += 1 + n;
    }
    if (length == 0) { result[0] = '/'; result[1] = '\0'; }
    if (!changed) return 0;
    strcpy(out, result);
    if (logging && first_time(full)) fprintf(stderr, "foccase: %s -> %s\n", full, out);
    return 1;
}

static int missing(void) { return errno == ENOENT || errno == ENOTDIR; }

// A lookup: as asked; if a name is not there, once more with the names on disk.
#define LOOKUP(call, retry)                                                           \
    do {                                                                              \
        __typeof__(call) result = call;                                               \
        if (result == failure && missing() && path != NULL && path[0] != '\0')        \
        {                                                                             \
            int saved = errno;                                                        \
            char resolved[PATH_MAX];                                                  \
            if (resolve(AT_FDCWD, path, resolved)) { path = resolved; return retry; } \
            errno = saved;                                                            \
        }                                                                             \
        return result;                                                                \
    } while (0)

// A creation: a name that exists under another case is that one, and the folders are the ones there.
static const char *for_creation(int dirfd, const char *path, char *buffer)
{
    if (path == NULL || path[0] == '\0') return path;
    char full[PATH_MAX];
    if (!absolute(dirfd, path, full) || root_of(full) == 0 || exists(full)) return path;
    return resolve(dirfd, path, buffer) ? buffer : path;
}

// ---------------------------------------------------------------------------------------------------------------
// The functions wrapped

static int creates(int flags) { return (flags & O_CREAT) != 0; }

static mode_t mode_of(int flags, va_list arguments)
{
    return (flags & (O_CREAT | O_TMPFILE)) ? (mode_t)va_arg(arguments, int) : 0;
}

#define OPEN_LIKE(name)                                                                              \
    int name(const char *path, int flags, ...)                                                       \
    {                                                                                                \
        va_list arguments; va_start(arguments, flags); mode_t mode = mode_of(flags, arguments); va_end(arguments); \
        char buffer[PATH_MAX];                                                                       \
        if (creates(flags)) return LOAD(name)(for_creation(AT_FDCWD, path, buffer), flags, mode);    \
        int failure = -1;                                                                            \
        LOOKUP(LOAD(name)(path, flags, mode), LOAD(name)(path, flags, mode));                        \
    }
OPEN_LIKE(open)
OPEN_LIKE(open64)

#define OPENAT_LIKE(name)                                                                            \
    int name(int dirfd, const char *path, int flags, ...)                                            \
    {                                                                                                \
        va_list arguments; va_start(arguments, flags); mode_t mode = mode_of(flags, arguments); va_end(arguments); \
        char buffer[PATH_MAX];                                                                       \
        if (creates(flags)) return LOAD(name)(dirfd, for_creation(dirfd, path, buffer), flags, mode); \
        int result = LOAD(name)(dirfd, path, flags, mode);                                           \
        if (result == -1 && missing() && resolve(dirfd, path, buffer))                               \
            return LOAD(name)(AT_FDCWD, buffer, flags, mode);                                        \
        return result;                                                                               \
    }
OPENAT_LIKE(openat)
OPENAT_LIKE(openat64)

int creat(const char *path, mode_t mode) { char b[PATH_MAX]; return LOAD(creat)(for_creation(AT_FDCWD, path, b), mode); }
int creat64(const char *path, mode_t mode) { char b[PATH_MAX]; return LOAD(creat64)(for_creation(AT_FDCWD, path, b), mode); }

int stat(const char *path, struct stat *s) { int failure = -1; LOOKUP(LOAD(stat)(path, s), LOAD(stat)(path, s)); }
int stat64(const char *path, struct stat64 *s) { int failure = -1; LOOKUP(LOAD(stat64)(path, s), LOAD(stat64)(path, s)); }
int lstat(const char *path, struct stat *s) { int failure = -1; LOOKUP(LOAD(lstat)(path, s), LOAD(lstat)(path, s)); }
int lstat64(const char *path, struct stat64 *s) { int failure = -1; LOOKUP(LOAD(lstat64)(path, s), LOAD(lstat64)(path, s)); }
int __xstat64(int version, const char *path, struct stat64 *s) { int failure = -1; LOOKUP(LOAD(__xstat64)(version, path, s), LOAD(__xstat64)(version, path, s)); }
int __lxstat64(int version, const char *path, struct stat64 *s) { int failure = -1; LOOKUP(LOAD(__lxstat64)(version, path, s), LOAD(__lxstat64)(version, path, s)); }

#define STATAT_LIKE(name, type)                                                                      \
    int name(int dirfd, const char *path, type *s, int flags)                                        \
    {                                                                                                \
        int result = LOAD(name)(dirfd, path, s, flags);                                              \
        char buffer[PATH_MAX];                                                                       \
        if (result == -1 && missing() && path != NULL && path[0] != '\0' && resolve(dirfd, path, buffer)) \
            return LOAD(name)(AT_FDCWD, buffer, s, flags);                                           \
        return result;                                                                               \
    }
STATAT_LIKE(fstatat, struct stat)
STATAT_LIKE(fstatat64, struct stat64)

int __fxstatat64(int version, int dirfd, const char *path, struct stat64 *s, int flags)
{
    int result = LOAD(__fxstatat64)(version, dirfd, path, s, flags);
    char buffer[PATH_MAX];
    if (result == -1 && missing() && path != NULL && path[0] != '\0' && resolve(dirfd, path, buffer))
        return LOAD(__fxstatat64)(version, AT_FDCWD, buffer, s, flags);
    return result;
}

int access(const char *path, int mode) { int failure = -1; LOOKUP(LOAD(access)(path, mode), LOAD(access)(path, mode)); }

int faccessat(int dirfd, const char *path, int mode, int flags)
{
    int result = LOAD(faccessat)(dirfd, path, mode, flags);
    char buffer[PATH_MAX];
    if (result == -1 && missing() && resolve(dirfd, path, buffer)) return LOAD(faccessat)(AT_FDCWD, buffer, mode, flags);
    return result;
}

DIR *opendir(const char *path) { DIR *failure = NULL; LOOKUP(LOAD(opendir)(path), LOAD(opendir)(path)); }
int rmdir(const char *path) { int failure = -1; LOOKUP(LOAD(rmdir)(path), LOAD(rmdir)(path)); }
int unlink(const char *path) { int failure = -1; LOOKUP(LOAD(unlink)(path), LOAD(unlink)(path)); }
ssize_t readlink(const char *path, char *target, size_t size) { ssize_t failure = -1; LOOKUP(LOAD(readlink)(path, target, size), LOAD(readlink)(path, target, size)); }
char *realpath(const char *path, char *resolved_path) { char *failure = NULL; LOOKUP(LOAD(realpath)(path, resolved_path), LOAD(realpath)(path, resolved_path)); }
int chmod(const char *path, mode_t mode) { int failure = -1; LOOKUP(LOAD(chmod)(path, mode), LOAD(chmod)(path, mode)); }
int chdir(const char *path) { int failure = -1; LOOKUP(LOAD(chdir)(path), LOAD(chdir)(path)); }
int statfs(const char *path, struct statfs *s) { int failure = -1; LOOKUP(LOAD(statfs)(path, s), LOAD(statfs)(path, s)); }
int statfs64(const char *path, struct statfs64 *s) { int failure = -1; LOOKUP(LOAD(statfs64)(path, s), LOAD(statfs64)(path, s)); }
int truncate(const char *path, off_t length) { int failure = -1; LOOKUP(LOAD(truncate)(path, length), LOAD(truncate)(path, length)); }
long pathconf(const char *path, int name) { long failure = -1; LOOKUP(LOAD(pathconf)(path, name), LOAD(pathconf)(path, name)); }
int inotify_add_watch(int fd, const char *path, uint32_t mask) { int failure = -1; LOOKUP(LOAD(inotify_add_watch)(fd, path, mask), LOAD(inotify_add_watch)(fd, path, mask)); }

int utimensat(int dirfd, const char *path, const struct timespec times[2], int flags)
{
    int result = LOAD(utimensat)(dirfd, path, times, flags);
    char buffer[PATH_MAX];
    if (result == -1 && missing() && path != NULL && path[0] != '\0' && resolve(dirfd, path, buffer))
        return LOAD(utimensat)(AT_FDCWD, buffer, times, flags);
    return result;
}

int mkdir(const char *path, mode_t mode) { char b[PATH_MAX]; return LOAD(mkdir)(for_creation(AT_FDCWD, path, b), mode); }
int mkfifo(const char *path, mode_t mode) { char b[PATH_MAX]; return LOAD(mkfifo)(for_creation(AT_FDCWD, path, b), mode); }
int symlink(const char *target, const char *path) { char b[PATH_MAX]; return LOAD(symlink)(target, for_creation(AT_FDCWD, path, b)); }

int link(const char *from, const char *to)
{
    char f[PATH_MAX], t[PATH_MAX];
    const char *source = from;
    if (!exists(from) && resolve(AT_FDCWD, from, f)) source = f;
    return LOAD(link)(source, for_creation(AT_FDCWD, to, t));
}

// A rename to a name that exists under another case replaces that file (as Windows does), unless it is the file itself:
// then only the case of its name changes.
int rename(const char *from, const char *to)
{
    char f[PATH_MAX], t[PATH_MAX];
    const char *source = from;
    if (!exists(from) && resolve(AT_FDCWD, from, f)) source = f;
    const char *target = for_creation(AT_FDCWD, to, t);
    if (target != to)
    {
        struct stat64 a, b;
        if (LOAD(__lxstat64)(_STAT_VER, source, &a) == 0 && LOAD(__lxstat64)(_STAT_VER, target, &b) == 0 && a.st_ino == b.st_ino && a.st_dev == b.st_dev)
        {
            // The same file: its folder as on disk, its new name as written.
            const char *name = strrchr(to, '/');
            name = name != NULL ? name + 1 : to;
            const char *slash = strrchr(t, '/');
            if (slash != NULL && (size_t)(slash - t) + 1 + strlen(name) < PATH_MAX)
            {
                strcpy(t + (slash - t) + 1, name);
                target = t;
            }
        }
    }
    return LOAD(rename)(source, target);
}

static int writes(const char *mode) { return mode != NULL && (strchr(mode, 'w') != NULL || strchr(mode, 'a') != NULL); }

FILE *fopen(const char *path, const char *mode)
{
    char buffer[PATH_MAX];
    if (writes(mode)) return LOAD(fopen)(for_creation(AT_FDCWD, path, buffer), mode);
    FILE *failure = NULL;
    LOOKUP(LOAD(fopen)(path, mode), LOAD(fopen)(path, mode));
}

FILE *fopen64(const char *path, const char *mode)
{
    char buffer[PATH_MAX];
    if (writes(mode)) return LOAD(fopen64)(for_creation(AT_FDCWD, path, buffer), mode);
    FILE *failure = NULL;
    LOOKUP(LOAD(fopen64)(path, mode), LOAD(fopen64)(path, mode));
}

// Native libraries by path (a DLL's name as the application wrote it).
void *dlopen(const char *path, int flags)
{
    void *result = LOAD(dlopen)(path, flags);
    if (result == NULL && path != NULL && strchr(path, '/') != NULL)
    {
        char buffer[PATH_MAX];
        if (resolve(AT_FDCWD, path, buffer)) return LOAD(dlopen)(buffer, flags);
    }
    return result;
}
