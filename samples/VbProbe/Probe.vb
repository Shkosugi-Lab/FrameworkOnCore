Imports System.Collections.Generic
Imports System.Web
Imports System.Web.UI

''' <summary>
''' A page that writes "name = value" lines (parity-scenario.json compares them with the original's): each case alone,
''' an exception written as its type and message; control characters shown.
''' </summary>
Public MustInherit Class ProbePage
    Inherits Page

    Private ReadOnly lines As New List(Of String)

    Protected Sub [Case](name As String, value As Func(Of Object))
        Dim text As String
        Try
            text = Show(value())
        Catch ex As Exception
            text = "!" & ex.GetType().FullName & ": " & Show(ex.Message)
        End Try
        lines.Add(name & " = " & text)
    End Sub

    Protected Sub Section(title As String)
        lines.Add("## " & title)
    End Sub

    Public Shared Function Show(value As Object) As String
        If value Is Nothing Then Return "(Nothing)"
        Dim s = If(TypeOf value Is String, DirectCast(value, String), System.Convert.ToString(value, System.Globalization.CultureInfo.CurrentCulture))
        Dim b As New System.Text.StringBuilder()
        For Each c In s
            Select Case c
                Case ControlChars.Cr : b.Append("\r")
                Case ControlChars.Lf : b.Append("\n")
                Case ControlChars.Tab : b.Append("\t")
                Case Else
                    If AscW(c) < 32 OrElse AscW(c) = &HA0 OrElse AscW(c) = &H202F OrElse AscW(c) = &H3000 Then
                        b.Append("\u").Append(AscW(c).ToString("x4"))
                    Else
                        b.Append(c)
                    End If
            End Select
        Next
        Return "[" & b.ToString() & "]"
    End Function

    Protected MustOverride Sub Run()

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        Run()
        writer.Write("<!DOCTYPE html><html><head><meta charset=""utf-8""><title>" & Me.GetType().BaseType.Name & "</title></head><body><pre id=""out"">")
        writer.Write(HttpUtility.HtmlEncode(String.Join(vbLf, lines)))
        writer.Write("</pre></body></html>")
    End Sub
End Class
