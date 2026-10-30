<%@ Page Language="C#" %>
<script runat="server">
    // ページ自身のスクリプト(fileEncoding="shift_jis" のフォルダー)
    string 案内() { return "宣言された fileEncoding で読んだページ:申込表・ソフト"; }
</script>
<!DOCTYPE html>
<html>
<head><title>宣言あり</title></head>
<body>
    <p id="script"><%= 案内() %></p>
    <p id="markup">マークアップ:能力十分、表示予定</p>
    <p id="encoding"><%= Response.ContentEncoding.WebName %></p>
</body>
</html>
