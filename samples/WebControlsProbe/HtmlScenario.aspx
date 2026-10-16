<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="HtmlScenario.aspx.cs" Inherits="WebControlsProbe.HtmlScenario" %>
<!DOCTYPE html>
<html id="elmPage" runat="server" lang='<%# "en" %>'>
<head id="hdPage" runat="server">
    <title id="ttlPage" runat="server">HTML controls scenario</title>
    <meta id="metaBound" runat="server" name="description" content='<%# BoundText %>' />
    <link id="lnkBound" runat="server" rel="stylesheet" href='<%# "~/Images/probe.css" %>' />
</head>
<body>
    <form id="form1" runat="server" title='<%# BoundText %>'>
        <p>
            Generic:
            <span id="genPlain" runat="server">Plain</span>
            <span id="genBound" runat="server" title='<%# BoundText %>'><%# BoundText %></span>
            <span id="genCode" runat="server"></span>
            <span id="genNoViewState" runat="server" enableviewstate="false"></span>
            <div id="genHidden" runat="server" visible="false"></div>
        </p>
        <p>
            Anchor:
            <a id="ancTarget" runat="server" href="~/ButtonTarget.aspx">Go to the target</a>
            <a id="ancSubmit" runat="server" onserverclick="Clicked" causesvalidation="false">Submit anchor</a>
            <a id="ancValidate" runat="server" onserverclick="Clicked" validationgroup="g1">Validate g1 by an anchor</a>
            <a id="ancDisabled" runat="server" onserverclick="Clicked" disabled="disabled">Disabled anchor</a>
            <a id="ancBound" runat="server" href='<%# "~/ButtonTarget.aspx?id=" + 7 %>'><%# BoundText %></a>
            <a id="ancHidden" runat="server" visible="false" onserverclick="Clicked" causesvalidation="false"></a>
        </p>
        <p>
            Image:
            <img id="imgPlain" runat="server" src="~/Images/probe.png" alt="Logo" />
            <img id="imgBound" runat="server" src='<%# "~/Images/probe.png" %>' alt='<%# BoundText %>' />
            <img id="imgCode" runat="server" src="~/Images/probe.png" />
            <img id="imgHidden" runat="server" src="~/Images/probe.png" visible="false" />
            <iframe id="ifrPlain" runat="server" src="~/Images/probe.png" width="30" height="20"></iframe>
            <map name="probeMap"><area id="areaPlain" runat="server" shape="rect" coords="0,0,10,10" href="~/ButtonTarget.aspx" alt='<%# BoundText %>' /></map>
            <audio id="audPlain" runat="server" src='<%# "~/Images/probe.png" %>'></audio>
            <video id="vidPlain" runat="server" poster="~/Images/probe.png" width="20" height="10"><source id="srcPlain" runat="server" src="~/Images/probe.png" /><track id="trkPlain" runat="server" src='<%# "~/Images/probe.png" %>' /></video>
            <embed id="embPlain" runat="server" src='<%# "~/Images/probe.png" %>' type="image/png" />
        </p>
        <table id="tblPlain" runat="server" border="1">
            <tr id="trPlain" runat="server"><td id="tdPlain" runat="server">Plain cell</td><td id="tdCode" runat="server"></td></tr>
            <tr id="trBound" runat="server" title='<%# BoundText %>'><td id="tdBound" runat="server"><%# BoundText %></td><td id="tdHiddenCell" runat="server" visible="false">Hidden cell</td></tr>
        </table>
        <table id="tblCode" runat="server" border="1"></table>
        <table id="tblHidden" runat="server" visible="false"></table>
        <p>
            Inputs:
            <input id="inpText" runat="server" type="text" onserverchange="Changed" />
            <input id="inpTextBound" runat="server" type="text" value='<%# BoundText %>' onserverchange="Changed" />
            <input id="inpTextNoViewState" runat="server" type="text" enableviewstate="false" onserverchange="Changed" />
            <input id="inpTextDisabled" runat="server" type="text" value="disabled" disabled="disabled" onserverchange="Changed" />
            <input id="inpTextHidden" runat="server" type="text" visible="false" onserverchange="Changed" />
            <input id="inpPassword" runat="server" type="password" onserverchange="Changed" />
            <input id="inpHidden" runat="server" type="hidden" value="initial" onserverchange="Changed" />
            <input id="inpHiddenBound" runat="server" type="hidden" value='<%# BoundText %>' onserverchange="Changed" />
            <input id="inpCheck" runat="server" type="checkbox" onserverchange="Changed" />
            <input id="inpCheckBound" runat="server" type="checkbox" checked='<%# BoundFlag %>' onserverchange="Changed" />
            <input id="inpRadioSmall" runat="server" type="radio" name="size" onserverchange="Changed" />
            <input id="inpRadioLarge" runat="server" type="radio" name="size" onserverchange="Changed" />
            <input id="inpEmail" runat="server" type="email" onserverchange="Changed" />
            <input id="inpEmailBound" runat="server" type="number" value='<%# 7 %>' onserverchange="Changed" />
            <textarea id="txaNote" runat="server" onserverchange="Changed"></textarea>
            <textarea id="txaBound" runat="server" onserverchange="Changed"><%# BoundText %></textarea>
            <select id="selSize" runat="server" onserverchange="Changed">
                <option value="S">Small</option>
                <option value="M" selected="selected">Medium</option>
                <option value="L">Large</option>
            </select>
            <select id="selMulti" runat="server" multiple="true" size="3" onserverchange="Changed">
                <option value="S">Small</option>
                <option value="M">Medium</option>
                <option value="L">Large</option>
            </select>
            <select id="selBound" runat="server" datatextfield="Name" datavaluefield="Code" onserverchange="Changed"></select>
            <input id="inpFile" runat="server" type="file" />
        </p>
        <p>
            Buttons:
            <button id="btnHtml" runat="server" onserverclick="Clicked" causesvalidation="false">HTML button</button>
            <button id="btnHtmlBound" runat="server" onserverclick="Clicked" causesvalidation="false"><%# BoundText %></button>
            <input id="inpButton" runat="server" type="button" value="Input button" onserverclick="Clicked" causesvalidation="false" />
            <input id="inpSubmit" runat="server" type="submit" value="Input submit" onserverclick="Clicked" causesvalidation="false" />
            <input id="inpSubmitBound" runat="server" type="submit" value='<%# BoundText %>' onserverclick="Clicked" causesvalidation="false" />
            <input id="inpReset" runat="server" type="reset" value="Reset" />
            <input id="inpResetBound" runat="server" type="reset" value='<%# BoundText %>' />
            <input id="inpImage" runat="server" type="image" src="~/Images/probe.png" alt="Input image" onserverclick="ImageClicked" causesvalidation="false" />
            <input id="inpImageBound" runat="server" type="image" src="~/Images/probe.png" alt='<%# BoundText %>' onserverclick="ImageClicked" causesvalidation="false" />
            <input id="inpSubmitValidate" runat="server" type="submit" value="Validate g1" onserverclick="Clicked" validationgroup="g1" />
            <button id="btnHtmlValidate" runat="server" onserverclick="Clicked" validationgroup="g1">Validate g1 by a button</button>
        </p>
        <p>
            Server validation (g1):
            <asp:RequiredFieldValidator ID="reqText" runat="server" ControlToValidate="inpText" ValidationGroup="g1" ErrorMessage="Text required" EnableClientScript="false" />
            <asp:RequiredFieldValidator ID="reqPassword" runat="server" ControlToValidate="inpPassword" ValidationGroup="g1" ErrorMessage="Password required" EnableClientScript="false" />
            <asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="inpEmail" ValidationGroup="g1" ErrorMessage="Email required" EnableClientScript="false" />
            <asp:RequiredFieldValidator ID="reqNote" runat="server" ControlToValidate="txaNote" ValidationGroup="g1" ErrorMessage="Note required" EnableClientScript="false" />
            <asp:CompareValidator ID="cmpSize" runat="server" ControlToValidate="selSize" ValidationGroup="g1" ValueToCompare="L" ErrorMessage="Size must be L" EnableClientScript="false" />
            <asp:RequiredFieldValidator ID="reqFile" runat="server" ControlToValidate="inpFile" ValidationGroup="g1" ErrorMessage="File required" EnableClientScript="false" />
        </p>
        <p>
            Client validation (g2):
            <input id="inpClient" runat="server" type="text" />
            <asp:RequiredFieldValidator ID="reqClient" runat="server" ControlToValidate="inpClient" ValidationGroup="g2" ErrorMessage="Client text required" />
            <input id="inpClientPassword" runat="server" type="password" />
            <asp:RequiredFieldValidator ID="reqClientPassword" runat="server" ControlToValidate="inpClientPassword" ValidationGroup="g2" ErrorMessage="Client password required" />
            <input id="inpClientEmail" runat="server" type="email" />
            <asp:RequiredFieldValidator ID="reqClientEmail" runat="server" ControlToValidate="inpClientEmail" ValidationGroup="g2" ErrorMessage="Client email required" />
            <textarea id="txaClient" runat="server"></textarea>
            <asp:RequiredFieldValidator ID="reqClientNote" runat="server" ControlToValidate="txaClient" ValidationGroup="g2" ErrorMessage="Client note required" />
            <select id="selClient" runat="server"><option value="">(choose)</option><option value="A">A</option></select>
            <asp:RequiredFieldValidator ID="reqClientSelect" runat="server" ControlToValidate="selClient" ValidationGroup="g2" ErrorMessage="Client select required" />
            <input id="inpClientFile" runat="server" type="file" />
            <asp:RequiredFieldValidator ID="reqClientFile" runat="server" ControlToValidate="inpClientFile" ValidationGroup="g2" ErrorMessage="Client file required" />
            <input id="inpSubmitClient" runat="server" type="submit" value="Validate g2" onserverclick="Clicked" validationgroup="g2" />
        </p>
        <p>Dynamic: <asp:PlaceHolder ID="Dynamic" runat="server" /></p>
        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CausesValidation="false" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" CausesValidation="false" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
