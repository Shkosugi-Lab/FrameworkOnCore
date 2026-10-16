"""
The scenarios of the controls on MoreScenario.aspx (the table parts, FileUpload, MultiView, View, Localize, Substitution,
Xml, AdRotator, Calendar, ImageMap, LiteralControl), ContentScenario.aspx (Content, ContentPlaceHolder) and AjaxScenario.aspx
(ScriptManager, ScriptManagerProxy, UpdatePanel, UpdateProgress, Timer), NavigationScenario.aspx (TreeView, Menu, SiteMapPath,
SiteMapDataSource), LoginScenario.aspx (the login controls, the wizards and their steps) and DataSourceScenario.aspx (the data
source controls, QueryExtender, ModelErrorMessage) and WebPartsScenario.aspx (the web parts): one flow per
page, each control's file with its snapshots tagged with the mechanisms they cover for it (Mechanisms.cs: a postback
event for the types that handle one: Table, Calendar, ImageMap, Timer, ScriptManager's Navigate).

    python tests/WebControlsParity/more-scenarios.py
"""
import json, os

root = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
out = os.path.join(root, 'samples', 'WebControlsProbe', 'scenarios')

more = ['Table', 'TableRow', 'TableHeaderRow', 'TableFooterRow', 'TableCell', 'TableHeaderCell', 'FileUpload', 'MultiView', 'View', 'Localize',
        'Substitution', 'Xml', 'AdRotator', 'Calendar', 'ImageMap', 'LiteralControl']
content = ['Content', 'ContentPlaceHolder']


def tag(*mechanisms):
    return ''.join('[' + m + ']' for m in mechanisms)


def area_click(target):
    # An image map's hot spot: its area (in the map after the image) clicked by script: an area has no box to click.
    return [{'action': 'evaluate', 'target': target, 'value': "document.getElementById('ImageMap' + e.id).querySelector('area').click();"},
            {'action': 'waitms', 'ms': 3000}]


def more_scenario(name):
    def snap(text, *mechanisms, only=None):
        own = [m for m in mechanisms if only is None or name in only] or ['viewstate']
        return {'action': 'snapshot', 'name': name + ' ' + tag(*dict.fromkeys(own)) + ' ' + text}
    steps = [{'action': 'goto', 'path': '/MoreScenario.aspx'},
             snap('first request: plain, bound, set in code, made in code', 'render', 'databind', 'dynamic'),
             {'action': 'upload', 'target': 'fupPlain', 'value': 'probe.txt:file content'},
             {'action': 'upload', 'target': 'fupMulti', 'value': 'multi.txt:multi content'},
             {'action': 'upload', 'target': 'fupDynamic', 'value': 'dynamic.txt:dynamic content'},
             {'action': 'fill', 'target': 'txtInFirst', 'value': 'in the first view'},
             {'action': 'click', 'target': 'btnSubmit'},
             snap('posted: files, view state, what code set (a table\'s rows and a literal control\'s text are not kept)', 'viewstate', 'dynamic'),
             {'action': 'click', 'target': 'btnNext'},
             snap('the next view (NextView bubbled from a button)', 'viewstate'),
             {'action': 'click', 'target': 'btnToThird'},
             {'action': 'click', 'target': 'btnToFirst'},
             snap('switched by ID and by index: the first view\'s text box kept its value', 'viewstate'),
             {'action': 'click', 'target': 'btnDynamicOne'},
             snap('a dynamic multi-view\'s next view', 'dynamic', 'viewstate'),
             {'action': 'clicktextin', 'target': 'calPlain', 'value': '20'},
             snap('a calendar\'s day chosen', 'postbackevent', only={'Calendar'}),
             {'action': 'clicktextin', 'target': 'calPlain', 'value': 'Week'},
             snap('a calendar\'s week chosen (several dates)', 'postbackevent', 'multiselect', only={'Calendar'}),
             {'action': 'clicktextin', 'target': 'calPlain', 'value': '>'},
             snap('a calendar\'s next month', 'postbackevent', only={'Calendar'}),
             {'action': 'clicktextin', 'target': 'calDynamic', 'value': '21'},
             snap('a dynamic calendar\'s day chosen', 'postbackevent', 'dynamic', only={'Calendar'}),
             ] + area_click('imapPlain') + [
             snap('an image map\'s hot spot clicked (PostBack)', 'postbackevent', only={'ImageMap'}),
             ] + area_click('imapDynamic') + [
             snap('a dynamic image map\'s hot spot clicked', 'postbackevent', 'dynamic', only={'ImageMap'}),
             {'action': 'click', 'target': 'btnShow'},
             snap('hidden ones shown with what was set while hidden', 'visible', 'viewstate'),
             {'action': 'click', 'target': 'btnUploadValidate'},
             snap('a file upload validated on the server (g1): no file posted', 'viewstate'),
             {'action': 'evaluate', 'target': 'tblPlain', 'value': "__doPostBack(e.id, 'probe');"},
             {'action': 'waitms', 'ms': 3000},
             snap('a postback event to a table (not registered for event validation): rejected', 'postbackevent', only={'Table'}),
             ]
    return steps


