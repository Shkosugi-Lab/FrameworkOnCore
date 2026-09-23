using System.Drawing;
using System.Web.UI.WebControls;

namespace DefaultsProbe.Wizard
{
    /// <summary>n2cms の LocationWizard と同型(名前空間と同名のフィールドから呼ばれる型)。</summary>
    public class ProbeLocationWizard
    {
        public int GetLocations()
        {
            return 3;
        }
    }

    /// <summary>
    /// 名前空間 DefaultsProbe.Wizard の中で、フィールド Wizard のメンバーを呼ぶ(n2cms の Wizard ページと同型)。
    /// C# ではフィールドが名前空間より優先されるので、変換で互換層の型 Wizard に書き換えてはいけない。
    /// </summary>
    public class ProbeWizardPage
    {
        protected ProbeLocationWizard Wizard = new ProbeLocationWizard();

        public int CountLocations()
        {
            return Wizard.GetLocations();
        }
    }
}

namespace DefaultsProbe
{
    /// <summary>
    /// コードビハインドで Color を色のプロパティへ代入する(n2cms の EditPassword と同型、連鎖代入を含む)。
    /// 互換層の ForeColor / BackColor は文字列なので、変換で ColorTranslator.ToHtml を通す必要がある。
    /// </summary>
    public static class ProbeColors
    {
        public static void MarkError(Label first, Label second)
        {
            first.ForeColor = second.ForeColor = Color.Red;
            first.BackColor = Color.FromArgb(0x12, 0x34, 0x56);
        }
    }

    /// <summary>
    /// Image 継承のカスタムコントロール(n2cms の ResizedImage と同型)。ImageUrl を持つ基底
    /// (LegacyImage)に写ることと、img が WebForms と同じく自己終了で描かれることの検証用。
    /// </summary>
    public class ProbeImage : Image
    {
        public string CurrentUrl
        {
            get { return ImageUrl; }
        }
    }
}
