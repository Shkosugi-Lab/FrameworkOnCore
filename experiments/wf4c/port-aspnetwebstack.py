# Imports AspNetWebStack's projects (MVC 5, Web API 2, Web Pages and Razor 3; github.com/aspnet/AspNetWebStack, Apache
# 2.0) into FrameworkOnCore.Runtime/src as SDK projects for .NET 10, as the other ports there: the assembly's name and
# version as Microsoft's (5.3.0.0, 3.0.0.0, 6.0.0.0), Microsoft's public key token (FakeStrongName), FrameworkOnCore's
# System.Web and the other ports referenced in place of .NET Framework's.
#
#   git clone --depth 1 --branch v3.3.0 https://github.com/aspnet/AspNetWebStack.git experiments/wf4c/_aspnetwebstack
#   python experiments/wf4c/port-aspnetwebstack.py [--sources] System.Web.Razor ...
#
# The sources are copied once (--sources; a project folder that has them is left: it holds FrameworkOnCore's changes,
# marked "FrameworkOnCore:"); the project file is written every time. Each project's compile items as upstream's
# project lists them (the shared ..\Common files copied under Shared\), its resources by the names its designer
# classes ask for.
import glob, os, re, shutil, sys
import xml.etree.ElementTree as ET

HERE = os.path.dirname(os.path.abspath(__file__))
REPO = os.path.dirname(os.path.dirname(HERE))
UPSTREAM = os.path.join(HERE, '_aspnetwebstack', 'src')
RUNTIME = os.path.join(REPO, 'FrameworkOnCore.Runtime', 'src')
MS = '{http://schemas.microsoft.com/developer/msbuild/2003}'

# upstream project -> (FrameworkOnCore's folder, package)
PROJECTS = {
    'System.Web.Razor': ('WebFormsForCore.Web.Razor', 'FrameworkOnCore.Web.Razor'),
    'System.Web.WebPages.Deployment': ('WebFormsForCore.Web.WebPages.Deployment', 'FrameworkOnCore.Web.WebPages'),
    'System.Web.WebPages': ('WebFormsForCore.Web.WebPages', 'FrameworkOnCore.Web.WebPages'),
    'System.Web.WebPages.Razor': ('WebFormsForCore.Web.WebPages.Razor', 'FrameworkOnCore.Web.WebPages'),
    'System.Web.Helpers': ('WebFormsForCore.Web.Helpers', 'FrameworkOnCore.Web.WebPages'),
    'System.Web.Mvc': ('WebFormsForCore.Web.Mvc', 'FrameworkOnCore.Web.Mvc'),
    'System.Net.Http.Formatting': ('WebFormsForCore.Net.Http.Formatting', 'FrameworkOnCore.WebApi.Client'),
    'System.Web.Http': ('WebFormsForCore.Web.Http', 'FrameworkOnCore.WebApi'),
    'System.Web.Http.WebHost': ('WebFormsForCore.Web.Http.WebHost', 'FrameworkOnCore.WebApi'),
    'System.Web.Http.Owin': ('WebFormsForCore.Web.Http.Owin', 'FrameworkOnCore.WebApi.Owin'),
    'System.Web.Cors': ('WebFormsForCore.Web.Cors', 'FrameworkOnCore.Web.Cors'),
    'System.Web.Http.Cors': ('WebFormsForCore.Web.Http.Cors', 'FrameworkOnCore.WebApi.Cors'),
}
# .NET Framework's references -> FrameworkOnCore's projects (None: .NET has it)
REFERENCES = {
    'System.Web': 'WebFormsForCore.Web', 'System.Web.Abstractions': None, 'System.Web.Routing': None,
    'System.Web.Extensions': 'WebFormsForCore.Web.Extensions', 'System.Web.DataVisualization': 'WebFormsForCore.Web.DataVisualization',
    'System.Configuration': 'WebFormsForCore.Configuration', 'System.Data.Linq': 'WebFormsForCore.Data.Linq',
    'System.Drawing': 'WebFormsForCore.Drawing', 'Microsoft.Web.Infrastructure': 'WebFormsForCore.Web.Infrastructure',
}
PACKAGES = {'System.Runtime.Caching': ('System.Runtime.Caching', '10.0.0'), 'Newtonsoft.Json': ('Newtonsoft.Json', '13.0.4'),
            'Newtonsoft.Json.Bson': ('Newtonsoft.Json.Bson', '1.0.2'), 'Microsoft.Owin': ('Microsoft.Owin', '4.2.2'), 'Owin': ('Owin', '1.0.0')}
