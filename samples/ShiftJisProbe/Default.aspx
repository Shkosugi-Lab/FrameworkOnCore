<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ShiftJisProbe._Default" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>既定のページ</title></head>
<body>
    <form id="form1" runat="server">
        <h1>Shift_JIS のページ(表示・能力・ソフト)</h1>
        <p id="markup">ページに直接書いた日本語:漢字、ひらがな、カタカナ、全角記号「～」「―」「∥」、半角カナ:ｱｲｳ</p>
        <p id="appcode">App_Code: <%= Greeting.Hello("太郎") %> / <%= Greeting.表題 %></p>
        <p id="globalres"><asp:Literal ID="litGlobal" runat="server" Text="<%$ Resources:Messages, Welcome %>" /></p>
        <p id="expr"><%= "式の中の文字列:申し込み表" %></p>
        <pre id="out"><asp:Literal ID="Out" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