def content_scenario(name):
    def snap(text, *mechanisms):
        return {'action': 'snapshot', 'name': name + ' ' + tag(*mechanisms) + ' ' + text}
    return [{'action': 'goto', 'path': '/ContentScenario.aspx'},
            snap('first request: the page\'s content in the master\'s place holders, a default, a hidden one, one filled in code', 'render', 'databind', 'dynamic', 'naming'),
            {'action': 'fill', 'target': 'txtName', 'value': 'in the content'},
            {'action': 'fill', 'target': 'txtDynamic', 'value': 'in a place holder filled in code'},
            {'action': 'click', 'target': 'btnSubmit'},
            snap('posted: the content\'s and the dynamic text boxes, view state', 'viewstate', 'dynamic'),
            {'action': 'click', 'target': 'btnShow'},
            snap('hidden ones shown (the content\'s panel, the master\'s place holder)', 'visible', 'viewstate'),
            {'action': 'click', 'target': 'btnMaster'},
            snap('the master\'s own button', 'viewstate')]


ajax = ['ScriptManager', 'ScriptManagerProxy', 'UpdatePanel', 'UpdateProgress', 'Timer']


def ajax_scenario(name):
    def snap(text, *mechanisms, only=None):
        own = [m for m in mechanisms if only is None or name in only] or ['viewstate']
        return {'action': 'snapshot', 'name': name + ' ' + tag(*dict.fromkeys(own)) + ' ' + text}
    templated = {'UpdatePanel', 'UpdateProgress'}
    return [{'action': 'goto', 'path': '/AjaxScenario.aspx'},
            snap('first request: the panels, the proxy\'s script run, a panel made in code', 'render', 'databind', 'dynamic', *(['templates'] if name in templated else [])),
            {'action': 'fill', 'target': 'txtInPanel', 'value': 'in the panel'},
            {'action': 'click', 'target': 'btnInPanel'},
            snap('an asynchronous postback of the main panel: the conditional panel outside left as it was', 'viewstate', 'postdata', 'templates', only={'ScriptManager', 'UpdatePanel'}),
            {'action': 'click', 'target': 'btnOutside'},
            snap('a button outside triggers the second panel', 'viewstate', 'postdata', only={'ScriptManager', 'UpdatePanel'}),
            {'action': 'click', 'target': 'btnFull'},
            snap('a postback trigger in the panel: a full postback', 'viewstate', 'postdata', only={'ScriptManager', 'UpdatePanel'}),
            {'action': 'evaluate', 'target': 'btnSlow', 'value': 'e.click();'},
            snap('while a slow request runs: the progress shown', 'templates', 'render', only={'UpdateProgress'}),
            {'action': 'waitms', 'ms': 4000},
            snap('after the slow request: the progress hidden', 'templates', 'viewstate', only={'UpdateProgress'}),
            {'action': 'click', 'target': 'btnStartTimer'},
            {'action': 'waitms', 'ms': 4000},
            snap('the timer started: one tick, then stopped', 'postbackevent', only={'Timer'}),
            {'action': 'click', 'target': 'btnHistory'},
            {'action': 'evaluate', 'target': 'btnInPanel', 'value': 'history.back();'},
            {'action': 'waitms', 'ms': 4000},
            snap('a history point, then the browser\'s back: the script manager\'s Navigate', 'postbackevent', only={'ScriptManager'}),
            {'action': 'click', 'target': 'btnDynamic'},
            snap('an asynchronous postback of the panel made in code', 'dynamic', 'viewstate'),
            {'action': 'click', 'target': 'btnShow'},
            snap('a hidden panel shown with what was set while hidden', 'visible', 'viewstate')]


navigation = ['TreeView', 'Menu', 'SiteMapPath', 'SiteMapDataSource']


