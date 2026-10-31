<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void MapClick(object sender, ImageMapEventArgs e) { L("ImageMapClick(" + e.PostBackValue + ")"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>N/ImageMap</title></head>
<body>
  <form id="form1" runat="server">
    <h1>N/ImageMap</h1>
    <asp:ImageMap ID="im" runat="server" ImageUrl="map.gif" Width="200" Height="100" AlternateText="map" OnClick="MapClick">
      <asp:RectangleHotSpot Left="0" Top="0" Right="100" Bottom="100" HotSpotMode="PostBack" PostBackValue="left" AlternateText="post back left" />
      <asp:RectangleHotSpot Left="100" Top="0" Right="200" Bottom="100" HotSpotMode="Navigate" NavigateUrl="~/N/Default.aspx" AlternateText="navigate" />
    </asp:ImageMap>
    <nav:Info runat="server" />
  </form>
</body>
</html>
