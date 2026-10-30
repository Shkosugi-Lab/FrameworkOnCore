using System;

namespace ShiftJisProbe
{
    /// <summary>
    /// 古い機能: .NET にない API(System.EnterpriseServices)を使うメンバーがある。変換器はその本体を
    /// PlatformNotSupportedException に置き換え、このファイルを書き戻す(Shift_JIS のまま)。
    /// </summary>
    public static class 古い機能
    {
        // 表示用の文字列(ソ・表・能)
        public const string 説明 = "ソフト表示能力の確認";

        public static string 使える()
        {
            return 説明 + "(十分)";
        }

        // 呼ばれない: トランザクションの中かどうか(COM+)
        public static bool トランザクション中()
        {
            // 予約: 申請表
            return System.EnterpriseServices.ContextUtil.IsInTransaction;
        }

        public static string 後の文字列 = "後ろの行も表示";
    }
}
