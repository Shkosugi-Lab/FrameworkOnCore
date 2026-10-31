<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected string Describe()
{
    var q = Request.QueryString;
    var lines = new System.Collections.Generic.List<string>();
    lines.Add("count=" + q.Count + " url.query=" + Request.Url.Query + " tostring=" + q.ToString());
    foreach (var key in q.AllKeys)
    {
        var values = q.GetValues(key);
        lines.Add("[" + (key == null ? "(null)" : key) + "] = " + (values == null ? "(null)" : string.Join(" | ", System.Linq.Enumerable.Select(values, v => "'" + v + "'"))));
    }
    lines.Add("Request[a]=" + (Request["a"] ?? "(null)"));
    return string.Join("\n", lines);
}
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>U/Query</title></head>
<body>
  <form id="form1" runat="server">
    <h1>U/Query</h1>
    <pre id="query"><%: Describe() %></pre>
    <nav:Info runat="server" />
  </form>
</body>
</html>
