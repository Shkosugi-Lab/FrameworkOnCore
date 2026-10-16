<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="WebPartsScenario.aspx.cs" Inherits="WebControlsProbe.WebPartsScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Web parts scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:WebPartManager ID="wpmPage" runat="server" OnDisplayModeChanged="DisplayModeChanged" OnWebPartAdded="WebPartAdded" OnWebPartClosed="WebPartClosed" OnWebPartMoved="WebPartMoved" />
        <asp:ProxyWebPartManager ID="pwpmPage" runat="server" />
        <p>
            Display mode: <asp:Label ID="lblMode" runat="server" />
            <asp:Button ID="btnBrowse" runat="server" Text="Browse" OnClick="ToBrowse" />
            <asp:Button ID="btnDesign" runat="server" Text="Design" OnClick="ToDesign" />
            <asp:Button ID="btnCatalog" runat="server" Text="Catalog" OnClick="ToCatalog" />
            <asp:Button ID="btnEditPart" runat="server" Text="Edit the first part" OnClick="EditFirst" />
            <asp:Button ID="btnConnectPart" runat="server" Text="Connect the first part" OnClick="ConnectFirst" />
        </p>
        <asp:WebPartZone ID="wpzMain" runat="server" HeaderText="Main zone" WebPartVerbRenderMode="TitleBar">
            <ZoneTemplate>
                <asp:Label ID="lblPart" runat="server" Title="Label part" Text='<%# BoundText %>' />
                <asp:TextBox ID="txtPart" runat="server" Title="Text part" />
            </ZoneTemplate>
        </asp:WebPartZone>
        <asp:WebPartZone ID="wpzSide" runat="server" HeaderText="Side zone" EmptyZoneText="The side zone is empty." />
        <asp:CatalogZone ID="czCatalog" runat="server" HeaderText="Catalog">
            <ZoneTemplate>
                <asp:DeclarativeCatalogPart ID="dcpParts" runat="server" Title="Declared parts">
                    <WebPartsTemplate>
                        <asp:Label ID="lblCatalogPart" runat="server" Title="Catalog label" Text="Added from the catalog" />
                    </WebPartsTemplate>
                </asp:DeclarativeCatalogPart>
                <asp:PageCatalogPart ID="pcpPage" runat="server" />
                <asp:ImportCatalogPart ID="icpImport" runat="server" />
            </ZoneTemplate>
        </asp:CatalogZone>
        <asp:EditorZone ID="ezEditor" runat="server" HeaderText="Editor">
            <ZoneTemplate>
                <asp:AppearanceEditorPart ID="aepAppearance" runat="server" />
                <asp:BehaviorEditorPart ID="bepBehavior" runat="server" />
                <asp:LayoutEditorPart ID="lepLayout" runat="server" />
                <asp:PropertyGridEditorPart ID="pgepGrid" runat="server" />
            </ZoneTemplate>
        </asp:EditorZone>
        <asp:ConnectionsZone ID="cnzConnections" runat="server" HeaderText="Connections" />
        <asp:WebPartZone ID="wpzHidden" runat="server" HeaderText="Hidden zone" Visible="false" />
        <p>Dynamic: <asp:PlaceHolder ID="Dynamic" runat="server" /></p>
        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
