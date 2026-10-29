using System.CodeDom.Compiler;
using WebFormsForCore.CodeDom.Compiler;

namespace FrameworkOnCore.Runtime.Tests.CodeDom;

/// <summary>
/// The port's Executor (ExecutorPortable), which stands for .NET Framework's System.CodeDom.Compiler.Executor
/// (learn.microsoft.com: ExecWaitWithCapture "executes the command using the specified temporary files and waits for the call to
/// return"; referencesource.microsoft.com: Executor.cs, "the reference source" below): the output file begins with the command
/// line, the process's output and errors in the files, its exit code returned. Where .NET Framework's cannot apply (no
/// impersonation; csc.dll and vbc.dll run in-process by dotnet), the code's comments.
/// </summary>
public class ExecutorPortableTests : CompilationTest
{
    static readonly bool Windows = OperatingSystem.IsWindows();
    static string Shell => Windows ? Path.Combine(Environment.SystemDirectory, "cmd.exe") : "/bin/sh";
    static string Script(string script) => Windows ? "/c " + script : "-c \"" + script + "\"";
    static string Dotnet => Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "..", "..", "..", Windows ? "dotnet.exe" : "dotnet");

    [Fact] // the reference source: the output file "dir> command args", two empty lines, then the process's output; its errors in the error file; its exit code returned
    public void A_process_s_output_errors_and_exit_code()
    {
        var tempFiles = new TempFileCollection(Folder);
        string output = null, error = null;
        var exit = ExecutorPortable.ExecWaitWithCapture(Shell, Script("echo out&& echo err 1>&2&& exit 3"), Folder, tempFiles, ref output, ref error);
        Assert.Equal(3, exit);
        var lines = File.ReadAllLines(output);
        Assert.Equal(Folder + "> " + Shell + " " + Script("echo out&& echo err 1>&2&& exit 3"), lines[0]);
        Assert.Equal("", lines[1]);
        Assert.Equal("", lines[2]);
        Assert.Equal("out", lines[3].Trim());
        Assert.Equal("err", File.ReadAllText(error).Trim());
        Assert.Equal(Path.ChangeExtension(output, ".err"), error);
    }

    [Fact] // the reference source: the files given used (none made); the overloads the current directory and no user token
    public void The_files_given_and_the_overloads()
    {
        var tempFiles = new TempFileCollection(Folder);
        string output = Path.Combine(Folder, "given.out"), error = Path.Combine(Folder, "given.err");
        Assert.Equal(0, ExecutorPortable.ExecWaitWithCapture(Shell, Script("exit 0"), tempFiles, ref output, ref error));
        Assert.Equal(Path.Combine(Folder, "given.out"), output);
        Assert.StartsWith(Environment.CurrentDirectory + "> ", File.ReadAllLines(output)[0]);
        Assert.True(File.Exists(error));

        string o2 = null, e2 = null;
        Assert.Equal(4, ExecutorPortable.ExecWaitWithCapture(IntPtr.Zero, Shell, Script("exit 4"), new TempFileCollection(Folder), ref o2, ref e2));
        ExecutorPortable.ExecWait(Shell, Script("exit 0"), new TempFileCollection(Folder));
    }

    [Fact] // the port's choice: no impersonation, a user token not supported
    public void A_user_token_is_not_supported()
    {
        string output = null, error = null;
        Assert.Throws<PlatformNotSupportedException>(() =>
            ExecutorPortable.ExecWaitWithCapture(new IntPtr(1), Shell, "", Folder, new TempFileCollection(Folder), ref output, ref error));
    }

    [Fact] // the port's choice (Compilers.Compiler.Run): dotnet runs csc.dll / vbc.dll in-process; anything else as a process
    public void Dotnet_runs_other_things_as_a_process()
    {
        string output = null, error = null;
        var exit = ExecutorPortable.ExecWaitWithCapture(Dotnet, "--version", Folder, new TempFileCollection(Folder), ref output, ref error);
        Assert.Equal(0, exit);
        Assert.Equal(Environment.Version.Major, Version.Parse(File.ReadAllLines(output)[3].Split('-')[0]).Major);
    }
}
