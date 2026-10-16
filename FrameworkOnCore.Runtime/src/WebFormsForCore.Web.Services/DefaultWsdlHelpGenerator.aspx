<%@ Page Language="C#" Debug="false" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Web.Services.Description" %>
<script runat="server">
    // FrameworkOnCore's help page of an ASMX web service (the page .NET Framework's CONFIG folder has, written anew: the
    // service, its operations, a link to its WSDL; ?op= an operation: its documentation, its SOAPAction). DocumentationServerProtocol
    // gives it the service's descriptions (Context.Items["wsdls"]).
    ServiceDescription description;
    Service service;
    string operationName;

    protected void Page_Load(object sender, EventArgs e)
    {
        var wsdls = (ServiceDescriptionCollection)Context.Items["wsdls"];
        description = wsdls == null ? null : wsdls.Cast<ServiceDescription>().FirstOrDefault(d => d.Services.Count > 0);
        service = description == null ? null : description.Services[0];
        operationName = Request.QueryString["op"];
    }

    IEnumerable<Operation> Operations()
    {
        if (description == null) return Enumerable.Empty<Operation>();
        return description.PortTypes.Cast<PortType>().SelectMany(p => p.Operations.Cast<Operation>())
            .GroupBy(o => o.Name).Select(g => g.First()).OrderBy(o => o.Name, StringComparer.Ordinal);
    }

    string Documentation(Operation operation)
    {
        return operation.Documentation ?? "";
    }

    string ServiceName { get { return service == null ? "Web Service" : service.Name; } }
</script>
<!DOCTYPE html>
<html>
<head>
    <title><%: ServiceName %> Web Service</title>
    <style>
        body { font-family: Verdana, sans-serif; margin: 0; }
        #content { margin: 20px; font-size: .8em; }
        .heading1 { background-color: #003366; color: #ffffff; font-size: 2em; padding: 10px 20px; }
        li { margin-bottom: 6px; }
    </style>
</head>
<body>
    <div class="heading1"><%: ServiceName %></div>
    <div id="content">
        <% if (operationName == null) { %>
            <% if (service != null && !string.IsNullOrEmpty(service.Documentation)) { %><p><%: service.Documentation %></p><% } %>
            <p>The following operations are supported. For a formal definition, please review the <a href="?WSDL">Service Description</a>.</p>
            <ul>
                <% foreach (var operation in Operations()) { %>
                <li><a href="?op=<%: operation.Name %>"><%: operation.Name %></a><% if (Documentation(operation).Length > 0) { %><br /><%: Documentation(operation) %><% } %></li>
                <% } %>
            </ul>
        <% } else { %>
            <p><a href="<%: Request.Path %>">Click here</a> for a complete list of operations.</p>
            <h2><%: operationName %></h2>
            <% var operation = Operations().FirstOrDefault(o => o.Name == operationName); %>
            <% if (operation == null) { %>
                <p>The operation is not one of the service's.</p>
            <% } else { %>
                <p><%: Documentation(operation) %></p>
                <p>SOAPAction: <%: description.TargetNamespace.TrimEnd('/') + "/" + operation.Name %></p>
                <p>See the <a href="?WSDL">Service Description</a> for its messages.</p>
            <% } %>
        <% } %>
    </div>
</body>
</html>
