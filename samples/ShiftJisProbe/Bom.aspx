<%@ Page Language="C#" %>
<script runat="server">
    string 説明() { return "UTF-8(BOM 付き)のページ:表示・能力・吉野家"; }
</script>
<!DOCTYPE html>
<html>
<head><title>BOM 付き</title></head>
<body>
    <p id="script"><%= 説明() %></p>
    <p id="markup">応答は shift_jis: ①②③ と ～ は表せる</p>
</body>
</html>
