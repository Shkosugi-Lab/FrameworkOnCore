namespace WebForm2Blazor.Converter.Emit;

public sealed class ScaffoldOptions
{
    public required string ProjectName { get; init; }

    /// <summary>Relative path from the output csproj to the compatibility-components csproj.</summary>
    public required string ComponentsProjectReference { get; init; }

    /// <summary>The default layout as a fully qualified type name (used in typeof()).</summary>
    public required string DefaultLayoutComponent { get; init; }

    /// <summary>Whether to generate MainLayout.razor (a pass-through layout).</summary>
    public required bool EmitFallbackLayout { get; init; }

    public string SiteTitle { get; init; } = "";

    /// <summary>The port dotnet run listens on.</summary>
    public int Port { get; init; } = 5080;
}

/// <summary>Generates the skeleton of the target Blazor Server (.NET 10, Interactive Server) project.</summary>
public sealed class BlazorScaffolder(ScaffoldOptions options)
{
    public void Scaffold(string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);
        Directory.CreateDirectory(Path.Combine(outputDirectory, "Properties"));
        Directory.CreateDirectory(Path.Combine(outputDirectory, "Components", "Layout"));
        Directory.CreateDirectory(Path.Combine(outputDirectory, "Components", "Pages"));
        Directory.CreateDirectory(Path.Combine(outputDirectory, "Components", "Controls"));
        Directory.CreateDirectory(Path.Combine(outputDirectory, "wwwroot"));

        Write(outputDirectory, $"{options.ProjectName}.csproj", Csproj);
        Write(outputDirectory, "Program.cs", ProgramCs);
        Write(outputDirectory, Path.Combine("Properties", "launchSettings.json"), LaunchSettingsJson);
        Write(outputDirectory, Path.Combine("Components", "App.razor"), AppRazor);
        Write(outputDirectory, Path.Combine("Components", "Routes.razor"), RoutesRazor);
        Write(outputDirectory, Path.Combine("Components", "_Imports.razor"), BuildImportsRazor());

