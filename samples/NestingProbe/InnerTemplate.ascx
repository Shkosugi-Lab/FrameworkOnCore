<%@ Control Language="C#" ClassName="InnerTemplate" %>
<script runat="server">
    // Compiled at run time (no code-behind): the Literal's DataBinding reads the RepeaterItem it landed in
    // (LoadTemplate puts the template in this user control, its naming container; the item is its parent's).
    void Lit_DataBinding(object sender, EventArgs e)
    {
        var literal = (System.Web.UI.WebControls.Literal)sender;
        var item = (System.Web.UI.WebControls.RepeaterItem)literal.NamingContainer.NamingContainer;
        literal.Text = "[ascx] " + item.ItemIndex + " " + System.Web.HttpUtility.HtmlEncode(Convert.ToString(item.DataItem));
    }
</script>
<span class="ascx"><asp:Literal ID="litL" runat="server" OnDataBinding="Lit_DataBinding" /></span>
