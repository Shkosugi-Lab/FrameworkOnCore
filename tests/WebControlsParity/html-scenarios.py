"""
The HTML server controls' scenarios (samples/WebControlsProbe/scenarios/Html*.json): one flow on HtmlScenario.aspx (and
HtmlFormScenario.aspx), each control's file with the steps of the mechanisms its type takes part in (Mechanisms.cs: posted
data, change events, postback events, validation, a select's event validation), its snapshots tagged with them.

    python tests/WebControlsParity/html-scenarios.py
"""
import json, os

root = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
out = os.path.join(root, 'samples', 'WebControlsProbe', 'scenarios')

postdata = {'HtmlInputText', 'HtmlInputPassword', 'HtmlInputHidden', 'HtmlInputCheckBox', 'HtmlInputRadioButton', 'HtmlInputGenericControl',
            'HtmlSelect', 'HtmlTextArea', 'HtmlInputFile', 'HtmlInputImage'}
change = postdata - {'HtmlInputFile', 'HtmlInputImage'}
validated = {'HtmlInputText', 'HtmlInputPassword', 'HtmlInputGenericControl', 'HtmlSelect', 'HtmlTextArea', 'HtmlInputFile'}
# The button each clicks for its postback event.
clicks = {'HtmlAnchor': 'ancSubmit', 'HtmlButton': 'btnHtml', 'HtmlInputButton': 'inpButton', 'HtmlInputSubmit': 'inpSubmit', 'HtmlInputImage': 'inpImage'}
on_form_page = {'HtmlForm', 'HtmlHead', 'HtmlTitle'}
names = ['HtmlGenericControl', 'HtmlAnchor', 'HtmlForm', 'HtmlHead', 'HtmlLink', 'HtmlImage', 'HtmlTable', 'HtmlTableRow', 'HtmlTableCell',
         'HtmlInputText', 'HtmlInputPassword', 'HtmlInputHidden', 'HtmlInputCheckBox', 'HtmlInputRadioButton', 'HtmlInputButton', 'HtmlInputSubmit',
         'HtmlInputReset', 'HtmlInputImage', 'HtmlInputFile', 'HtmlInputGenericControl', 'HtmlButton', 'HtmlSelect', 'HtmlTextArea', 'HtmlMeta',
         'HtmlTitle', 'HtmlIframe', 'HtmlArea', 'HtmlAudio', 'HtmlVideo', 'HtmlSource', 'HtmlTrack', 'HtmlEmbed', 'HtmlElement']

# Every posting input filled (the dynamic ones too): what "posted" sends.
fill_all = [
    {'action': 'fill', 'target': 'inpText', 'value': 'posted text'},
    {'action': 'fill', 'target': 'inpPassword', 'value': 'secret'},
    {'action': 'setvalue', 'target': 'inpHidden', 'value': 'changed'},
    {'action': 'check', 'target': 'inpCheck'},
    {'action': 'check', 'target': 'inpRadioLarge'},
    {'action': 'fill', 'target': 'inpEmail', 'value': 'a@example.com'},
    {'action': 'fill', 'target': 'txaNote', 'value': 'posted note'},
    {'action': 'select', 'target': 'selSize', 'value': 'L'},
    {'action': 'selectmany', 'target': 'selMulti', 'value': 'S,L'},
    {'action': 'select', 'target': 'selBound', 'value': 'B2'},
    {'action': 'upload', 'target': 'inpFile', 'value': 'probe.txt:file content'},
    {'action': 'fill', 'target': 'inpDynamicText', 'value': 'dynamic posted'},
    {'action': 'fill', 'target': 'inpDynamicPassword', 'value': 'dynamic secret'},
    {'action': 'setvalue', 'target': 'inpDynamicHidden', 'value': 'dynamic changed'},
    {'action': 'check', 'target': 'inpDynamicCheck'},
    {'action': 'check', 'target': 'inpDynamicRadio'},
    {'action': 'fill', 'target': 'inpDynamicEmail', 'value': 'b@example.com'},
    {'action': 'fill', 'target': 'txaDynamic', 'value': 'dynamic note'},
    {'action': 'select', 'target': 'selDynamic', 'value': '2'},
    {'action': 'upload', 'target': 'inpDynamicFile', 'value': 'dynamic.txt:dynamic content'},
]


def tag(*mechanisms):
    return ''.join('[' + m + ']' for m in mechanisms)


