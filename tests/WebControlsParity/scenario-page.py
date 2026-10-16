"""
A probe site page's designer file and its entries in the project (samples/WebControlsProbe), as Visual Studio writes them:
a field for each control of the page's markup with an ID (<asp:X ID="..."> and the HTML server controls, <tag id="..."
runat="server">, by the type the page parser makes of the tag), not those inside a template (a template's controls are
the container's). The page, its code-behind and the designer file are added to WebControlsProbe.csproj when missing.

    python tests/WebControlsParity/scenario-page.py HtmlScenario [OtherPage ... Probe.master]
    python tests/WebControlsParity/scenario-page.py --content App_Data/ads.xml   (files the site serves)
"""
import os, re, sys

root = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
site = os.path.join(root, 'samples', 'WebControlsProbe')
sep = chr(92)

# The HTML server controls the page parser makes of a tag (HtmlControls; the rest: HtmlGenericControl).
html_tags = {
    'form': 'HtmlForm', 'head': 'HtmlHead', 'title': 'HtmlTitle', 'meta': 'HtmlMeta', 'link': 'HtmlLink', 'a': 'HtmlAnchor',
    'img': 'HtmlImage', 'table': 'HtmlTable', 'tr': 'HtmlTableRow', 'td': 'HtmlTableCell', 'th': 'HtmlTableCell',
    'button': 'HtmlButton', 'select': 'HtmlSelect', 'textarea': 'HtmlTextArea', 'iframe': 'HtmlIframe', 'area': 'HtmlArea',
    'audio': 'HtmlAudio', 'video': 'HtmlVideo', 'source': 'HtmlSource', 'track': 'HtmlTrack', 'embed': 'HtmlEmbed', 'html': 'HtmlElement',
}
input_types = {
    'text': 'HtmlInputText', 'password': 'HtmlInputPassword', 'hidden': 'HtmlInputHidden', 'checkbox': 'HtmlInputCheckBox',
    'radio': 'HtmlInputRadioButton', 'button': 'HtmlInputButton', 'submit': 'HtmlInputSubmit', 'reset': 'HtmlInputReset',
    'image': 'HtmlInputImage', 'file': 'HtmlInputFile',
}
# The asp: controls of System.Web.UI (System.Web.Extensions'); the rest are System.Web.UI.WebControls'.
ui_controls = {'ScriptManager', 'ScriptManagerProxy', 'UpdatePanel', 'UpdateProgress', 'Timer'}
# The asp: controls of System.Web.UI.WebControls.WebParts.
web_parts = {'WebPartManager', 'ProxyWebPartManager', 'WebPartZone', 'CatalogZone', 'DeclarativeCatalogPart', 'PageCatalogPart', 'ImportCatalogPart',
             'EditorZone', 'AppearanceEditorPart', 'BehaviorEditorPart', 'LayoutEditorPart', 'PropertyGridEditorPart', 'ConnectionsZone'}


def fields(markup):
    # A template's controls are its container's (not the page's fields), but for a template instantiated once
    # ([TemplateInstance(Single)]: an update panel's ContentTemplate), whose controls are the page's.
    markup = re.sub(r'<%--.*?--%>', '', markup, flags=re.S)
    while True:
        stripped = re.sub(r'<((?!ContentTemplate>)\w*Template)>(?:(?!<\1>).)*?</\1>', '', markup, flags=re.S)
        if stripped == markup:
            break
        markup = stripped
    found = []
    for m in re.finditer(r'<(asp:)?(\w+)\b([^>]*?)/?>', markup, flags=re.S):
        prefix, tag, attributes = m.group(1), m.group(2), m.group(3)
        if prefix:
            ident = re.search(r'\bID="(\w+)"', attributes)
            # A content page's Content controls are no fields (their controls are).
            if not ident or tag == 'Content':
                continue
            namespace = 'System.Web.UI' if tag in ui_controls else 'System.Web.UI.WebControls.WebParts' if tag in web_parts else 'System.Web.UI.WebControls'
            found.append((namespace + '.' + tag, ident.group(1)))
            continue
        if not re.search(r'\brunat="server"', attributes, flags=re.I):
            continue
        ident = re.search(r'\bid="(\w+)"', attributes, flags=re.I)
        if not ident:
            continue
        lower = tag.lower()
        if lower == 'input':
            kind = re.search(r'\btype="(\w+)"', attributes, flags=re.I)
            kind = kind.group(1).lower() if kind else 'text'
            name = input_types.get(kind, 'HtmlInputGenericControl')
        else:
            name = html_tags.get(lower, 'HtmlGenericControl')
        found.append(('System.Web.UI.HtmlControls.' + name, ident.group(1)))
    return found


