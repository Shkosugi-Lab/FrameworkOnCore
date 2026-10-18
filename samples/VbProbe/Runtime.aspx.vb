Option Strict Off
Option Explicit On

Imports System.Globalization

''' <summary>
''' The Visual Basic runtime as a page sees it (Microsoft.VisualBasic: .NET Framework's and .NET's are different
''' builds): number formats and conversions, string functions, StrConv's Japanese conversions, ANSI code-page functions,
''' Like, dates, Rnd with a seed, IIf/Choose/Switch, Err, late binding with Option Strict Off.
''' </summary>
Public Class Runtime
    Inherits ProbePage

    Protected Overrides Sub Run()
        Dim d As Double = 0.1 + 0.2
        Dim third As Double = 1 / 3
        Section("formatting")
        [Case]("CStr(d)", Function() CStr(d))
        [Case]("d & """"", Function() d & "")
        [Case]("Str(d)", Function() Str(d))
        [Case]("Str(-0.5)", Function() Str(-0.5))
        [Case]("Format(d)", Function() Format(d))
        [Case]("Format(third, ""0.00"")", Function() Format(third, "0.00"))
        [Case]("Format(2.675, ""Fixed"")", Function() Format(2.675, "Fixed"))
        [Case]("Format(1234.5, ""Currency"")", Function() Format(1234.5, "Currency"))
        [Case]("Format(0.125, ""Percent"")", Function() Format(0.125, "Percent"))
        [Case]("Format(1234567, ""Standard"")", Function() Format(1234567, "Standard"))
        [Case]("Format(Now date)", Function() Format(New DateTime(2026, 3, 4, 13, 5, 9), "yyyy/MM/dd HH:mm:ss"))
        [Case]("Format(date, ""Long Date"")", Function() Format(New DateTime(2026, 3, 4), "Long Date"))
        [Case]("Format(True, ""Yes/No"")", Function() Format(True, "Yes/No"))
        [Case]("FormatNumber(2.675, 2)", Function() FormatNumber(2.675, 2))
        [Case]("FormatNumber(-0.004, 2)", Function() FormatNumber(-0.004, 2))
        [Case]("FormatNumber(1234.5, 1, , TriState.True)", Function() FormatNumber(-1234.5, 1, , TriState.True))
        [Case]("FormatCurrency(22.5, 0)", Function() FormatCurrency(22.5, 0))
        [Case]("FormatCurrency(-1234.5)", Function() FormatCurrency(-1234.5))
        [Case]("FormatPercent(0.125, 1)", Function() FormatPercent(0.125, 1))
        [Case]("FormatDateTime(LongDate)", Function() FormatDateTime(New DateTime(2026, 3, 4, 13, 5, 9), DateFormat.LongDate))
        [Case]("FormatDateTime(GeneralDate)", Function() FormatDateTime(New DateTime(2026, 3, 4, 13, 5, 9), DateFormat.GeneralDate))
        [Case]("Hex/Oct", Function() Hex(255) & " " & Oct(8) & " " & Hex(-1))

        Section("conversions")
        [Case]("CDbl(""1,234.5"")", Function() CDbl("1,234.5"))
        [Case]("CDbl(""1e400"")", Function() CDbl("1e400"))
        [Case]("CInt(2.5)", Function() CInt(2.5))
        [Case]("CInt(3.5)", Function() CInt(3.5))
        [Case]("CInt(""&H1F"")", Function() CInt("&H1F"))
        [Case]("CLng(""1e3"")", Function() CLng("1e3"))
        [Case]("CDec(""1,000.50"")", Function() CDec("1,000.50"))
        [Case]("CBool(""True"")", Function() CBool("True"))
        [Case]("CBool(""1"")", Function() CBool("1"))
        [Case]("CBool(""yes"")", Function() CBool("yes"))
        [Case]("CDate(""2026/3/4"")", Function() CDate("2026/3/4").ToString("s"))
        [Case]("CDate(""March 4, 2026 1:05 PM"")", Function() CDate("March 4, 2026 1:05 PM").ToString("s"))
        [Case]("CDate(""4.3.2026"")", Function() CDate("4.3.2026").ToString("s"))
        [Case]("CInt(""abc"")", Function() CInt("abc"))
        [Case]("Val(""12abc"")", Function() Val("12abc"))
        [Case]("Val(""  1 2 3"")", Function() Val("  1 2 3"))
        [Case]("Val(""&HFF"")", Function() Val("&HFF"))
        [Case]("Val(""1,5"")", Function() Val("1,5"))
        [Case]("Val(""1e400"")", Function() Val("1e400"))
        [Case]("IsNumeric(""1,000"")", Function() IsNumeric("1,000"))
        [Case]("IsNumeric(""$5"")", Function() IsNumeric("$5"))
        [Case]("IsNumeric(""&H10"")", Function() IsNumeric("&H10"))
        [Case]("IsNumeric(""1e5"")", Function() IsNumeric("1e5"))
        [Case]("IsNumeric("" 7 "")", Function() IsNumeric(" 7 "))
        [Case]("IsNumeric(""１２"")", Function() IsNumeric("１２"))
        [Case]("IsDate(""2026-02-30"")", Function() IsDate("2026-02-30"))
        [Case]("IsDate(""March 4"")", Function() IsDate("March 4"))
        [Case]("CStr(Nothing)", Function() CStr(Nothing) Is Nothing)
        [Case]("CStr(date)", Function() CStr(New DateTime(2026, 3, 4)))
        [Case]("CStr(date time)", Function() CStr(New DateTime(2026, 3, 4, 13, 5, 9)))
        [Case]("CStr(True)", Function() CStr(True))
        [Case]("TypeName/VarType", Function() TypeName(1.5) & " " & VarType(1.5) & " " & TypeName(Nothing) & " " & TypeName(New Integer() {1}))

        Section("strings")
        Dim s = "Hello, World"
        [Case]("Left/Right/Mid", Function() Left(s, 3) & "|" & Right(s, 3) & "|" & Mid(s, 4, 5) & "|" & Mid(s, 20))
        [Case]("InStr/InStrRev", Function() InStr(s, "o") & " " & InStrRev(s, "o") & " " & InStr(s, "O") & " " & InStr(1, s, "O", CompareMethod.Text))
        [Case]("Replace count", Function() Replace("aaaa", "a", "b", 2, 2))
        [Case]("Split/Join", Function() Join(Split("a,,b", ","), "|"))
        [Case]("LCase/UCase", Function() LCase("ÅBC") & UCase("straße"))
        [Case]("StrReverse", Function() StrReverse("abc😀"))
        [Case]("Space/StrDup", Function() "[" & Space(3) & StrDup(3, "xy") & "]")
        [Case]("Trim full-width", Function() "[" & Trim("　a　") & "]")
        [Case]("Len(Nothing)", Function() Len(CStr(Nothing)))
        [Case]("Asc(""あ"")", Function() Asc("あ"))
        [Case]("AscW(""あ"")", Function() AscW("あ"))
        [Case]("Chr(&H82A0)", Function() Chr(&H82A0))
        [Case]("Chr(200)", Function() AscW(Chr(200)))
        [Case]("ChrW(&H3042)", Function() ChrW(&H3042))
        [Case]("LenB-like", Function() System.Text.Encoding.GetEncoding(932).GetByteCount("あいa"))
        [Case]("StrConv Wide", Function() StrConv("abc123ｱｲｳ", VbStrConv.Wide, &H411))
        [Case]("StrConv Narrow", Function() StrConv("ＡＢＣ１２３アイウ", VbStrConv.Narrow, &H411))
        [Case]("StrConv Katakana", Function() StrConv("ひらがな", VbStrConv.Katakana, &H411))
        [Case]("StrConv Hiragana", Function() StrConv("カタカナ", VbStrConv.Hiragana, &H411))
        [Case]("StrConv ProperCase", Function() StrConv("hello wORLD", VbStrConv.ProperCase))
        [Case]("Like", Function() ("aBc" Like "a?c") & " " & ("abc" Like "[a-c]*") & " " & ("A1" Like "[!0-9]#") & " " & ("ア" Like "[ア-オ]"))
        [Case]("Option Compare", Function() ("a" = "A") & " " & ("a" < "B") & " " & String.Compare("a", "B", True))
        [Case]("Filter", Function() Join(Filter(New String() {"apple", "banana", "cherry"}, "an"), ","))
        [Case]("Format string", Function() Format("abc", ">") & Format("ABC", "<") & Format("12345", "@@@-@@"))

        Section("dates")
        Dim day As New DateTime(2026, 3, 4, 13, 5, 9)
        [Case]("DateAdd", Function() DateAdd(DateInterval.Month, 1, New DateTime(2026, 1, 31)).ToString("s"))
        [Case]("DateAdd string", Function() DateAdd("ww", 2, day).ToString("s"))
        [Case]("DateDiff", Function() DateDiff(DateInterval.Day, New DateTime(2026, 1, 1), day) & " " & DateDiff("m", New DateTime(2025, 12, 31), New DateTime(2026, 1, 1)) & " " & DateDiff(DateInterval.WeekOfYear, New DateTime(2026, 1, 1), day))
        [Case]("DatePart", Function() DatePart(DateInterval.Weekday, day) & " " & DatePart("ww", day) & " " & DatePart(DateInterval.Quarter, day) & " " & DatePart("y", day))
        [Case]("DatePart week FirstFourDays", Function() DatePart(DateInterval.WeekOfYear, New DateTime(2027, 1, 1), Microsoft.VisualBasic.FirstDayOfWeek.Monday, Microsoft.VisualBasic.FirstWeekOfYear.FirstFourDays))
        [Case]("Weekday/Names", Function() Weekday(day) & " " & WeekdayName(Weekday(day)) & " " & MonthName(3) & " " & MonthName(3, True))
        [Case]("DateSerial", Function() DateSerial(2026, 14, 0).ToString("s"))
        [Case]("TimeSerial", Function() TimeSerial(25, 61, 0).ToString("s"))
        [Case]("DateValue/TimeValue", Function() DateValue("2026/3/4 13:05").ToString("s") & " " & TimeValue("1:05 PM").ToString("s"))
        [Case]("Year/Month/Day/Hour", Function() Year(day) & "/" & Month(day) & "/" & Microsoft.VisualBasic.DateAndTime.Day(day) & " " & Hour(day) & ":" & Minute(day) & ":" & Second(day))

        Section("math and random")
        [Case]("Fix/Int", Function() Fix(-2.5) & " " & Int(-2.5) & " " & Fix(2.7) & " " & Int(2.7))
        [Case]("CInt vs Math.Round", Function() CInt(0.5) & " " & CInt(1.5) & " " & Math.Round(2.5))
        [Case]("\ and Mod", Function() (7 \ 2) & " " & (-7 \ 2) & " " & (-7 Mod 3) & " " & (5.5 Mod 2))
        [Case]("^", Function() (2 ^ 10) & " " & (2 ^ 0.5))
        [Case]("Rnd seeded", Function()
                                 Rnd(-1)
                                 Randomize(42)
                                 Return Rnd() & " " & Rnd() & " " & CInt(Int(100 * Rnd()))
                             End Function)
        [Case]("IIf/Choose/Switch", Function() IIf(True, "y", "n") & " " & Choose(2, "a", "b", "c") & " " & Switch(False, "x", True, "z"))
        [Case]("overflow", Function()
                               Dim i As Integer = Integer.MaxValue
                               Return i + 1
                           End Function)
        [Case]("divide by zero", Function()
                                     Dim z As Integer = 0
                                     Return 1 \ z
                                 End Function)
        [Case]("double divide by zero", Function()
                                            Dim z As Double = 0
                                            Return 1 / z
                                        End Function)

        Section("late binding (Option Strict Off)")
        Dim o As Object = "12"
        [Case]("""12"" + 3", Function() o + 3)
        [Case]("""12"" & 3", Function() o & 3)
        [Case]("""1.5"" * 2", Function() "1.5" * 2)
        [Case]("""a"" + 1", Function() "a" + 1)
        [Case]("Nothing + 1", Function() Nothing + 1)
        [Case]("""10"" > 9", Function() "10" > 9)
        [Case]("""10"" > ""9""", Function() "10" > "9")
        [Case]("obj.Length", Function()
                                 Dim x As Object = "abcd"
                                 Return x.Length
                             End Function)
        [Case]("obj.Missing", Function()
                                  Dim x As Object = "abcd"
                                  Return x.NoSuchMember
                              End Function)
        [Case]("CallByName", Function() CallByName("abc", "ToUpper", CallType.Method))
        [Case]("late double", Function()
                                  Dim x As Object = 0.1
                                  Return x + 0.2
                              End Function)

        Section("Err")
        [Case]("Err.Raise", AddressOf ErrRaise)
        [Case]("Err from exception", AddressOf ErrFromException)
        [Case]("Err.Raise custom", AddressOf ErrCustom)
        [Case]("Error(11)", Function() ErrorToString(11))
        [Case]("Error(53)", Function() ErrorToString(53))
    End Sub

    ' On Error is not allowed in a lambda.
    Private Function ErrRaise() As Object
        On Error Resume Next
        Err.Raise(5)
        Return Err.Number & " " & Err.Description
    End Function

    Private Function ErrFromException() As Object
        On Error Resume Next
        Dim a() As Integer = {1}
        Dim x = a(3)
        Return Err.Number & " " & Err.Description & " " & TypeName(Err.GetException())
    End Function

    Private Function ErrCustom() As Object
        On Error Resume Next
        Err.Raise(vbObjectError + 513, "Probe", "custom message")
        Return Err.Number & " " & Err.Source & " " & Err.Description
    End Function
End Class
