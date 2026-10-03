using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Converter;

/// <summary>
/// How the converted application is deployed on Linux, in either of two ways (--deploy):
/// - container: a Dockerfile (and .dockerignore) in the output, built from it as the context;
/// - linux: deploy/linux/install.sh, which installs the site as a systemd service on a Linux machine.
/// Both start the application with deploy/start.sh, and share what IIS gave the application and Linux
/// does not:
/// - the original server's culture data (ICU data built from the culture profile, on the machine the
///   application runs on: they are for its ICU version; ICU_DATA);
/// - its culture (the profile's default culture: LANG);
/// - the deployment's settings: environment variables as Azure App Service gives them to .NET Framework
///   applications (APPSETTING_key, SQLCONNSTR_name: the runtime reads them), and files laid over the
///   site from a configuration folder (CONFIG_DIR) for the rest;
/// - the folder the application writes to (App_Data), kept when the application is replaced;
/// - file names without regard to case, as on Windows (--case-insensitive, on unless off: casefs/libfoccase.so, preloaded
///   by start.sh on the machines it loads on; FOC_CASE_INSENSITIVE=0 turns it off there).
/// </summary>
public sealed class DeployWriter(Report report, string outRoot, string runtimeDirectory)
{
    public void Write(string kinds, string site, ConvertedProject web, string? cultureProfile, bool caseInsensitive = true)
    {
        var container = kinds is "both" or "container";
        var linux = kinds is "both" or "linux";
        if (!container && !linux) return;

        var assembly = XDocument.Load(web.TargetPath).Descendants().FirstOrDefault(e => e.Name.LocalName == "AssemblyName")?.Value ?? web.Name;
        var app = new string(assembly.ToLowerInvariant().Select(c => char.IsAsciiLetterOrDigit(c) ? c : '-').ToArray()).Trim('-');
        var siteRelative = Path.GetRelativePath(outRoot, site).Replace('\\', '/');
        var deploy = Path.Combine(outRoot, "deploy");
        FrameworkOnCore.Analysis.FileTrees.Delete(deploy);
        Directory.CreateDirectory(deploy);

        string? culture = null;
        if (cultureProfile != null)
        {
            using var profile = JsonDocument.Parse(File.ReadAllText(cultureProfile));
            if (profile.RootElement.TryGetProperty("DefaultCulture", out var c)) culture = c.GetString();
            WriteIcu(Path.Combine(deploy, "icu"), cultureProfile);
        }
        var lang = culture != null ? culture.Replace('-', '_') + ".UTF-8" : "C.UTF-8";

        if (caseInsensitive) caseInsensitive = WriteCaseLibraries(Path.Combine(deploy, "casefs"));
        var drawing = DrawingUsers(site);
        if (drawing.Count > 0)
            report.Add(Report.Kind.Project, "deploy", $"System.Drawing on Linux: libgdiplus and fonts (fonts-liberation2: Arial, Times New Roman, Courier New's metrics) installed by the deployment, for {string.Join(", ", drawing.Take(6))}{(drawing.Count > 6 ? ", ..." : "")}");
        WriteText(Path.Combine(deploy, "start.sh"), StartScript.Replace("__DLL__", assembly));
        if (container)
        {
            WriteText(Path.Combine(outRoot, "Dockerfile"), Dockerfile(siteRelative, lang, cultureProfile != null, caseInsensitive, drawing.Count > 0));
            // The site the output itself (the web project built in place): all of it ("!." takes nothing back from "*").
            WriteText(Path.Combine(outRoot, ".dockerignore"), siteRelative == "."
                ? "# The build context is the conversion's output, the site itself (the web project built in place).\n**/obj\n"
                : $"# The build context is the conversion's output: only the site and deploy.\n*\n!{siteRelative}\n!deploy\n**/obj\n");
        }
        if (linux)
        {
            WriteText(Path.Combine(deploy, "linux", "install.sh"), InstallScript
                .Replace("__APP__", app).Replace("__SITE__", siteRelative).Replace("__LANG__", lang)
                .Replace("__ICU__", cultureProfile != null ? "1" : "").Replace("__GDIPLUS__", drawing.Count > 0 ? "1" : ""));
        }
        WriteText(Path.Combine(deploy, "README.md"), Readme(app, assembly, siteRelative, lang, container, linux, cultureProfile != null, caseInsensitive, drawing.Count > 0));
        report.Add(Report.Kind.Project, "deploy", $"{(container ? "Dockerfile" : "")}{(container && linux ? " and " : "")}{(linux ? "deploy/linux/install.sh (systemd)" : "")}: {app}, site {siteRelative}, culture {lang}, " +
            $"file names {(caseInsensitive ? "without regard to case (deploy/casefs; FOC_CASE_INSENSITIVE=0 turns it off)" : "case-sensitive")} (deploy/README.md)");
    }

