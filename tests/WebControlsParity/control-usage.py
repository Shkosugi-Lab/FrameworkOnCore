"""
The order the component parity takes the controls in (tests/WebControlsParity/README.md): how much the corpora
use each control of System.Web.UI.WebControls / HtmlControls, as markup tags (<asp:Name ...> in .aspx/.ascx/.master/.skin
files of corpora/work) and as code references (the API analysis, experiments/wf4c/_analysis/<corpus>/api-analysis.json).
Sorted by the number of applications using it, then by uses. A proxy for what users rely on most: every control is
covered completely once taken; this decides only which comes first.

    python tests/WebControlsParity/control-usage.py
"""
import json, os, re, collections
root = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
corpora = {'be': 'BlogEngine.NET-3.3.8.0', 'dnn': 'Dnn.Platform-9.13.10', 'yaf': 'YAFNET-3.2.15', 'mojo': 'mojoportal-3.1.6',
           'n2': 'n2cms-master', 'nop': 'nopCommerce-release-1.90', 'imis': 'web_app_vb-main', 'wt': 'wingtiptoys-master'}
markup = collections.Counter()      # tag uses
markup_apps = collections.defaultdict(set)
tag = re.compile(r'<asp:(\w+)[\s>/]', re.I)
for key, folder in corpora.items():
    for dirpath, _, files in os.walk(os.path.join(root, 'corpora', 'work', folder)):
        if any(x in dirpath.lower() for x in ('\\bin', '\\obj', 'node_modules', '\\packages')): continue
        for f in files:
            if f.lower().endswith(('.aspx', '.ascx', '.master', '.skin')):
                try: text = open(os.path.join(dirpath, f), encoding='utf-8', errors='ignore').read()
                except OSError: continue
                for m in tag.finditer(text):
                    name = m.group(1)
                    markup[name.lower()] += 1
                    markup_apps[name.lower()].add(key)
code = collections.Counter()
code_apps = collections.defaultdict(set)
for key in corpora:
    p = os.path.join(root, 'experiments', 'wf4c', '_analysis', key, 'api-analysis.json')
    if not os.path.exists(p): continue
    for api in json.load(open(p, encoding='utf-8'))['apis']:
        m = re.match(r'^[TMPFE]:System\.Web\.UI\.(WebControls|HtmlControls)\.(\w+)', api['id'])
        if m and api.get('count', 0) > 0:
            code[m.group(2).lower()] += api['count']
            code_apps[m.group(2).lower()].add(key)
names = set(markup) | set(code)
rows = []
for n in names:
    apps = markup_apps[n] | code_apps[n]
    rows.append((len(apps), markup[n] + code[n], n, markup[n], code[n]))
rows.sort(reverse=True)
print('apps total markup code name')
for apps, total, n, mk, cd in rows[:70]:
    print(f'{apps:4} {total:6} {mk:6} {cd:6} {n}')
