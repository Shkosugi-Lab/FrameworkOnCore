<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="DeepTemplates.aspx.cs" Inherits="NestingProbe.DeepTemplates" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Deep templates</title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Repeater &gt; DataList &gt; Repeater</h2>
        <asp:Repeater ID="rptA" runat="server" OnItemCommand="ACommand" OnItemDataBound="ABound">
            <ItemTemplate>
                <h3><%# HttpUtility.HtmlEncode((string)Eval("Name")) %></h3>
                <asp:LinkButton ID="lnkTop" runat="server" CommandName="top" CommandArgument='<%# Eval("Id") %>' Text="top-level command" />
                <asp:DataList ID="dlB" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"
                    OnItemCommand="BCommand" OnItemDataBound="BBound">
                    <ItemTemplate>
                        <b><%# HttpUtility.HtmlEncode((string)Eval("Name")) %></b>
                        <asp:Button ID="btnMid" runat="server" CommandName="mid" CommandArgument='<%# Eval("Id") %>' Text="mid" />
                        <asp:Repeater ID="rptC" runat="server" OnItemCommand="CCommand">
                            <ItemTemplate>
                                <asp:Button ID="btnPick" runat="server" CommandName="pick"
                                    CommandArgument='<%# Container.DataItem %>' Text='<%# "pick " + Container.DataItem %>' />
                            </ItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </asp:DataList>
            </ItemTemplate>
        </asp:Repeater>

        <h2>GridView rows that post back on their own</h2>
        <asp:GridView ID="gvAuto" runat="server" AutoGenerateColumns="false" DataKeyNames="Id">
            <Columns>
                <asp:TemplateField HeaderText="Name">
                    <ItemTemplate><%# HttpUtility.HtmlEncode((string)Eval("Name")) %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Option">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlOpt" runat="server" AutoPostBack="true" OnSelectedIndexChanged="OptionChanged">
                            <asp:ListItem Text="A" Value="a" />
                            <asp:ListItem Text="B" Value="b" />
                            <asp:ListItem Text="C" Value="c" />
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Flag">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkRow" runat="server" AutoPostBack="true" OnCheckedChanged="FlagChanged" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Note">
                    <ItemTemplate>
                        <asp:TextBox ID="txtRow" runat="server" OnTextChanged="NoteChanged" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <h2>Templates made in code</h2>
        <asp:Repeater ID="rptDyn" runat="server" />
        <asp:Repeater ID="rptLoaded" runat="server" />
        <p><asp:Button ID="btnDynPost" runat="server" Text="Post back" OnClick="DynPost" /></p>

        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