def click_link(target, argument):
    # A tree's link by its postback argument (an expand link is an image: no text to find it by).
    return [{'action': 'evaluate', 'target': target, 'value': "Array.from(e.querySelectorAll('a')).find(a => a.href.indexOf(\"'" + argument + "'\") >= 0).click();"},
            {'action': 'waitms', 'ms': 3000}]


def navigation_scenario(name):
    def snap(text, *mechanisms, only=None):
        own = [m for m in mechanisms if only is None or name in only] or ['viewstate']
        return {'action': 'snapshot', 'name': name + ' ' + tag(*dict.fromkeys(own)) + ' ' + text}
    templated = {'Menu', 'SiteMapPath'}
    return [{'action': 'goto', 'path': '/NavigationScenario.aspx'},
            snap('first request: the site map as a path, trees and menus; bound to XML; templated; made in code', 'render', 'databind', 'dynamic', *(['templates'] if name in templated else [])),
            ] + click_link('tvStatic', 'tf') + [
            snap('a node expanded by a postback (EnableClientScript=false)', 'postbackevent', 'viewstate', only={'TreeView'}),
            {'action': 'clicktextin', 'target': 'tvStatic', 'value': 'Apple'},
            snap('a node selected (its value a control parameter\'s: ControlValueProperty)', 'postbackevent', 'change', 'bind', only={'TreeView'}),
            {'action': 'evaluate', 'target': 'tvStatic', 'value': "e.querySelectorAll('input[type=checkbox]')[2].checked = true;"},
            {'action': 'click', 'target': 'btnSubmit'},
            snap('a node\'s check box posted', 'postdata', 'change', only={'TreeView'}),
            ] + click_link('tvStatic', 'tv') + [
            snap('a node populated on demand by a postback', 'postbackevent', only={'TreeView'}),
            {'action': 'evaluate', 'target': 'tvCallback', 'value': "Array.from(e.querySelectorAll('a')).find(a => a.href.indexOf('TreeView_PopulateNode') >= 0).click();"},
            {'action': 'waitms', 'ms': 3000},
            snap('a node populated on demand by a client callback (no postback)', 'callback', only={'TreeView'}),
            {'action': 'clicktextin', 'target': 'mnuStatic', 'value': 'Help'},
            snap('a menu\'s item clicked', 'postbackevent', only={'Menu'}),
            {'action': 'clicktextin', 'target': 'mnuBound', 'value': 'Bound two'},
            snap('a bound menu\'s item clicked', 'postbackevent', 'databind', only={'Menu'}),
            {'action': 'clicktextin', 'target': 'mnuTemplated', 'value': 'Bound child'},
            snap('a templated menu\'s item clicked', 'postbackevent', 'templates', only={'Menu'}),
            {'action': 'clicktextin', 'target': 'tvDynamic', 'value': 'Dynamic node'},
            snap('a dynamic tree\'s node selected', 'postbackevent', 'dynamic', only={'TreeView'}),
            {'action': 'clicktextin', 'target': 'mnuDynamic', 'value': 'Dynamic child'},
            snap('a dynamic menu\'s item clicked', 'postbackevent', 'dynamic', only={'Menu'}),
            {'action': 'click', 'target': 'btnShow'},
            snap('hidden ones shown with what was added while hidden', 'visible', 'viewstate')]


login = ['Login', 'LoginName', 'LoginStatus', 'LoginView', 'ChangePassword', 'PasswordRecovery', 'CreateUserWizard', 'CreateUserWizardStep',
         'CompleteWizardStep', 'Wizard', 'WizardStep', 'TemplatedWizardStep']


