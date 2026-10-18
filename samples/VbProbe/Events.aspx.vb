Option Strict On

Imports System.Xml.Linq

    ''' <summary>
    ''' Visual Basic's event wiring (AutoEventWireup off): Handles on the page's and the controls' events, WithEvents
    ''' fields (the designer's), a control made in code with AddHandler, the order they run in; XML literals and axis
    ''' properties written into the page.
    ''' </summary>
    Partial Public Class Events
        Inherits System.Web.UI.Page

        Private ReadOnly entries As New List(Of String)

        Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
            entries.Add("Init")
            Dim dynamic As New Button With {.ID = "btnDynamic", .Text = "Dynamic"}
            AddHandler dynamic.Click, AddressOf DynamicClicked
            ph.Controls.Add(dynamic)
        End Sub

        Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            entries.Add("Load IsPostBack=" & IsPostBack.ToString())
        End Sub

        Private Sub Go(sender As Object, e As EventArgs) Handles btnGo.Click, btnXml.Click
            entries.Add("Click " & DirectCast(sender, Button).ID & " name=" & txtName.Text)
            If sender Is btnXml Then
                Dim name = txtName.Text
                Dim doc = <order id="7" customer=<%= name %>>
                              <line sku="A1" qty="2" price="1.25"/>
                              <line sku="B&amp;2" qty="1" price="0.3"/>
                              <!-- a comment -->
                          </order>
                entries.Add(doc.ToString())
                entries.Add("lines=" & doc.<line>.Count().ToString() & " first sku=" & doc.<line>.First().@sku & " total qty=" & doc.<line>.Sum(Function(l) CInt(l.@qty)).ToString())
                entries.Add("descendants=" & String.Join(",", doc...<line>.Select(Function(l) l.@sku)))
            End If
        End Sub

        Private Sub KindChanged(sender As Object, e As EventArgs) Handles ddlKind.SelectedIndexChanged
            entries.Add("SelectedIndexChanged " & ddlKind.SelectedValue)
        End Sub

        Private Sub NameChanged(sender As Object, e As EventArgs) Handles txtName.TextChanged
            entries.Add("TextChanged " & txtName.Text)
        End Sub

        Private Sub DynamicClicked(sender As Object, e As EventArgs)
            entries.Add("Dynamic clicked")
        End Sub

        Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
            entries.Add("PreRender")
            Log.Text = HttpUtility.HtmlEncode(String.Join(vbLf, entries))
        End Sub
    End Class