def scenario(name):
    steps = [{'action': 'goto', 'path': '/HtmlScenario.aspx'},
             {'action': 'snapshot', 'name': name + ' ' + tag('render', 'databind', 'dynamic') + ' first request: plain, bound, set in code, made in code'}]
    kept = ['viewstate', 'dynamic'] + (['postdata', 'change'] if name in change else ['postdata'] if name in postdata else []) + (['postbackevent'] if name == 'HtmlInputSubmit' else [])
    steps += fill_all + [{'action': 'click', 'target': 'inpSubmit'},
                         {'action': 'snapshot', 'name': name + ' ' + tag(*kept) + ' every input posted by a submit input'}]
    if name in change:
        steps += [{'action': 'click', 'target': 'btnSubmit'},
                  {'action': 'snapshot', 'name': name + ' ' + tag('change', 'viewstate') + ' posted again unchanged: no change event'}]
    if name in clicks:
        steps += [{'action': 'click', 'target': clicks[name]},
                  {'action': 'snapshot', 'name': name + ' ' + tag('postbackevent') + ' its server click'}]
        dynamic_click = {'HtmlAnchor': 'ancDynamic', 'HtmlButton': 'btnDynamic', 'HtmlInputButton': 'inpDynamicButton', 'HtmlInputSubmit': 'inpDynamicSubmit', 'HtmlInputImage': 'inpDynamicImage'}[name]
        steps += [{'action': 'click', 'target': dynamic_click},
                  {'action': 'snapshot', 'name': name + ' ' + tag('postbackevent', 'dynamic') + ' a dynamic one\'s server click'}]
    if name == 'HtmlInputReset':
        steps += [{'action': 'fill', 'target': 'inpText', 'value': 'typed, then reset'},
                  {'action': 'click', 'target': 'inpReset'},
                  {'action': 'snapshot', 'name': name + ' ' + tag('postbackevent') + ' a reset input resets the form without a request'}]
    steps += [{'action': 'click', 'target': 'btnShow'},
              {'action': 'snapshot', 'name': name + ' ' + tag('visible', 'viewstate') + ' hidden ones shown with what was set while hidden'}]
    if name in validated:
        steps += [{'action': 'fill', 'target': 'inpText', 'value': ''},
                  {'action': 'fill', 'target': 'inpEmail', 'value': ''},
                  {'action': 'fill', 'target': 'txaNote', 'value': ''},
                  {'action': 'select', 'target': 'selSize', 'value': 'M'},
                  {'action': 'click', 'target': 'inpSubmitValidate'},
                  {'action': 'snapshot', 'name': name + ' ' + tag('validation') + ' validated on the server (g1): empty, not chosen'},
                  {'action': 'click', 'target': 'inpSubmitClient'},
                  {'action': 'snapshot', 'name': name + ' ' + tag('clientvalidation') + ' validated in the browser (g2): no request'}]
    if name == 'HtmlSelect':
        steps += [{'action': 'evaluate', 'target': 'selSize', 'value': "const o = document.createElement('option'); o.value = 'X'; o.text = 'X'; e.add(o); e.value = 'X';"},
                  {'action': 'click', 'target': 'btnSubmit'},
                  {'action': 'snapshot', 'name': name + ' ' + tag('eventvalidation') + ' a value not in the list posted: rejected'}]
    if name in on_form_page:
        steps += [{'action': 'goto', 'path': '/HtmlFormScenario.aspx'},
                  {'action': 'snapshot', 'name': name + ' ' + tag('dynamic', 'render') + ' the page\'s form, head and title made in code'},
                  {'action': 'fill', 'target': 'txtInForm', 'value': 'in a dynamic form'},
                  {'action': 'press', 'target': 'txtInForm', 'value': 'Enter'},
                  {'action': 'snapshot', 'name': name + ' ' + tag('dynamic', 'viewstate', 'postdata') + ' Enter in the form: its default button, its values and view state kept'}]
    return {
        '_comment': 'The ' + name + "'s scenario layer (HtmlScenario.aspx; tests/WebControlsParity/README.md; written by html-scenarios.py). A snapshot's tags are the mechanisms it covers.",
        'ignorePatterns': [],
        'steps': steps,
    }


for name in names:
    with open(os.path.join(out, name + '.json'), 'w', encoding='utf-8', newline='\n') as f:
        json.dump(scenario(name), f, indent=2, ensure_ascii=False)
        f.write('\n')
print(len(names), 'scenarios')