def write(page):
    # A page (Name: Name.aspx) or a master page (Name.master).
    page = page if page.endswith('.master') else page + '.aspx'
    path = os.path.join(site, page)
    markup = open(path, encoding='utf-8-sig').read()
    names = fields(markup)
    lowered = [n.lower() for _, n in names]
    duplicates = sorted({n for n in lowered if lowered.count(n) > 1})
    if duplicates:
        sys.exit(page + ': IDs named twice (without regard to case): ' + ', '.join(duplicates))
    klass = re.search(r'Inherits="[\w.]*?(\w+)"', markup).group(1)
    lines = ['        protected global::%s %s;' % (t, n) for t, n in names]
    # <%@ MasterType %>: the page's Master, typed (its master's class).
    master = re.search(r'<%@\s*MasterType\s+VirtualPath="~/([^"]+)"', markup)
    if master:
        master_markup = open(os.path.join(site, master.group(1)), encoding='utf-8-sig').read()
        master_class = re.search(r'Inherits="([\w.]+)"', master_markup).group(1)
        lines.append('\n        public new global::%s Master\n        {\n            get\n            {\n                return ((global::%s)(base.Master));\n            }\n        }' % (master_class, master_class))
    source = ('//------------------------------------------------------------------------------\n'
              '// <auto-generated>\n'
              '//     This code was generated by a tool.\n'
              '// </auto-generated>\n'
              '//------------------------------------------------------------------------------\n'
              '\n'
              'namespace WebControlsProbe\n'
              '{\n'
              '    public partial class %s\n'
              '    {\n'
              '%s\n'
              '    }\n'
              '}\n') % (klass, '\n'.join(lines))
    open(os.path.join(site, page + '.designer.cs'), 'w', encoding='utf-8-sig', newline='\r\n').write(source)
    for f in (page, page + '.cs'):
        f = os.path.join(site, f)
        if os.path.exists(f):
            b = open(f, 'rb').read()
            if not b.startswith(b'\xef\xbb\xbf'):
                open(f, 'wb').write(b'\xef\xbb\xbf' + b)
    project = os.path.join(site, 'WebControlsProbe.csproj')
    text = open(project, encoding='utf-8-sig').read()
    crlf = '\r\n' in text
    text = text.replace('\r\n', '\n')
    if '<Content Include="%s" />' % page not in text:
        anchor = '    <Content Include="Web.config" />'
        text = text.replace(anchor, '    <Content Include="%s" />\n' % page + anchor, 1)
    if '<Compile Include="%s.cs">' % page not in text:
        anchor = '    <Compile Include="ProbeStore.cs" />'
        text = text.replace(anchor, ('    <Compile Include="{0}.cs">\n      <DependentUpon>{0}</DependentUpon>\n      <SubType>ASPXCodeBehind</SubType>\n    </Compile>\n'
                                     '    <Compile Include="{0}.designer.cs">\n      <DependentUpon>{0}</DependentUpon>\n    </Compile>\n').format(page) + anchor, 1)
    if crlf:
        text = text.replace('\n', '\r\n')
    open(project, 'w', encoding='utf-8-sig', newline='').write(text)
    print(page + ': %d fields' % len(names))


def content(*paths):
    """Files the site serves (images, style sheets, skins), as Content of the project."""
    project = os.path.join(site, 'WebControlsProbe.csproj')
    text = open(project, encoding='utf-8-sig').read()
    crlf = '\r\n' in text
    text = text.replace('\r\n', '\n')
    for p in paths:
        entry = '    <Content Include="%s" />' % p.replace('/', sep)
        if entry not in text:
            anchor = '    <Content Include="Web.config" />'
            text = text.replace(anchor, entry + '\n' + anchor, 1)
    if crlf:
        text = text.replace('\n', '\r\n')
    open(project, 'w', encoding='utf-8-sig', newline='').write(text)


if __name__ == '__main__':
    args = sys.argv[1:]
    if args and args[0] == '--content':
        content(*args[1:])
    else:
        for page in args:
            write(page)