    /// <summary>
    /// What in the site draws with System.Drawing (GDI+, libgdiplus on Linux): its assemblies that reference System.Drawing
    /// or System.Drawing.Common, not FrameworkOnCore's own (System.Web's and the mobile controls' references are its types',
    /// Color and the like, not drawing: the mobile controls draw only the font names a designer's property grid lists) nor .NET's System.Windows.Extensions (in every site, a dependency of the configuration's packages: its
    /// System.Drawing members are Windows' certificate dialogs and sounds), and pages whose markup or App_Code names
    /// System.Drawing (compiled on the server).
    /// </summary>
    public static List<string> DrawingUsers(string site)
    {
        var users = new List<string>();
        var own = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "System.Drawing", "System.Drawing.Common", "System.Web", "System.Web.Extensions", "System.Web.DynamicData", "System.Web.Mobile", "System.Web.Services", "System.Windows.Forms", "System.Windows.Extensions" };
        var bin = Path.Combine(site, "bin");
        if (Directory.Exists(bin))
        {
            foreach (var dll in Directory.EnumerateFiles(bin, "*.dll"))
            {
                if (own.Contains(Path.GetFileNameWithoutExtension(dll))) continue;
                try
                {
                    using var stream = File.OpenRead(dll);
                    using var pe = new System.Reflection.PortableExecutable.PEReader(stream);
                    if (!pe.HasMetadata) continue;
                    var metadata = System.Reflection.Metadata.PEReaderExtensions.GetMetadataReader(pe);
                    if (metadata.AssemblyReferences.Select(h => metadata.GetString(metadata.GetAssemblyReference(h).Name)).Any(n => n is "System.Drawing" or "System.Drawing.Common"))
                        users.Add(Path.GetFileName(dll));
                }
                catch (BadImageFormatException) { }
            }
        }
        foreach (var pattern in new[] { "*.aspx", "*.ascx", "*.master", "*.ashx", "*.asmx", "*.cshtml", "*.vbhtml", "*.cs", "*.vb" })
            foreach (var file in Directory.EnumerateFiles(site, pattern, SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(site, file);
                if (relative.StartsWith("bin" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || relative.StartsWith("obj" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) continue;
                // Code files count only where the server compiles them.
                if ((pattern is "*.cs" or "*.vb") && !relative.StartsWith("App_Code" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) continue;
                if (File.ReadAllText(file).Contains("System.Drawing", StringComparison.Ordinal)) users.Add(relative.Replace('\\', '/'));
            }
        return users;
    }

    // The library of file names without regard to case (casefs/libfoccase.so, built by casefs/build.ps1) for each
    // processor, and its source. False when it has not been built: the deployment is made without it.
    static readonly string[] caseRuntimes = ["linux-x64", "linux-arm64"];

    bool WriteCaseLibraries(string directory)
    {
        var built = Path.Combine(runtimeDirectory, "casefs", "out");
        var missing = caseRuntimes.Where(r => !File.Exists(Path.Combine(built, r, "libfoccase.so"))).ToList();
        if (missing.Count > 0)
        {
            report.Add(Report.Kind.Error, "deploy", $"casefs/out/{string.Join(", ", missing)}/libfoccase.so not built (casefs/build.ps1): deployed without file names without regard to case");
            return false;
        }
        foreach (var runtime in caseRuntimes)
        {
            Directory.CreateDirectory(Path.Combine(directory, runtime));
            File.Copy(Path.Combine(built, runtime, "libfoccase.so"), Path.Combine(directory, runtime, "libfoccase.so"), overwrite: true);
        }
        foreach (var source in new[] { "foccase.c", "casemap.h" })
            File.Copy(Path.Combine(runtimeDirectory, "casefs", source), Path.Combine(directory, source), overwrite: true);
        return true;
    }

    // The ICU data tool (CultureIcu, framework-dependent: the runtime runs it) and the profile.
    void WriteIcu(string directory, string cultureProfile)
    {
        Directory.CreateDirectory(directory);
        File.Copy(cultureProfile, Path.Combine(directory, "culture-profile.json"), overwrite: true);
        var project = Path.Combine(runtimeDirectory, "icu", "CultureIcu", "CultureIcu.csproj");
        var start = new ProcessStartInfo("dotnet", $"publish \"{project}\" -c Release -o \"{Path.Combine(directory, "tool")}\" -nologo -v q") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
        using var process = Process.Start(start)!;
        var (output, error) = ProcessOutput.ReadAll(process);
        process.WaitForExit();
        if (process.ExitCode != 0) report.Add(Report.Kind.Error, "deploy", $"CultureIcu could not be published: {output} {error}");
        WriteText(Path.Combine(directory, "build-icu-data.sh"), IcuScript);
    }

    static void WriteText(string path, string text)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        // Shell scripts and the Dockerfile are read on Linux: LF.
        File.WriteAllText(path, text.Replace("\r\n", "\n"), new UTF8Encoding(false));
    }

    const string StartScript = """
        #!/bin/bash
        # Starts the application (generated by FrameworkOnCore). Used by the Dockerfile and the systemd unit.
        #   APP_DIR     the site (web.config, bin)
        #   CONFIG_DIR  files laid over the site before it starts (web.config, user.config ...), if any
        #   ICU_DATA    the original server's culture data (deploy/icu), if built
        #   FOC_CASE_INSENSITIVE  0: file names case-sensitive (by default without regard to case, as on Windows, when
        #               casefs is next to this script); FOC_CASE_ROOTS: the folders it applies to (the site by default,
        #               ":" between them); FOC_CASE_LOG=1: the names found under another case, on the standard error
        # Settings from the environment: APPSETTING_<key> (appSettings), SQLCONNSTR_<name> (connectionStrings).
        set -e
        app="${APP_DIR:?APP_DIR: the folder of the site}"
        here="$(cd "$(dirname "$0")" && pwd)"
        if [ -n "${CONFIG_DIR:-}" ] && [ -d "$CONFIG_DIR" ] && [ -n "$(ls -A "$CONFIG_DIR")" ]; then
            cp -r "$CONFIG_DIR"/. "$app"/
        fi
        if [ -n "${ICU_DATA:-}" ] && [ ! -d "$ICU_DATA" ]; then unset ICU_DATA; fi
        # File names without regard to case (casefs/<runtime>/libfoccase.so), if the library loads on this machine (glibc).
        if [ -d "$here/casefs" ] && [ "${FOC_CASE_INSENSITIVE:-1}" != 0 ]; then
            case "$(uname -m)" in x86_64) runtime=linux-x64 ;; aarch64|arm64) runtime=linux-arm64 ;; *) runtime= ;; esac
            library="$here/casefs/$runtime/libfoccase.so"
            if [ -n "$runtime" ] && [ -f "$library" ] && [ -z "$(LD_PRELOAD="$library" /bin/true 2>&1)" ]; then
                export LD_PRELOAD="$library${LD_PRELOAD:+:$LD_PRELOAD}"
                export FOC_CASE_ROOTS="${FOC_CASE_ROOTS:-$app}"
                # The library finds the names: the runtime's own matching of case (WebFormsForCore, FrameworkOnCore.Compat)
                # would only do it twice. Without the library that one stays.
                export WEBFORMSFORCORE_PATH_CASING="${WEBFORMSFORCORE_PATH_CASING:-0}"
            else
                echo "file names are case-sensitive: casefs has no library that loads here ($(uname -m))" >&2
            fi
        fi
        cd "$app"
        exec dotnet "bin/__DLL__.dll" "$@"

        """;

    const string IcuScript = """
        #!/bin/bash
        # Builds ICU data that make .NET's culture data the original server's (generated by FrameworkOnCore).
        # Runs on the machine the application runs on: the data are for its ICU version.
        #   build-icu-data.sh <culture-profile.json> <out-directory>     then ICU_DATA=<out-directory>
        # Needs the .NET runtime, ICU's tools (genrb, icuinfo: icu-devtools) and access to github.com
        # (ICU's data sources of that version).
        set -euo pipefail
        profile="$1"
        out="$2"
        here="$(cd "$(dirname "$0")" && pwd)"
        if ! command -v genrb >/dev/null 2>&1; then
            if command -v apt-get >/dev/null 2>&1; then
                apt-get update -qq >/dev/null && apt-get install -y -qq icu-devtools >/dev/null
            else
                echo "genrb is needed: install ICU's tools (Debian/Ubuntu: icu-devtools; RHEL/Fedora: icu)" >&2
                exit 1
            fi
        fi
        version="$(icuinfo 2>/dev/null | sed -n 's/.*"version">\([0-9.]*\)<.*/\1/p' | head -1)"
        if [ -z "$version" ]; then echo "ICU version not found (icuinfo)" >&2; exit 1; fi
        echo "ICU $version"
        mkdir -p "$out"
        dotnet "$here/tool/CultureIcu.dll" generate "$profile" "$version" "$out"
        echo "--- check (ICU_DATA=$out)"
        ICU_DATA="$out" dotnet "$here/tool/CultureIcu.dll" diff "$profile" || true

        """;

    static string Dockerfile(string site, string lang, bool icu, bool caseInsensitive, bool gdiplus)
    {
        var text = new StringBuilder("""
            # Generated by FrameworkOnCore. Built from the conversion's output folder:
            #   docker build -t <name> .
            #   docker run -p 8080:8080 -e SQLCONNSTR_<name>="..." -e APPSETTING_<key>="..." -v <data>:/app/App_Data <name>
            # See deploy/README.md.

            """);
        if (icu)
        {
            text.Append("""
                # The original server's culture data, for the runtime image's ICU version (the same base image).
                FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS icu
                RUN apt-get update && apt-get install -y --no-install-recommends icu-devtools && rm -rf /var/lib/apt/lists/*
                COPY deploy/icu /opt/foc/icu
                RUN bash /opt/foc/icu/build-icu-data.sh /opt/foc/icu/culture-profile.json /opt/foc/icu-data


                """);
        }
        text.Append("""
            FROM mcr.microsoft.com/dotnet/aspnet:10.0

            """);
        if (gdiplus)
            text.Append("""
                # System.Drawing's Linux implementation (FrameworkOnCore's System.Drawing.Common) draws with libgdiplus; the fonts
                # have the metrics of Windows' Arial, Times New Roman and Courier New (fontconfig maps those names to them).
                RUN apt-get update && apt-get install -y --no-install-recommends libgdiplus fonts-liberation2 && rm -rf /var/lib/apt/lists/*

                """);
        // A path with a space (DNN's "DNN Platform/Website") in the JSON form: in the other one it is two sources.
        var copy = site.IndexOf(' ') >= 0 ? $"[\"{site.Replace("\\", "\\\\").Replace("\"", "\\\"")}\", \"/app\"]" : $"{site} /app";
        text.Append($"""
            # The site, writable by the application (it writes to App_Data, and some to more: installers).
            COPY --chown=app:app {copy}
            # App_Data the application's too where the site has none (the volume's folder is made as root otherwise: the
            # runtime's App_Data/machine.config not written, the site not started).
            RUN mkdir -p /app/App_Data && chown app:app /app/App_Data
            COPY deploy/start.sh /opt/foc/start.sh

            """);
        if (caseInsensitive) text.Append("# File names without regard to case, as on Windows (start.sh preloads it; FOC_CASE_INSENSITIVE=0 turns it off).\nCOPY deploy/casefs /opt/foc/casefs\n");
        if (icu) text.Append("COPY --from=icu /opt/foc/icu-data /opt/foc/icu-data\nENV ICU_DATA=/opt/foc/icu-data\n");
        text.Append($"""
            ENV APP_DIR=/app CONFIG_DIR=/config LANG={lang} ASPNETCORE_HTTP_PORTS=8080
            USER app
            # The application's data: kept when the container is replaced.
            VOLUME /app/App_Data
            EXPOSE 8080
            ENTRYPOINT ["bash", "/opt/foc/start.sh"]

            """);
        return text.ToString();
    }

