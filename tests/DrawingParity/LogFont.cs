using System.Runtime.InteropServices;

namespace FrameworkOnCore.DrawingParity
{
    /// <summary>GDI's LOGFONT, as applications declare it for Font.ToLogFont / Font.FromLogFont (which take it as object).</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class LogFont
    {
        public int lfHeight;
        public int lfWidth;
        public int lfEscapement;
        public int lfOrientation;
        public int lfWeight;
        public byte lfItalic;
        public byte lfUnderline;
        public byte lfStrikeOut;
        public byte lfCharSet;
        public byte lfOutPrecision;
        public byte lfClipPrecision;
        public byte lfQuality;
        public byte lfPitchAndFamily;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string lfFaceName;

        public override string ToString() =>
            $"LogFont height={lfHeight} width={lfWidth} weight={lfWeight} italic={lfItalic} charset={lfCharSet} quality={lfQuality} face={lfFaceName}";
    }
}
