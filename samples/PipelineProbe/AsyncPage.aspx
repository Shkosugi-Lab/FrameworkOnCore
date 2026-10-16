<%@ Page Language="C#" AutoEventWireup="true" Async="true" %>
<%@ Import Namespace="System.Threading.Tasks" %>
<script runat="server">
    // An asynchronous page: an async void Page_Load and a task registered (RegisterAsyncTask), each across an await; when
    // they run among the page's events (before PreRenderComplete), and what is there after the await (HttpContext.Current,
    // the request, the culture). C# 5: .NET Framework's own compiler compiles the page on IIS.
    readonly System.Collections.Generic.List<string> log = new System.Collections.Generic.List<string>();

    void Note(string what)
    {
        lock (log) log.Add(what);
    }

    string State()
    {
        return "HttpContext.Current " + (HttpContext.Current == null ? "null" : HttpContext.Current == Context ? "the request's" : "another") +
            ", Request.Path " + (HttpContext.Current == null ? "-" : HttpContext.Current.Request.Path) + ", culture " + System.Threading.Thread.CurrentThread.CurrentCulture.Name;
    }

    protected async void Page_Load(object sender, EventArgs e)
    {
        Note("Page_Load before await: " + State());
        await Task.Delay(20);
        Note("Page_Load after await: " + State());
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RegisterAsyncTask(new PageAsyncTask(async () =>
        {
            Note("Task before await: " + State());
            await Task.Delay(20);
            Note("Task after await: " + State());
        }));
        Note("OnLoad: a task registered");
    }

    protected override void OnPreRender(EventArgs e)
    {
        Note("PreRender");
        base.OnPreRender(e);
    }

    protected override void OnPreRenderComplete(EventArgs e)
    {
        Note("PreRenderComplete");
        base.OnPreRenderComplete(e);
    }

    protected override void Render(HtmlTextWriter writer)
    {
        Note("Render: " + State());
        Output.Text = HttpUtility.HtmlEncode(string.Join("\n", log));
        base.Render(writer);
    }
</script>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Pipeline probe: async page</title>
</head>
<body>
    <pre id="log"><asp:Literal ID="Output" runat="server" EnableViewState="false" /></pre>
</body>
</html>