    const string InstallScript = """
        #!/bin/bash
        # Installs __APP__ as a systemd service on a Linux machine (generated by FrameworkOnCore; see ../README.md).
        #   sudo ./install.sh [--prefix /opt/__APP__] [--port 5000] [--user __APP__] [--no-start]
        # Again for an update: the site is replaced, its App_Data kept.
        set -euo pipefail
        here="$(cd "$(dirname "$0")" && pwd)"
        output="$(cd "$here/../.." && pwd)"
        app=__APP__
        prefix=/opt/$app
        port=5000
        user=$app
        start=1
        while [ $# -gt 0 ]; do
            case "$1" in
                --prefix) prefix="$2"; shift 2 ;;
                --port) port="$2"; shift 2 ;;
                --user) user="$2"; shift 2 ;;
                --no-start) start=; shift ;;
                *) echo "unknown option: $1" >&2; exit 2 ;;
            esac
        done
        if [ "$(id -u)" != 0 ]; then echo "run as root (sudo)" >&2; exit 1; fi

        # The ASP.NET Core 10 runtime.
        if ! dotnet --list-runtimes 2>/dev/null | grep -q '^Microsoft.AspNetCore.App 10\.'; then
            echo "installing the ASP.NET Core 10 runtime in /usr/share/dotnet (dotnet-install.sh)"
            command -v curl >/dev/null || { apt-get update -qq && apt-get install -y -qq curl ca-certificates; }
            curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
            bash /tmp/dotnet-install.sh --channel 10.0 --runtime aspnetcore --install-dir /usr/share/dotnet >/dev/null
            ln -sf /usr/share/dotnet/dotnet /usr/bin/dotnet
        fi

        # System.Drawing's Linux implementation draws with libgdiplus; fonts with Windows' Arial, Times New Roman and Courier
        # New's metrics.
        if [ -n "__GDIPLUS__" ]; then
            if command -v apt-get >/dev/null; then
                apt-get update -qq && apt-get install -y -qq --no-install-recommends libgdiplus fonts-liberation2 >/dev/null
            elif command -v dnf >/dev/null; then
                dnf install -y -q libgdiplus liberation-sans-fonts liberation-serif-fonts liberation-mono-fonts || echo "libgdiplus is in EPEL on RHEL: dnf install epel-release first" >&2
            else
                echo "install libgdiplus and the Liberation fonts (System.Drawing needs them)" >&2
            fi
        fi

        id "$user" >/dev/null 2>&1 || useradd --system --home-dir "$prefix" --shell /usr/sbin/nologin "$user"
        mkdir -p "$prefix/site"
        # The site; App_Data is the application's (kept on an update).
        if [ -d "$prefix/site/App_Data" ]; then
            (cd "$output/__SITE__" && tar --exclude=./App_Data -cf - .) | (cd "$prefix/site" && tar xf -)
        else
            cp -r "$output/__SITE__"/. "$prefix/site/"
        fi
        cp "$output/deploy/start.sh" "$prefix/start.sh"
        # File names without regard to case (start.sh preloads it; FOC_CASE_INSENSITIVE=0 in /etc/$app/environment turns
        # it off). Replaced as a whole: an output without it removes it.
        rm -rf "$prefix/casefs"
        if [ -d "$output/deploy/casefs" ]; then cp -r "$output/deploy/casefs" "$prefix/casefs"; fi
        icu=
        if [ -n "__ICU__" ]; then
            bash "$output/deploy/icu/build-icu-data.sh" "$output/deploy/icu/culture-profile.json" "$prefix/icu-data"
            icu="ICU_DATA=$prefix/icu-data"
        fi
        chown -R "$user:" "$prefix"

        # The deployment's settings: /etc/$app/environment (APPSETTING_<key>=..., SQLCONNSTR_<name>=...),
        # and files laid over the site from /etc/$app/config.
        mkdir -p "/etc/$app/config"
        if [ ! -f "/etc/$app/environment" ]; then
            cat > "/etc/$app/environment" <<EOF
        # $app's settings (read by the service at start; restart it after a change):
        #   SQLCONNSTR_<name>=<connection string>   replaces web.config's connectionStrings <name>
        #   APPSETTING_<key>=<value>                 sets web.config's appSettings <key>
        EOF
            chmod 600 "/etc/$app/environment"
        fi
        cat > "/etc/$app/service.env" <<EOF
        APP_DIR=$prefix/site
        CONFIG_DIR=/etc/$app/config
        LANG=__LANG__
        ASPNETCORE_URLS=http://0.0.0.0:$port
        $icu
        EOF

        cat > "/etc/systemd/system/$app.service" <<EOF
        [Unit]
        Description=$app (converted by FrameworkOnCore)
        After=network-online.target
        Wants=network-online.target

        [Service]
        User=$user
        WorkingDirectory=$prefix/site
        EnvironmentFile=/etc/$app/service.env
        EnvironmentFile=-/etc/$app/environment
        ExecStart=/bin/bash $prefix/start.sh
        Restart=on-failure
        KillSignal=SIGTERM
        TimeoutStopSec=30

        [Install]
        WantedBy=multi-user.target
        EOF

        if command -v systemctl >/dev/null && [ -d /run/systemd/system ]; then
            systemctl daemon-reload
            systemctl enable "$app" >/dev/null
            if [ -n "$start" ]; then systemctl restart "$app"; echo "started: systemctl status $app, journalctl -u $app"; fi
        else
            echo "systemd is not running here: start it with"
            # As root: /etc/$app/environment is readable by root only (as systemd reads it).
            echo "  sudo bash -c 'set -a; . /etc/$app/service.env; . /etc/$app/environment; exec setpriv --reuid=$user --regid=$user --init-groups bash $prefix/start.sh'"
        fi
        echo "installed $app in $prefix (http://0.0.0.0:$port)"

        """;

