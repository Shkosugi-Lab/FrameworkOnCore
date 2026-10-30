using System;

// App_Code: サーバーがコンパイルする(Shift_JIS、web.config に fileEncoding なし: ANSI コードページで読む)。
public static class Greeting
{
    public const string 表題 = "App_Code の表題(ソ十能)";

    public static string Hello(string 名前)
    {
        return "ようこそ、" + 名前 + " 様。予定表を表示します。";
    }
}