        if (options.EmitFallbackLayout)
        {
            Write(outputDirectory, Path.Combine("Components", "Layout", "MainLayout.razor"), MainLayoutRazor);
        }
    }

    private void Write(string outputDirectory, string relativePath, string template)
    {
        var content = template
            .Replace("{{NAME}}", options.ProjectName)
            .Replace("{{COMPONENTS_REF}}", options.ComponentsProjectReference)
            .Replace("{{LAYOUT}}", options.DefaultLayoutComponent)
            .Replace("{{PORT}}", options.Port.ToString())
            .Replace("{{TITLE}}", string.IsNullOrWhiteSpace(options.SiteTitle) ? options.ProjectName : options.SiteTitle);

        File.WriteAllText(Path.Combine(outputDirectory, relativePath), content);
    }

    private const string Csproj = """
        <Project Sdk="Microsoft.NET.Sdk.Web">

          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
            <!-- Match the source (.NET Framework) semantics: nullable is disabled in converted code -->
            <Nullable>disable</Nullable>
            <!-- Ported Framework code was written without implicit usings, and the ones
                 the SDK adds collide with types the app defines itself: System.IO.File /
                 Directory against an app's own FileSystem.File / Directory, and
                 Microsoft.Extensions.Configuration.ConfigurationManager against
                 System.Configuration.ConfigurationManager. Razor keeps its own imports
                 through _Imports.razor, so only Program.cs needs explicit usings. -->
            <ImplicitUsings>disable</ImplicitUsings>
            <RootNamespace>{{NAME}}</RootNamespace>
            <!-- BL0005: the WebForms compatibility components are designed to have their
                 properties written directly from code-behind (lblResult.Text = ... etc.) -->
            <!-- BL0001: converted code-behind exposes ported properties as [Parameter];
                 some have custom getters/setters the analyzer would reject -->
            <!-- SYSLIB0011: BinaryFormatter is obsolete-as-error and removed from the
                 runtime. Ported code that uses it is left compiling on purpose: the call
                 throws PlatformNotSupportedException the moment it runs, which points at
                 the exact line to migrate. Blocking the whole build instead would hide
                 every other problem behind one line, and a residual already records it. -->
            <NoWarn>$(NoWarn);BL0005;BL0001;SYSLIB0011</NoWarn>
          </PropertyGroup>

          <ItemGroup>
            <ProjectReference Include="{{COMPONENTS_REF}}" />
          </ItemGroup>

          <!-- Ported code-behind uses the System.Web compatibility surface (HttpContext,
               QueryString etc.); the implicit ASP.NET Core usings would make those names
               ambiguous, so they are removed for the whole project -->
          <!-- The AI residual layer keeps candidate .razor answers here; they are inputs
               to that workflow, not part of the app -->
          <ItemGroup>
            <Content Remove="ai-layer\**" />
            <None Remove="ai-layer\**" />
            <Compile Remove="ai-layer\**" />
          </ItemGroup>

          <ItemGroup>
            <Using Remove="Microsoft.AspNetCore.Http" />
            <Using Remove="Microsoft.AspNetCore.Routing" />
            <Using Remove="Microsoft.Extensions.Hosting" />
          </ItemGroup>

        </Project>
        """;

    private const string ProgramCs = """
        using Microsoft.AspNetCore.Builder;
        using Microsoft.AspNetCore.Hosting;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Hosting;
        using WebForm2Blazor.Components;

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // WebForms compatibility runtime (Session / Application / ConfigurationManager)
        builder.Services.AddWebFormsCompat(builder.Configuration);

        var app = builder.Build();

        // WebForms-compatible session (kept across page navigations via a session ID cookie)
        app.UseWebFormsSession();

        app.UseAntiforgery();

        // Serves framework static assets such as blazor.web.js.
        // UseStaticFiles 404s when the app is not published, so MapStaticAssets is used.
        app.MapStaticAssets();

        app.MapRazorComponents<{{NAME}}.Components.App>()
            .AddInteractiveServerRenderMode();

        app.Run();
        """;

    private const string AppRazor = """
        <!DOCTYPE html>
        <html lang="ja">

        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <base href="/" />
            <title>{{TITLE}}</title>
            <HeadOutlet @rendermode="InteractiveServer" />
        </head>

        <body>
            <Routes @rendermode="InteractiveServer" />
            <script src="_framework/blazor.web.js"></script>
        </body>

        </html>
        """;

    private const string RoutesRazor = """
        <Router AppAssembly="typeof(Routes).Assembly">
            <Found Context="routeData">
                <RouteView RouteData="routeData" DefaultLayout="typeof({{LAYOUT}})" />
            </Found>
        </Router>
        """;

    private string BuildImportsRazor()
    {
        // Page/control namespaces are deliberately NOT imported globally: folder-preserving
        // output allows equally named components in different folders, and a global import
        // would make their tags ambiguous. Each .razor gets per-file @using lines instead.
        var namespaces = new List<string>
        {
            $"{options.ProjectName}.Components",
        };
        if (options.EmitFallbackLayout)
        {
            namespaces.Add($"{options.ProjectName}.Components.Layout");
        }

        var usings = string.Join(Environment.NewLine,
            namespaces.Distinct().Order(StringComparer.Ordinal).Select(name => $"@using {name}"));

        return $"""
            @using System.Net.Http
            @* Microsoft.AspNetCore.Components.Forms is deliberately not imported.
               It contains types with the same names as the WebForms compatibility
               components (e.g. ValidationSummary), causing tag-name collisions (RZ9985). *@
            @using Microsoft.AspNetCore.Components.Routing
            @using Microsoft.AspNetCore.Components.Sections
            @using Microsoft.AspNetCore.Components.Web
            @using static Microsoft.AspNetCore.Components.Web.RenderMode
            {usings}
            @using WebForm2Blazor.Components
            """;
    }

    private const string MainLayoutRazor = """
        @inherits LayoutComponentBase

        <main>
            @Body
        </main>
        """;

    private const string LaunchSettingsJson = """
        {
          "profiles": {
            "http": {
              "commandName": "Project",
              "dotnetRunMessages": true,
              "launchBrowser": true,
              "applicationUrl": "http://localhost:{{PORT}}",
              "environmentVariables": {
                "ASPNETCORE_ENVIRONMENT": "Development"
              }
            }
          }
        }
        """;
}