# FrameworkOnCore's own references: System.Web's FrameworkNumbers (doubles written and read as .NET Framework did) for
# Web API's core, which upstream keeps apart from System.Web (a converted application always has it).
EXTRA_REFERENCES = {'System.Web.Http': ['WebFormsForCore.Web'], 'System.Net.Http.Formatting': ['WebFormsForCore.Web']}
# What .NET Framework had in System.dll and .NET has in packages: every project gets them.
ALWAYS = [('System.CodeDom', '10.0.0')]
# Microsoft's key whose token is 31bf3856ad364e35 (as System.ServiceModel's port has it).
PUBLIC_KEY = '0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9'


def items(project, kind, src_dir=None):
    """The project's items of a kind; a wildcard include (System.Net.Http.Formatting's **\\*.cs) expanded in src_dir,
    without its Exclude."""
    out = []
    for e in project.iter(MS + kind):
        include = e.get('Include')
        if src_dir and '*' in include:
            excluded = set()
            for pattern in (e.get('Exclude') or '').split(';'):
                if pattern:
                    excluded |= {os.path.normcase(os.path.relpath(f, src_dir)) for f in glob.glob(os.path.join(src_dir, pattern.replace('\\', os.sep)), recursive=True)}
            for f in sorted(glob.glob(os.path.join(src_dir, include.replace('\\', os.sep)), recursive=True)):
                rel = os.path.relpath(f, src_dir)
                if os.path.normcase(rel) not in excluded:
                    out.append((rel.replace(os.sep, '\\'), e))
        else:
            out.append((include, e))
    return out


def target_of(include):
    """Where an upstream compile item goes in the port: its path, the shared files (..\\X) under Shared\\."""
    include = include.replace('\\', '/')
    return 'Shared/' + include[3:] if include.startswith('../') else include


def resource_name(src_dir, resx, compiles, root, link):
    """The manifest name upstream's resources have: the base name its designer class gives ResourceManager."""
    stem = os.path.splitext(os.path.normpath(os.path.join(src_dir, resx)))[0]
    for c in compiles:
        path = os.path.normpath(os.path.join(src_dir, c))
        if path.lower() == (stem + '.designer.cs').lower():
            m = re.search(r'ResourceManager\("([^"]+)"', open(path, encoding='utf-8-sig').read())
            if m:
                return m.group(1) + '.resources'
            # CommonResources finds itself by the end of its name: MSBuild's name for it (the root namespace and its link).
            return root + '.' + link.replace('\\', '.').replace('/', '.')[:-len('.resx')] + '.resources'
    raise SystemExit(f'no designer class for {resx}')


