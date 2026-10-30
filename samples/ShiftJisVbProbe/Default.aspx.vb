Imports System.Configuration
Imports System.IO
Imports System.Text

' 既定のページ(Visual Basic、Shift_JIS)
Partial Public Class _Default
    Inherits System.Web.UI.Page

    Private Sub ページ読み込み(sender As Object, e As EventArgs) Handles Me.Load
        Dim 結果 As New StringBuilder()
        Dim 表示件数 As Integer = 3
        結果.AppendLine("件数: " & 表示件数 & " 件(表示能力)")
        結果.AppendLine("見出し: " & ConfigurationManager.AppSettings("見出し"))
        結果.AppendLine("モジュール: " & 表示部品.説明文)
        結果.AppendLine("Encoding.Default: " & Encoding.Default.WebName & " " & Encoding.Default.GetByteCount("ソ"))
        結果.AppendLine("StrConv: " & StrConv("ｿﾌﾄ", VbStrConv.Wide))
        If IsPostBack Then
            ' メモを Shift_JIS で保存して読む(App_Data\memo.txt)
            Dim 保存先 = Server.MapPath("~/App_Data") & "\memo.txt"
            Directory.CreateDirectory(Server.MapPath("~/App_Data"))
            File.WriteAllText(保存先, txtMemo.Text & "(保存済み)", Encoding.GetEncoding("shift_jis"))
            結果.AppendLine("保存: " & File.ReadAllText(保存先, Encoding.GetEncoding(932)) & " " & File.ReadAllBytes(保存先).Length & " bytes")
        End If
        Out.Text = Server.HtmlEncode(結果.ToString())
    End Sub
End Class
