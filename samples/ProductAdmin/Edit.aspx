<%@ Page Title="商品編集" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="ProductAdmin.Edit" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .field { margin: 8px 0; }
        .error { color: #c00; }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Literal ID="litTitle" runat="server" /></h2>

    <asp:Panel ID="pnlForm" runat="server" CssClass="edit-form">
        <div class="field">
            商品名:
            <asp:TextBox ID="txtName" runat="server" Width="240px" TabIndex="1" />
            <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                ErrorMessage="商品名は必須です。" Text="*" ForeColor="Red" Font-Bold="True"
                ValidationGroup="ProductForm" />
        </div>
        <div class="field">
            価格:
            <asp:TextBox ID="txtPrice" runat="server" Width="120px" TabIndex="2" ToolTip="税込価格を入力してください" />
            <asp:RequiredFieldValidator ID="rfvPrice" runat="server" ControlToValidate="txtPrice"
                ErrorMessage="価格は必須です。" Text="*" ForeColor="Red" Font-Bold="True"
                ValidationGroup="ProductForm" />
            <asp:RangeValidator ID="rvPrice" runat="server" ControlToValidate="txtPrice"
                Type="Double" MinimumValue="1" MaximumValue="999999"
                ErrorMessage="価格は 1〜999999 の範囲で入力してください。" Text="*"
                ForeColor="Red" Font-Bold="True" ValidationGroup="ProductForm" />
        </div>
        <div class="field">
            カテゴリ:
            <asp:DropDownList ID="ddlCategory" runat="server" />
        </div>
        <div class="field">
            <asp:CheckBox ID="chkInStock" runat="server" Text="在庫あり" />
        </div>

        <asp:ValidationSummary ID="vsMain" runat="server" ValidationGroup="ProductForm" />
        <asp:Label ID="lblError" runat="server" CssClass="error" />

        <div class="field">
            <asp:Button ID="btnSave" runat="server" Text="保存" OnClick="btnSave_Click"
                ToolTip="入力内容を保存します" ValidationGroup="ProductForm" />
            <asp:Button ID="btnCancel" runat="server" Text="キャンセル" OnClick="btnCancel_Click"
                CausesValidation="false" OnClientClick="return confirm('編集を破棄して一覧に戻りますか？');" />
        </div>
    </asp:Panel>
</asp:Content>