def login_scenario(name):
    def snap(text, *mechanisms, only=None):
        own = [m for m in mechanisms if only is None or name in only] or ['viewstate']
        return {'action': 'snapshot', 'name': name + ' ' + tag(*dict.fromkeys(own)) + ' ' + text}
    templated = {'Login', 'LoginView', 'Wizard', 'TemplatedWizardStep', 'CreateUserWizard', 'CreateUserWizardStep', 'CompleteWizardStep', 'ChangePassword', 'PasswordRecovery'}
    wizard = {'Wizard', 'WizardStep', 'TemplatedWizardStep'}
    return [{'action': 'goto', 'path': '/LoginScenario.aspx?reset=1'},
            snap('first request, anonymous: the login forms, the status, the views, the wizards', 'render', 'databind', 'dynamic', *(['templates'] if name in templated else [])),
            {'action': 'fill', 'target': 'lgnMain_UserName', 'value': 'taro'},
            {'action': 'fill', 'target': 'lgnMain_Password', 'value': 'wrong'},
            {'action': 'click', 'target': 'lgnMain_LoginButton'},
            snap('a login with a wrong password: its error', 'viewstate'),
            {'action': 'fill', 'target': 'lgnMain_UserName', 'value': 'taro'},
            {'action': 'fill', 'target': 'lgnMain_Password', 'value': 'Passw0rd!'},
            {'action': 'click', 'target': 'lgnMain_LoginButton'},
            snap('logged in (redirected to the page): the name, the status, the role\'s view', 'viewstate', 'templates', only={'LoginView', 'Login'}),
            {'action': 'fill', 'target': 'cpwChange_ChangePasswordContainerID_CurrentPassword', 'value': 'Passw0rd!'},
            {'action': 'fill', 'target': 'cpwChange_ChangePasswordContainerID_NewPassword', 'value': 'Changed1!'},
            {'action': 'fill', 'target': 'cpwChange_ChangePasswordContainerID_ConfirmNewPassword', 'value': 'Changed1!'},
            {'action': 'click', 'target': 'cpwChange_ChangePasswordContainerID_ChangePasswordPushButton'},
            snap('the password changed: the success template', 'viewstate', 'templates', only={'ChangePassword'}),
            {'action': 'click', 'target': 'lsStatus'},
            snap('logged out by the status', 'viewstate', 'templates', only={'LoginView', 'LoginStatus'}),
            {'action': 'fill', 'target': 'prRecover_UserNameContainerID_UserName', 'value': 'nobody'},
            {'action': 'click', 'target': 'prRecover_UserNameContainerID_SubmitButton'},
            snap('a recovery for a user who does not exist', 'viewstate'),
            {'action': 'fill', 'target': 'prRecover_UserNameContainerID_UserName', 'value': 'taro'},
            {'action': 'click', 'target': 'prRecover_UserNameContainerID_SubmitButton'},
            snap('the user\'s question', 'viewstate'),
            {'action': 'fill', 'target': 'prRecover_QuestionContainerID_Answer', 'value': 'blue'},
            {'action': 'click', 'target': 'prRecover_QuestionContainerID_SubmitButton'},
            snap('the answer right: the password reset, its mail prepared (cancelled), the success template', 'viewstate', 'templates', only={'PasswordRecovery'}),
            {'action': 'fill', 'target': 'cuwCreate_CreateUserStepContainer_UserName', 'value': 'hanako'},
            {'action': 'fill', 'target': 'cuwCreate_CreateUserStepContainer_Password', 'value': 'Hanako1!'},
            {'action': 'fill', 'target': 'cuwCreate_CreateUserStepContainer_ConfirmPassword', 'value': 'Hanako1!'},
            {'action': 'fill', 'target': 'cuwCreate_CreateUserStepContainer_Email', 'value': 'hanako@example.com'},
            {'action': 'fill', 'target': 'cuwCreate_CreateUserStepContainer_Question', 'value': 'pet'},
            {'action': 'fill', 'target': 'cuwCreate_CreateUserStepContainer_Answer', 'value': 'cat'},
            {'action': 'clicksubmit', 'value': 'Create the user'},
            snap('a user created: the complete step', 'viewstate', 'templates', only={'CreateUserWizard', 'CreateUserWizardStep', 'CompleteWizardStep'}),
            {'action': 'fill', 'target': 'txtFirst', 'value': 'first step'},
            {'action': 'clicktextin', 'target': 'wizSteps', 'value': 'Next'},
            snap('the wizard\'s next step (the templated one)', 'viewstate', 'templates', only=wizard),
            {'action': 'clicktextin', 'target': 'wizSteps', 'value': 'Next'},
            {'action': 'clicktextin', 'target': 'wizSteps', 'value': 'Previous'},
            snap('next, then previous', 'viewstate', only=wizard),
            {'action': 'clicktextin', 'target': 'wizSteps', 'value': 'Last'},
            snap('a step chosen in the side bar', 'viewstate', only=wizard),
            {'action': 'clicktextin', 'target': 'wizSteps', 'value': 'Finish'},
            snap('finished: the complete step, the first step\'s value kept', 'viewstate', only=wizard),
            {'action': 'fill', 'target': 'txtDynamic', 'value': 'in a dynamic wizard'},
            {'action': 'clicktextin', 'target': 'wizDynamic', 'value': 'Next'},
            snap('a wizard made in code: its next step', 'dynamic', 'viewstate'),
            {'action': 'click', 'target': 'btnShow'},
            snap('a hidden wizard shown', 'visible', 'viewstate')]


