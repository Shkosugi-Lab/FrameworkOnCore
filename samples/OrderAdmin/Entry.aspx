<%@ Page Title="新規受注" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Entry.aspx.cs" Inherits="OrderAdmin.Entry" %>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <h2>新規受注</h2>

    <div class="field">
        顧客名:
        <asp:TextBox ID="txtCustomer" runat="server" Width="240px" />
        <asp:RequiredFieldValidator ID="rfvCustomer" runat="server" ControlToValidate="txtCustomer"
            ErrorMessage="顧客名は必須です。" Text="*" ForeColor="Red" ValidationGroup="Entry" />
    </div>
    <div class="field">
        商品:
        <asp:DropDownList ID="ddlProduct" runat="server" DataTextField="Name" DataValueField="Name" />
    </div>
    <div class="field">
        数量:
        <asp:TextBox ID="txtQuantity" runat="server" Width="80px" />
        <asp:RequiredFieldValidator ID="rfvQuantity" runat="server" ControlToValidate="txtQuantity"
            ErrorMessage="数量は必須です。" Text="*" ForeColor="Red" ValidationGroup="Entry" />
        <asp:RangeValidator ID="rvQuantity" runat="server" ControlToValidate="txtQuantity"
            Type="Integer" MinimumValue="1" MaximumValue="100"
            ErrorMessage="数量は 1〜100 で入力してください。" Text="*" ForeColor="Red" ValidationGroup="Entry" />
        <asp:CustomValidator ID="cvStock" runat="server" ControlToValidate="txtQuantity"
            OnServerValidate="cvStock_ServerValidate"
            ErrorMessage="在庫が不足しています。" Text="*" ForeColor="Red" ValidationGroup="Entry" />
    </div>
    <div class="field">
        メール:
        <asp:TextBox ID="txtEmail" runat="server" Width="240px" />
        <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
            ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
            ErrorMessage="メールアドレスの形式が正しくありません。" Text="*" ForeColor="Red" ValidationGroup="Entry" />
    </div>
    <div class="field">
        メール(確認):
        <asp:TextBox ID="txtEmailConfirm" runat="server" Width="240px" />
        <asp:CompareValidator ID="cvEmail" runat="server" ControlToValidate="txtEmailConfirm"
            ControlToCompare="txtEmail" Operator="Equal" Type="String"
            ErrorMessage="メールアドレスが一致しません。" Text="*" ForeColor="Red" ValidationGroup="Entry" />
    </div>
    <div class="field">
        備考:
        <asp:TextBox ID="txtNote" runat="server" TextMode="MultiLine" Rows="3" Width="320px" />
    </div>

    <asp:ValidationSummary ID="vsEntry" runat="server" ValidationGroup="Entry" />

    <div class="field">
        <asp:Button ID="btnSubmit" runat="server" Text="登録" OnClick="btnSubmit_Click" ValidationGroup="Entry" />
    </div>
</asp:Content>
