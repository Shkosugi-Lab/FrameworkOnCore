<%@ Page Language="C#" AutoEventWireup="true" EnableViewStateMac="false" %>
<script runat="server">
    // What the request brought: the form's keys and length, the query's length, the path's.
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "text/plain";
        Response.Write("form keys " + Request.Form.Count + ", form length " + Request.Form.ToString().Length + ", query length " + Request.Url.Query.Length +
            ", path length " + Request.Path.Length + ", content length " + Request.ContentLength);
        Response.End();
    }
</script>