sources = ['ObjectDataSource', 'SqlDataSource', 'AccessDataSource', 'XmlDataSource', 'LinqDataSource', 'QueryExtender', 'ModelErrorMessage']


def first_text(target, *values):
    # The text boxes of a grid's edit row or a details view's insert row (they have no IDs of their own), in order.
    script = 'const inputs = e.querySelectorAll("input[type=text]"); ' + ' '.join('inputs[%d].value = %s;' % (i, json.dumps(v)) for i, v in enumerate(values))
    return {'action': 'evaluate', 'target': target, 'value': script}


def source_scenario(name):
    def snap(text, *mechanisms, only=None):
        own = [m for m in mechanisms if only is None or name in only] or ['viewstate']
        return {'action': 'snapshot', 'name': name + ' ' + tag(*dict.fromkeys(own)) + ' ' + text}
    return [{'action': 'goto', 'path': '/DataSourceScenario.aspx?reset=1'},
            snap('first request: grids bound to each data source, the Access data source selected in code, ones made in code', 'render', 'databind', 'dynamic'),
            {'action': 'clicktextin', 'target': 'gvObject', 'value': 'Edit'},
            first_text('gvObject', 'Pen (edited)'),
            {'action': 'clicktextin', 'target': 'gvObject', 'value': 'Update'},
            snap('an object data source\'s row updated', 'viewstate', only={'ObjectDataSource'}),
            {'action': 'clicktextin', 'target': 'gvObject', 'value': 'Delete'},
            snap('an object data source\'s row deleted', 'viewstate', only={'ObjectDataSource'}),
            {'action': 'clicktextin', 'target': 'gvSql', 'value': 'Name'},
            snap('a SQL data source sorted', 'viewstate', only={'SqlDataSource'}),
            {'action': 'clicktextin', 'target': 'gvSql', 'value': 'Edit'},
            first_text('gvSql', 'Bag (edited)', '25.5'),
            {'action': 'clicktextin', 'target': 'gvSql', 'value': 'Update'},
            snap('a SQL data source\'s row updated (its update command\'s parameters)', 'viewstate', only={'SqlDataSource'}),
            {'action': 'clicktextin', 'target': 'gvSql', 'value': 'Delete'},
            snap('a SQL data source\'s row deleted', 'viewstate', only={'SqlDataSource'}),
            first_text('dvSqlInsert', 'Z9', 'Zipper', '9.5'),
            {'action': 'clicktextin', 'target': 'dvSqlInsert', 'value': 'Insert'},
            snap('a SQL data source\'s row inserted', 'viewstate', only={'SqlDataSource'}),
            {'action': 'clicktextin', 'target': 'gvLinq', 'value': '2'},
            snap('a LINQ data source paged', 'viewstate', only={'LinqDataSource'}),
            {'action': 'clicktextin', 'target': 'gvLinq', 'value': 'Price'},
            snap('a LINQ data source sorted', 'viewstate', only={'LinqDataSource'}),
            {'action': 'fill', 'target': 'txtMinimum', 'value': '100'},
            {'action': 'click', 'target': 'btnSubmit'},
            snap('a LINQ data source\'s where parameter from a text box', 'viewstate', only={'LinqDataSource'}),
            {'action': 'fill', 'target': 'txtSearch', 'value': 'a'},
            {'action': 'click', 'target': 'btnSubmit'},
            snap('a query extender\'s search from a text box', 'viewstate', only={'QueryExtender'}),
            {'action': 'click', 'target': 'btnModel'},
            snap('a model error shown', 'viewstate', only={'ModelErrorMessage'}),
            {'action': 'fill', 'target': 'txtModelName', 'value': 'named'},
            {'action': 'click', 'target': 'btnModel'},
            snap('no model error', 'viewstate', only={'ModelErrorMessage'}),
            {'action': 'clicktextin', 'target': 'gvDynamic', 'value': 'Name'},
            snap('a grid sorted by a data source made in code', 'dynamic', 'viewstate'),
            {'action': 'click', 'target': 'btnShow'},
            snap('a hidden grid bound to a data source shown', 'visible', 'viewstate')]


web_parts = ['WebPartManager', 'ProxyWebPartManager', 'WebPartZone', 'CatalogZone', 'DeclarativeCatalogPart', 'PageCatalogPart', 'ImportCatalogPart',
             'EditorZone', 'AppearanceEditorPart', 'BehaviorEditorPart', 'LayoutEditorPart', 'PropertyGridEditorPart', 'ConnectionsZone']


