<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MobileProbe.Default" %>
<mobile:Form ID="Main" runat="server" Title="Mobile probe">
  <mobile:Label ID="lblTitle" runat="server" StyleReference="title" Text="Mobile probe" />
  <mobile:Label ID="lblDevice" runat="server" Text="Device: other">
    <DeviceSpecific>
      <Choice Filter="isWml11" Text="Device: WML 1.1" />
      <Choice Filter="isHtml32" Text="Device: HTML 3.2" />
    </DeviceSpecific>
  </mobile:Label>
  <mobile:Label ID="lblFilters" runat="server" />
  <mobile:TextView ID="txtIntro" runat="server">The mobile controls of <b>ASP.NET 1.1</b>, as .NET Framework 4.8 still has them.</mobile:TextView>
  <mobile:Label ID="lblName" runat="server" Text="Your name:" />
  <mobile:TextBox ID="txtName" runat="server" MaxLength="20" />
  <mobile:RequiredFieldValidator ID="reqName" runat="server" ControlToValidate="txtName" ErrorMessage="Enter a name." />
  <mobile:SelectionList ID="selSize" runat="server" SelectType="DropDown">
    <Item Text="Small" Value="S" />
    <Item Text="Medium" Value="M" Selected="True" />
    <Item Text="Large" Value="L" />
  </mobile:SelectionList>
  <mobile:Command ID="cmdGreet" runat="server" Text="Greet" OnClick="cmdGreet_Click" />
  <mobile:Label ID="lblResult" runat="server" />
  <mobile:List ID="lstColors" runat="server" OnItemCommand="lstColors_ItemCommand" ItemsAsLinks="False">
    <Item Text="Red" Value="#f00" />
    <Item Text="Green" Value="#0f0" />
    <Item Text="Blue" Value="#00f" />
  </mobile:List>
  <mobile:Label ID="lblColor" runat="server" />
  <mobile:Link ID="lnkSecond" runat="server" NavigateUrl="#Second" Text="To the second form" />
</mobile:Form>
<mobile:Form ID="Second" runat="server" Title="Products">
  <mobile:Label ID="lblProducts" runat="server" Text="Products" StyleReference="title" />
  <mobile:ObjectList ID="objProducts" runat="server" LabelField="Name" AutoGenerateFields="True" />
  <mobile:Panel ID="pnlTotal" runat="server">
    <mobile:Label ID="lblTotal" runat="server" />
  </mobile:Panel>
  <mobile:Link ID="lnkMain" runat="server" NavigateUrl="#Main" Text="Back" />
</mobile:Form>