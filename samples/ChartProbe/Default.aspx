<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ChartProbe.Default" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>ChartProbe</title></head>
<body>
  <form id="form1" runat="server">
    <%-- Served by ChartImg.axd (ImageStorageMode UseHttpHandler: the file storage of Web.config), with an image map. --%>
    <asp:Chart ID="Sales" runat="server" Width="480" Height="300" ImageStorageMode="UseHttpHandler" ImageType="Png">
      <Titles><asp:Title Name="Title" Text="Sales by month" /></Titles>
      <Legends><asp:Legend Name="Default" Docking="Bottom" /></Legends>
      <Series>
        <asp:Series Name="Sales" ChartType="Column" ChartArea="Main" Legend="Default" ToolTip="#VALX: #VALY{C}" />
        <asp:Series Name="Orders" ChartType="Line" ChartArea="Main" Legend="Default" YAxisType="Secondary" MarkerStyle="Circle" MarkerSize="7" BorderWidth="2" ToolTip="#VALX: #VAL orders" />
      </Series>
      <ChartAreas><asp:ChartArea Name="Main" /></ChartAreas>
    </asp:Chart>
    <%-- Written by the page itself into the application's folder (ImageStorageMode UseImageLocation). --%>
    <asp:Chart ID="Saved" runat="server" Width="240" Height="200" ImageStorageMode="UseImageLocation" ImageLocation="~/ChartImages/saved_#SEQ(10,10)" ImageType="Png">
      <Series><asp:Series Name="Share" ChartType="Pie" ChartArea="Pie" Label="#PERCENT" /></Series>
      <ChartAreas><asp:ChartArea Name="Pie" /></ChartAreas>
    </asp:Chart>
  </form>
</body>
</html>