def web_parts_scenario(name):
    def snap(text, *mechanisms, only=None):
        own = [m for m in mechanisms if only is None or name in only] or ['viewstate']
        return {'action': 'snapshot', 'name': name + ' ' + tag(*dict.fromkeys(own)) + ' ' + text}
    catalog = {'CatalogZone', 'DeclarativeCatalogPart', 'PageCatalogPart', 'ImportCatalogPart'}
    editor = {'EditorZone', 'AppearanceEditorPart', 'BehaviorEditorPart', 'LayoutEditorPart', 'PropertyGridEditorPart'}
    return [{'action': 'goto', 'path': '/WebPartsScenario.aspx?reset=1'},
            snap('first request (browse): the parts in their zones, a zone made in code', 'render', 'databind', 'dynamic', *(['templates'] if name == 'WebPartZone' else [])),
            {'action': 'click', 'target': 'btnDesign'},
            snap('the design mode', 'viewstate'),
            {'action': 'click', 'target': 'btnCatalog'},
            snap('the catalog mode: the catalog zone and its parts', 'viewstate', 'templates', only=catalog),
            {'action': 'evaluate', 'target': 'czCatalog', 'value': "e.querySelector('input[type=checkbox]').checked = true;"},
            {'action': 'clicktextin', 'target': 'czCatalog', 'value': 'Add'},
            snap('a part added from the catalog (its check box posted, the zone\'s Add)', 'postdata', 'postbackevent', only={'CatalogZone'}),
            {'action': 'click', 'target': 'btnEditPart'},
            snap('the edit mode: the editor zone and its parts for the first part', 'viewstate', 'templates', only=editor),
            {'action': 'evaluate', 'target': 'ezEditor', 'value': "e.querySelector('input[type=text]').value = 'Renamed part';"},
            {'action': 'clicktextin', 'target': 'ezEditor', 'value': 'OK'},
            snap('the first part\'s title edited (the editor zone\'s OK)', 'postbackevent', only={'EditorZone', 'AppearanceEditorPart'}),
            {'action': 'click', 'target': 'btnConnectPart'},
            snap('the connect mode for the first part', 'viewstate'),
            {'action': 'clicktextin', 'target': 'cnzConnections', 'value': 'Close'},
            snap('the connections zone closed', 'postbackevent', only={'ConnectionsZone'}),
            {'action': 'click', 'target': 'btnBrowse'},
            {'action': 'clicktextin', 'target': 'wpzMain', 'value': 'Minimize'},
            snap('a part minimized by its zone\'s verb', 'postbackevent', only={'WebPartZone', 'WebPartManager'}),
            {'action': 'clicktextin', 'target': 'wpzMain', 'value': 'Restore'},
            snap('restored', 'postbackevent', only={'WebPartZone'}),
            {'action': 'clicktextin', 'target': 'wpzDynamic', 'value': 'Minimize'},
            snap('a part of the zone made in code minimized', 'dynamic', 'postbackevent', only={'WebPartZone'}),
            {'action': 'click', 'target': 'btnShow'},
            snap('a hidden zone shown', 'visible', 'viewstate')]


def write(name, steps, page, **more):
    data = {'_comment': 'The ' + name + "'s scenario layer (" + page + '; tests/WebControlsParity/README.md; written by more-scenarios.py). A snapshot\'s tags are the mechanisms it covers.',
            'ignorePatterns': [], 'steps': steps}
    data.update(more)
    with open(os.path.join(out, name + '.json'), 'w', encoding='utf-8', newline='\n') as f:
        json.dump(data, f, indent=2, ensure_ascii=False)
        f.write('\n')


for name in more:
    write(name, more_scenario(name), 'MoreScenario.aspx')
for name in content:
    write(name, content_scenario(name), 'ContentScenario.aspx')
for name in ajax:
    write(name, ajax_scenario(name), 'AjaxScenario.aspx')
for name in navigation:
    write(name, navigation_scenario(name), 'NavigationScenario.aspx')
for name in login:
    write(name, login_scenario(name), 'LoginScenario.aspx')
for name in sources:
    write(name, source_scenario(name), 'DataSourceScenario.aspx')
for name in web_parts:
    # A web part added at run time is named "wp" and a random number (WebPartManager's dynamic IDs).
    write(name, web_parts_scenario(name), 'WebPartsScenario.aspx', maskIds=['wp-?[0-9]+'])
print(len(more) + len(content) + len(ajax) + len(navigation) + len(login) + len(sources) + len(web_parts), 'scenarios')
