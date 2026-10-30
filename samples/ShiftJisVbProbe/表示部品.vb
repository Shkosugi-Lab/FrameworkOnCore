' 表示部品: 日本語の名前のモジュール(Shift_JIS)
Public Module 表示部品
    Public Const 説明文 As String = "ソフト表示能力のモジュール"

    Public Function 署名(名前 As String) As String
        Return 名前 & " 様(申込表)"
    End Function

    ' 呼ばれない: .NET にない API(COM+)。変換器が本体を置き換え、このファイルを Shift_JIS のまま書き戻す。
    Public Function トランザクション中() As Boolean
        Return System.EnterpriseServices.ContextUtil.IsInTransaction
    End Function

    Public ReadOnly 末尾 As String = "後ろの行(十分)"
End Module