def port(name, sources):
    folder, package = PROJECTS[name]
    src_dir = os.path.join(UPSTREAM, name)
    dest = os.path.join(RUNTIME, folder)
    project = ET.parse(os.path.join(src_dir, name + '.csproj')).getroot()
    compiles = list(dict.fromkeys(i for i, _ in items(project, 'Compile', src_dir)))
    resources = [i for i, _ in items(project, 'EmbeddedResource')]
    if sources and not os.path.exists(dest):
        for c in compiles + resources:
            to = os.path.join(dest, target_of(c))
            os.makedirs(os.path.dirname(to), exist_ok=True)
            shutil.copy2(os.path.join(src_dir, c), to)
        shutil.copy2(os.path.join(UPSTREAM, '..', 'LICENSE.txt'), os.path.join(dest, 'LICENSE-AspNetWebStack.txt'))
    root = project.findtext(f'{MS}PropertyGroup/{MS}RootNamespace') or name
    defines = ';'.join(d for e in project.iter(MS + 'DefineConstants') for d in (e.text or '').split(';') if d and d != '$(DefineConstants)')
    references, packages = set(), set(ALWAYS)
    for include, _ in items(project, 'Reference'):
        ref = include.split(',')[0]
        if ref in REFERENCES and REFERENCES[ref]:
            references.add(REFERENCES[ref])
        elif ref in PACKAGES:
            packages.add(PACKAGES[ref])
    references.update(EXTRA_REFERENCES.get(name, []))
    for include, _ in items(project, 'ProjectReference'):
        references.add(PROJECTS[os.path.splitext(os.path.basename(include))[0]][0])
    lines = [
        '<Project Sdk="Microsoft.NET.Sdk">',
        f'    <!-- {name} of AspNetWebStack v3.3.0 (github.com/aspnet/AspNetWebStack, Apache 2.0: LICENSE-AspNetWebStack.txt), ported',
        '         to .NET 10 on FrameworkOnCore\'s System.Web (experiments/wf4c/port-aspnetwebstack.py writes this file). -->',
        '    <PropertyGroup>',
        '        <TargetFramework>net10.0</TargetFramework>',
        '        <ImplicitUsings>disable</ImplicitUsings>',
        '        <Nullable>disable</Nullable>',
        '        <LangVersion>13</LangVersion>',
        '        <GenerateAssemblyInfo>false</GenerateAssemblyInfo>',
        '        <EnableDefaultEmbeddedResourceItems>false</EnableDefaultEmbeddedResourceItems>',
        '        <TreatWarningsAsErrors>false</TreatWarningsAsErrors>',
        '        <NoWarn>$(NoWarn);1591;CS0618;CS0672;SYSLIB0003;SYSLIB0011;SYSLIB0012;SYSLIB0050;SYSLIB0051</NoWarn>',
        f'        <DefineConstants>$(DefineConstants);{defines};WebFormsForCore</DefineConstants>',
        f'        <RootNamespace>{root}</RootNamespace>',
        f'        <AssemblyName>{name}</AssemblyName>',
        '        <SignAssembly>True</SignAssembly>',
        '        <AssemblyOriginatorKeyFile>..\\SigningKey\\WebFormsForCore.snk</AssemblyOriginatorKeyFile>',
        f'        <PackageId>{folder.replace("WebFormsForCore.", "FrameworkOnCore.")}</PackageId>',
        f'        <Description>{name} (AspNetWebStack v3.3.0) on .NET 10, for FrameworkOnCore</Description>',
        '        <PackageLicenseExpression>Apache-2.0</PackageLicenseExpression>',
        '        <PackageProjectUrl>https://github.com/aspnet/AspNetWebStack</PackageProjectUrl>',
        '    </PropertyGroup>',
        '    <ItemGroup>',
    ]
    for r in resources:
        link = dict(items(project, 'EmbeddedResource'))[r].findtext(MS + 'Link') or r
        lines.append(f'        <EmbeddedResource Include="{target_of(r)}" LogicalName="{resource_name(src_dir, r, compiles, root, link)}" />')
    lines.append('        <None Include="LICENSE-AspNetWebStack.txt" Pack="true" PackagePath="\\" />')
    lines.append('    </ItemGroup>')
    lines.append('    <ItemGroup>')
    for r in sorted(references):
        lines.append(f'        <ProjectReference Include="..\\{r}\\{r}.csproj" />')
    for p, v in sorted(packages):
        lines.append(f'        <PackageReference Include="{p}" Version="{v}" />')
    lines += [
        '    </ItemGroup>',
        '    <Import Project="..\\..\\lib\\WebFormsForCore.Build\\netstandard2.0\\FakeStrongName.targets" />',
        '    <PropertyGroup>',
        f'        <StrongNamePublicKey>{PUBLIC_KEY}</StrongNamePublicKey>',
        '        <StrongNamePublicKeyToken>31bf3856ad364e35</StrongNamePublicKeyToken>',
        '    </PropertyGroup>',
        '    <Target Name="ChangeAliasesOfNugetRefs" BeforeTargets="FindReferenceAssembliesForReferences;ResolveReferences">',
        '        <ItemGroup>',
        '            <!-- .NET\'s System.Web (HttpUtility\'s type forwards): FrameworkOnCore\'s is the one -->',
        '            <ReferencePath Remove="%(Identity)" Condition="\'%(FileName)\' == \'System.Web\' AND $([System.Text.RegularExpressions.Regex]::IsMatch(%(Identity),\'\\\\dotnet\\\\\'))" />',
        '            <ReferencePath Remove="%(Identity)" Condition="\'%(FileName)\' == \'System.Drawing\' AND $([System.Text.RegularExpressions.Regex]::IsMatch(%(Identity),\'\\\\dotnet\\\\\'))" />',
        '            <!-- .NET\'s System.Configuration.ConfigurationManager (a package other references bring): FrameworkOnCore\'s port is the one -->',
        '            <ReferencePath Remove="%(Identity)" Condition="\'%(FileName)\' == \'System.Configuration.ConfigurationManager\' AND $([System.Text.RegularExpressions.Regex]::IsMatch(%(Identity),\'(?i)\\\\system\\.configuration\\.configurationmanager\\\\\'))" />',
        '        </ItemGroup>',
        '    </Target>',
        '    <Import Project="..\\FrameworkOnCore.Translations.targets" />',
        '</Project>',
    ]
    os.makedirs(dest, exist_ok=True)
    with open(os.path.join(dest, folder + '.csproj'), 'w', encoding='utf-8', newline='\n') as f:
        f.write('\n'.join(lines) + '\n')
    print(f'{name} -> {folder}: {len(compiles)} sources, {len(resources)} resources, references {sorted(references)} {sorted(packages)}')


if __name__ == '__main__':
    args = sys.argv[1:]
    sources = '--sources' in args
    for name in [a for a in args if not a.startswith('--')] or list(PROJECTS):
        port(name, sources)