    static string Readme(string app, string assembly, string site, string lang, bool container, bool linux, bool icu, bool caseInsensitive, bool gdiplus)
    {
        var text = new StringBuilder($"""
            # {app} の配置(FrameworkOnCore が生成)

            変換したアプリ(`{site}`、`bin/{assembly}.dll`)を Linux で動かす方法。{(container && linux ? "コンテナと Linux のマシン(systemd)の 2 通り。" : "")}

            ## 共通

            - 環境ごとの設定は環境変数で渡す(Azure App Service が .NET Framework のアプリに渡すのと同じ名前。web.config はそのまま)。
              - `SQLCONNSTR_<名前>`: web.config の connectionStrings の `<名前>` を置き換える(`CUSTOMCONNSTR_` なども同じ)。
              - `APPSETTING_<キー>`: appSettings の `<キー>` を設定する(無ければ加える)。
            - それ以外の設定ファイルは、設定フォルダー(`CONFIG_DIR`)に置いたものがサイトに上書きされてから起動する。
            - カルチャ: `LANG={lang}`(元のサーバーの既定のカルチャ)。{(icu ? "元のサーバーのカルチャのデータ(通貨記号、日付の書式)は、動かすマシンで ICU のデータとして作る(`deploy/icu`。ICU の版に合わせるため。github.com への接続が要る)。" : "カルチャのプロファイルが無かったので、ICU のデータは作らない(Linux の既定のカルチャのデータになる)。")}
            - アプリは作業プロセスの中で bin のコピーから動き、web.config や bin が変わると自分で再起動する。
            - URL(`Request.Url`、絶対 URL へのリダイレクト)は、クライアントが送った Host ヘッダーのホスト名とポートから作る。HTTPS を終端するリバースプロキシの後ろでは `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true` を設定し、プロキシが `X-Forwarded-Proto` と元の `Host` を渡すようにする。
            - アプリが書き込む場所: `App_Data`。ほかにも書き込むアプリがある(インストーラーがモジュールを置くなど)ので、サイト全体をアプリのユーザーが書けるようにしてある。
            - ファイル名の大文字小文字: {(caseInsensitive ? "Windows と同じく区別しない(`deploy/casefs` の `libfoccase.so` を `start.sh` がプロセスに読み込む。x64・arm64 の glibc の Linux。読み込めないマシン(Alpine の musl など)では警告を出し、区別するまま動く)。`FOC_CASE_INSENSITIVE=0` で止める。対象はサイトのフォルダー(`FOC_CASE_ROOTS` に `:` 区切りでフォルダーを加えられる)。`FOC_CASE_LOG=1` で、大文字小文字違いで見つけたファイル名を標準エラーに出す(アプリの食い違いの一覧になる)。" : "区別する(変換の `--case-insensitive off`)。アプリが大文字小文字の違う名前でファイルを参照していると、Linux では見つからない。")}
            - System.Drawing(画像・フォント): {(gdiplus ? "Linux では FrameworkOnCore の System.Drawing.Common が libgdiplus で描く。配置が libgdiplus と Liberation のフォント(Arial、Times New Roman、Courier New と同じ文字幅)を入れる。描画の結果(アンチエイリアス、文字の形)は Windows と少し違う。ほかの Windows のフォント(MS ゴシックなど)を使うなら、そのフォントか代わりのフォントを入れる。" : "アプリは System.Drawing を使っていない(libgdiplus は入れない)。")}


            """);
        if (container)
        {
            text.Append($"""
                ## コンテナ

                変換の出力フォルダー(`Dockerfile` のあるところ)で:

                    docker build -t {app} .
                    docker run -d -p 8080:8080 --name {app} \
                      -e SQLCONNSTR_<名前>="Data Source=<サーバー>;Initial Catalog=<DB>;User ID=<ユーザー>;Password=<パスワード>" \
                      -v {app}-data:/app/App_Data \
                      {app}

                - 待ち受けはポート 8080(`ASPNETCORE_HTTP_PORTS`)。ユーザー `app`(root ではない)で動く。
                - `App_Data` はボリューム。設定ファイルを上書きするなら `-v <フォルダー>:/config:ro`。
                - 再起動の方針は実行環境で決める(`docker run --restart unless-stopped`、Kubernetes なら既定で再起動する)。


                """);
        }
        if (linux)
        {
            text.Append($"""
                ## Linux のマシン(systemd)

                変換の出力フォルダーを Linux のマシンにコピーして:

                    sudo deploy/linux/install.sh [--prefix /opt/{app}] [--port 5000]

                - ASP.NET Core 10 のランタイムが無ければ入れる(`/usr/share/dotnet`)。ユーザー `{app}` を作り、`/opt/{app}` に置き、サービス `{app}` として起動する。
                - 設定: `/etc/{app}/environment` に `SQLCONNSTR_<名前>=...` などを書き、`sudo systemctl restart {app}`。設定ファイルの上書きは `/etc/{app}/config`。
                - 更新: 新しい出力で同じコマンドを実行する(`App_Data` は残る)。
                - 前には nginx などのリバースプロキシを置き、HTTPS はそこで終端する。
                - Debian / Ubuntu で確認している(ICU のツールは apt で入れる。ほかのディストリビューションは `icu-devtools` に当たるものを先に入れる)。

                """);
        }
        return text.ToString();
    }
}
