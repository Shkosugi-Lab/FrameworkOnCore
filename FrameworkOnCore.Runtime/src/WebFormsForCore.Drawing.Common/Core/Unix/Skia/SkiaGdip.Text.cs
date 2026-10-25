// WebFormsForCore: GDI+'s StringFormat (as libgdiplus' stringformat.c) and its text layout over Skia: lines broken at spaces
// (a word too long for a line at a character), aligned in the layout rectangle, a sixth of an em of space before and after
// a line unless the format is the typographic one, lines one family line spacing apart, the baseline at the cell ascent,
// trimming of what does not fit, hotkey prefixes, tabs. Glyphs a font lacks come from the font fontconfig finds for them.
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Text;
using SkiaSharp;

namespace System.Drawing
{
    internal static partial class SafeNativeMethods
    {
        internal static unsafe partial class SkiaGdip
        {
            internal sealed class GpStringFormat
            {
                public StringAlignment Alignment = StringAlignment.Near;
                public StringAlignment LineAlignment = StringAlignment.Near;
                public HotkeyPrefix HotkeyPrefix = HotkeyPrefix.None;
                public StringFormatFlags Flags;
                public StringTrimming Trimming = StringTrimming.Character;
                public StringDigitSubstitute Substitute = StringDigitSubstitute.User;
                public int Language;
                public float FirstTabOffset;
                public float[] TabStops = Array.Empty<float>();
                public CharacterRange[] CharacterRanges = Array.Empty<CharacterRange>();
                /// <summary>GenericTypographic's (and its clones'): no space before and after the lines.</summary>
                public bool Typographic;
                public bool Generic;

                public GpStringFormat Clone()
                {
                    var f = (GpStringFormat)MemberwiseClone();
                    f.TabStops = (float[])TabStops.Clone();
                    f.CharacterRanges = (CharacterRange[])CharacterRanges.Clone();
                    f.Generic = false;
                    return f;
                }
            }

            private static readonly GpStringFormat s_genericDefault = new GpStringFormat { Generic = true };
            private static readonly GpStringFormat s_genericTypographic = new GpStringFormat
            {
                Flags = StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit | StringFormatFlags.NoClip,
                Trimming = StringTrimming.None,
                Typographic = true,
                Generic = true,
            };
            private static IntPtr s_genericDefaultHandle, s_genericTypographicHandle;

            internal static int GdipCreateStringFormat(StringFormatFlags options, int language, out IntPtr format)
            {
                format = NewHandle(new GpStringFormat { Flags = options });   // the language is not the digits' (GDI+: 0)
                return Ok;
            }

            internal static int GdipStringFormatGetGenericDefault(out IntPtr format)
            {
                format = s_genericDefaultHandle != IntPtr.Zero ? s_genericDefaultHandle : (s_genericDefaultHandle = NewHandle(s_genericDefault));
                return Ok;
            }

            internal static int GdipStringFormatGetGenericTypographic(out IntPtr format)
            {
                format = s_genericTypographicHandle != IntPtr.Zero ? s_genericTypographicHandle : (s_genericTypographicHandle = NewHandle(s_genericTypographic));
                return Ok;
            }

            internal static int GdipCloneStringFormat(HandleRef format, out IntPtr newFormat)
            {
                newFormat = IntPtr.Zero;
                if (!Is(format, out GpStringFormat? f))
                    return InvalidParameter;
                newFormat = NewHandle(f.Clone());
                return Ok;
            }

            internal static int GdipDeleteStringFormat(HandleRef format)
            {
                if (!Is(format, out GpStringFormat? f))
                    return InvalidParameter;
                if (f.Generic)
                    return Ok;   // singletons
                return Delete<GpStringFormat>(format.Handle);
            }

            internal static int GdipSetStringFormatAlign(HandleRef format, StringAlignment align)
            {
                if (!Is(format, out GpStringFormat? f) || align < StringAlignment.Near || align > StringAlignment.Far)
                    return InvalidParameter;
                f.Alignment = align;
                return Ok;
            }

            internal static int GdipGetStringFormatAlign(HandleRef format, out StringAlignment align)
            {
                align = 0;
                if (!Is(format, out GpStringFormat? f))
                    return InvalidParameter;
                align = f.Alignment;
                return Ok;
            }

            internal static int GdipSetStringFormatLineAlign(HandleRef format, StringAlignment align)
            {
                if (!Is(format, out GpStringFormat? f) || align < StringAlignment.Near || align > StringAlignment.Far)
                    return InvalidParameter;
                f.LineAlignment = align;
                return Ok;
            }

            internal static int GdipGetStringFormatLineAlign(HandleRef format, out StringAlignment align)
            {
                align = 0;
                if (!Is(format, out GpStringFormat? f))
                    return InvalidParameter;
                align = f.LineAlignment;
                return Ok;
            }

            internal static int GdipSetStringFormatHotkeyPrefix(HandleRef format, HotkeyPrefix hotkeyPrefix)
            {
                if (!Is(format, out GpStringFormat? f) || hotkeyPrefix < HotkeyPrefix.None || hotkeyPrefix > HotkeyPrefix.Hide)
                    return InvalidParameter;
                f.HotkeyPrefix = hotkeyPrefix;
                return Ok;
            }

            internal static int GdipGetStringFormatHotkeyPrefix(HandleRef format, out HotkeyPrefix hotkeyPrefix)
            {
                hotkeyPrefix = 0;
                if (!Is(format, out GpStringFormat? f))
                    return InvalidParameter;
                hotkeyPrefix = f.HotkeyPrefix;
                return Ok;
            }

            internal static int GdipSetStringFormatFlags(HandleRef format, StringFormatFlags options)
            {
                if (!Is(format, out GpStringFormat? f))
                    return InvalidParameter;
                f.Flags = options;
                return Ok;
            }

            internal static int GdipGetStringFormatFlags(HandleRef format, out StringFormatFlags result)
            {
                result = 0;
                if (!Is(format, out GpStringFormat? f))
                    return InvalidParameter;
                result = f.Flags;
                return Ok;
            }

            internal static int GdipSetStringFormatTrimming(HandleRef format, StringTrimming trimming)
            {
                if (!Is(format, out GpStringFormat? f) || trimming < StringTrimming.None || trimming > StringTrimming.EllipsisPath)
                    return InvalidParameter;
                f.Trimming = trimming;
                return Ok;
            }

            internal static int GdipGetStringFormatTrimming(HandleRef format, out StringTrimming trimming)
            {
                trimming = 0;
                if (!Is(format, out GpStringFormat? f))
                    return InvalidParameter;
                trimming = f.Trimming;
                return Ok;
            }

            internal static int GdipSetStringFormatTabStops(HandleRef format, float firstTabOffset, int count, float[] tabStops)
            {
                if (!Is(format, out GpStringFormat? f) || tabStops == null)
                    return InvalidParameter;
                if (count <= 0)
                    return Ok;
                if (firstTabOffset < 0)
                    return NotImplemented;
                for (int i = 0; i < count; i++)
                    if (tabStops[i] < 0)
                        return NotImplemented;
                f.FirstTabOffset = firstTabOffset;
                f.TabStops = tabStops.AsSpan(0, count).ToArray();
                return Ok;
            }

            internal static int GdipGetStringFormatTabStopCount(HandleRef format, out int count)
            {
                count = 0;
                if (!Is(format, out GpStringFormat? f))
                    return InvalidParameter;
                count = f.TabStops.Length;
                return Ok;
            }

            internal static int GdipGetStringFormatTabStops(HandleRef format, int count, out float firstTabOffset, float[] tabStops)
            {
                firstTabOffset = 0;
                if (!Is(format, out GpStringFormat? f) || tabStops == null || count < 0)
                    return InvalidParameter;
                firstTabOffset = f.FirstTabOffset;
                Array.Copy(f.TabStops, tabStops, Math.Min(count, f.TabStops.Length));
                return Ok;
            }

            internal static int GdipGetStringFormatDigitSubstitution(HandleRef format, out int langID, out StringDigitSubstitute sds)
            {
                langID = 0;
                sds = 0;
                if (!Is(format, out GpStringFormat? f))
                    return InvalidParameter;
                langID = f.Language;
                sds = f.Substitute;
                return Ok;
            }

            internal static int GdipSetStringFormatDigitSubstitution(HandleRef format, int langID, StringDigitSubstitute sds)
            {
                if (!Is(format, out GpStringFormat? f))
                    return InvalidParameter;
                f.Language = langID;
                f.Substitute = sds;
                return Ok;
            }

            internal static int GdipGetStringFormatMeasurableCharacterRangeCount(HandleRef format, out int count)
            {
                count = 0;
                if (!Is(format, out GpStringFormat? f))
                    return InvalidParameter;
                count = f.CharacterRanges.Length;
                return Ok;
            }

            internal static int GdipSetStringFormatMeasurableCharacterRanges(HandleRef format, int rangeCount, CharacterRange[] range)
            {
                if (!Is(format, out GpStringFormat? f) || range == null)
                    return InvalidParameter;
                if (rangeCount > 32)
                    return Gdip.ValueOverflow;
                f.CharacterRanges = rangeCount > 0 ? range.AsSpan(0, rangeCount).ToArray() : Array.Empty<CharacterRange>();
                return Ok;
            }

            // Layout

            /// <summary>A character of the text as laid out: where it came from, its width, its font.</summary>
            private struct Cell
            {
                public int Source;          // index in the string given
                public int Length;          // 1, 2 for a surrogate pair
                public float Width;
                public bool Hotkey;         // underlined (HotkeyPrefix.Show)
                public bool Break;          // a line break follows it (\n)
                public bool Tab;
                public SKTypeface Typeface;
            }

            private sealed class Line
            {
                public int Start, End;      // cells [Start, End)
                public float Width;         // without trailing spaces (unless measured)
                public float X, Y;
                public string? Ellipsis;    // trimming's ellipsis, after the cells
                public float EllipsisWidth;
                public int EllipsisAt = -1; // EllipsisPath: the cells before it, then the ellipsis, then those from EllipsisResume
                public int EllipsisResume = -1;
            }

            private sealed class Layout
            {
                public string Text = "";
                public Cell[] Cells = Array.Empty<Cell>();
                public readonly List<Line> Lines = new List<Line>();
                public float EmSize, LineHeight, Ascent, Padding;
                public RectangleF Box;      // the text's box (MeasureString's)
                public int CodepointsFitted;
                public bool Vertical;
            }

            private static bool IsSpace(char c) => c == ' ' || c == '　' || c == ' ';

            private static SKFont NewFont(SKTypeface typeface, float size, GpGraphics? g)
            {
                var font = new SKFont(typeface, size)
                {
                    Subpixel = true,
                    LinearMetrics = true,
                    Hinting = SKFontHinting.None,
                    Edging = g?.TextMode switch
                    {
                        TextRenderingHint.SingleBitPerPixel or TextRenderingHint.SingleBitPerPixelGridFit => SKFontEdging.Alias,
                        _ => SKFontEdging.Antialias,
                    },
                };
                return font;
            }

            /// <summary>The typeface for a code point: the font's, or the one fontconfig gives for it.</summary>
            private static SKTypeface TypefaceFor(GpFont font, SKTypeface primary, int codepoint, Dictionary<int, SKTypeface> fallbacks)
            {
                if (codepoint < 0x80 || primary.ContainsGlyph(codepoint))
                    return primary;
                if (fallbacks.TryGetValue(codepoint, out SKTypeface? t))
                    return t;
                t = SKFontManager.Default.MatchCharacter(primary.FamilyName, primary.FontStyle, null, codepoint) ?? primary;
                fallbacks[codepoint] = t;
                return t;
            }

            private static Layout LayOut(GpGraphics? g, string text, GpFont font, RectangleF rect, GpStringFormat format, float emSize)
            {
                var layout = new Layout { Text = text, EmSize = emSize };
                GpFontFamily.Metrics m = font.Family.FamilyMetrics;
                layout.LineHeight = m.LineSpacing * emSize / m.EmHeight;
                layout.Ascent = m.CellAscent * emSize / m.EmHeight;
                layout.Padding = format.Typographic ? 0 : emSize / 6;
                layout.Vertical = (format.Flags & StringFormatFlags.DirectionVertical) != 0;
                if (layout.Vertical)
                    rect = new RectangleF(rect.Y, rect.X, rect.Height, rect.Width);

                // The cells: hotkey prefixes taken out, line breaks and tabs marked.
                SKTypeface primary = font.Typeface;
                var fallbacks = new Dictionary<int, SKTypeface>();
                var cells = new List<Cell>(text.Length);
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    if (c == '\r')
                    {
                        if (cells.Count > 0 && (i + 1 >= text.Length || text[i + 1] != '\n'))
                        {
                            Cell last = cells[^1];
                            last.Break = true;
                            cells[^1] = last;
                        }
                        else if (cells.Count == 0 && (i + 1 >= text.Length || text[i + 1] != '\n'))
                        {
                            cells.Add(new Cell { Source = i, Length = 1, Break = true, Typeface = primary });
                        }
                        continue;
                    }
                    if (c == '\n')
                    {
                        cells.Add(new Cell { Source = i, Length = 1, Break = true, Typeface = primary });
                        continue;
                    }
                    bool hotkey = false;
                    if (c == '&' && format.HotkeyPrefix != HotkeyPrefix.None)
                    {
                        if (i + 1 < text.Length && text[i + 1] == '&')
                        {
                            i++;
                        }
                        else
                        {
                            if (i + 1 >= text.Length)
                                continue;
                            hotkey = format.HotkeyPrefix == HotkeyPrefix.Show;
                            i++;
                            c = text[i];
                        }
                    }
                    int length = char.IsHighSurrogate(c) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]) ? 2 : 1;
                    int codepoint = length == 2 ? char.ConvertToUtf32(c, text[i + 1]) : c;
                    cells.Add(new Cell { Source = i, Length = length, Hotkey = hotkey, Tab = c == '\t', Typeface = TypefaceFor(font, primary, codepoint, fallbacks) });
                    i += length - 1;
                }
                Cell[] cellArray = cells.ToArray();
                layout.Cells = cellArray;

                // Widths: each cell's advance (the runs of a typeface measured together).
                var sb = new StringBuilder();
                for (int i = 0; i < cellArray.Length;)
                {
                    if (cellArray[i].Break && cellArray[i].Source < text.Length && (text[cellArray[i].Source] == '\n' || text[cellArray[i].Source] == '\r'))
                    {
                        i++;
                        continue;
                    }
                    SKTypeface t = cellArray[i].Typeface;
                    int j = i;
                    sb.Clear();
                    while (j < cellArray.Length && cellArray[j].Typeface == t && !(cellArray[j].Break && (text[cellArray[j].Source] == '\n' || text[cellArray[j].Source] == '\r')))
                    {
                        sb.Append(text, cellArray[j].Source, cellArray[j].Length);
                        j++;
                    }
                    using SKFont skFont = NewFont(t, emSize, g);
                    string run = sb.ToString();
                    ushort[] glyphs = skFont.GetGlyphs(run);
                    float[] widths = skFont.GetGlyphWidths(glyphs);
                    for (int k = i, gi = 0; k < j; k++, gi++)
                        cellArray[k].Width = gi < widths.Length ? widths[gi] : 0;
                    i = j;
                }

                // Lines: broken at the line breaks, then (wrapping) at spaces, a word too long at a character.
                bool wrap = (format.Flags & StringFormatFlags.NoWrap) == 0 && rect.Width > 0;
                float available = rect.Width > 0 ? Math.Max(rect.Width - 2 * layout.Padding, 0) : float.MaxValue;
                bool measureTrailing = (format.Flags & StringFormatFlags.MeasureTrailingSpaces) != 0;
                int start = 0;
                for (int i = 0; i <= cellArray.Length; i++)
                {
                    bool hardBreak = i == cellArray.Length || (cellArray[i].Break && (text[cellArray[i].Source] == '\n' || text[cellArray[i].Source] == '\r'));
                    bool breakAfterCell = i < cellArray.Length && cellArray[i].Break && !hardBreak;
                    if (!hardBreak && !breakAfterCell)
                        continue;
                    int end = breakAfterCell ? i + 1 : i;
                    BreakParagraph(layout, cellArray, text, start, end, wrap, available, measureTrailing, format);
                    start = i + 1;
                    if (i == cellArray.Length)
                        break;
                }
                if (layout.Lines.Count == 0)
                    layout.Lines.Add(new Line { Start = 0, End = 0 });

                // Lines that fit in the height (LineLimit: whole lines only), the last one trimmed if text was left out.
                int visible = layout.Lines.Count;
                if (rect.Height > 0)
                {
                    float lines = rect.Height / layout.LineHeight;
                    int fit = (format.Flags & StringFormatFlags.LineLimit) != 0 ? (int)Math.Floor(lines + 0.0001) : (int)Math.Ceiling(lines - 0.0001);
                    visible = Math.Max(0, Math.Min(visible, fit));
                    if ((format.Flags & StringFormatFlags.LineLimit) == 0 && visible == 0 && layout.Lines.Count > 0)
                        visible = 1;
                }
                bool cut = visible < layout.Lines.Count;
                if (cut)
                {
                    int restStart = layout.Lines[visible > 0 ? visible - 1 : 0].Start;
                    layout.Lines.RemoveRange(visible, layout.Lines.Count - visible);
                    if (visible > 0 && format.Trimming != StringTrimming.None)
                    {
                        // The last line: what is left of the text, trimmed to the width.
                        Line last = layout.Lines[^1];
                        int restEnd = cellArray.Length;
                        while (restEnd > restStart && (cellArray[restEnd - 1].Break && (text[cellArray[restEnd - 1].Source] == '\n' || text[cellArray[restEnd - 1].Source] == '\r')))
                            restEnd--;
                        Trim(layout, last, cellArray, text, restStart, last.End, restEnd, available, format, emSize, g, primary, true);
                    }
                }
                else if (!wrap && rect.Width > 0 && format.Trimming != StringTrimming.None)
                {
                    foreach (Line line in layout.Lines)
                        if (line.Width > available + 0.001f)
                            Trim(layout, line, cellArray, text, line.Start, line.End, line.End, available, format, emSize, g, primary, false);
                }
                layout.CodepointsFitted = 0;
                foreach (Line line in layout.Lines)
                    for (int k = line.Start; k < line.End; k++)
                        layout.CodepointsFitted += cellArray[k].Length;
                if (!cut)
                    layout.CodepointsFitted = text.Length;

                // Positions.
                float maxWidth = 0;
                foreach (Line line in layout.Lines)
                    maxWidth = Math.Max(maxWidth, line.Width + line.EllipsisWidth);
                float textHeight = layout.Lines.Count * layout.LineHeight;
                float boxWidth = maxWidth + 2 * layout.Padding;
                if (maxWidth == 0 && layout.CodepointsFitted == 0)
                    boxWidth = 0;
                StringAlignment align = format.Alignment, lineAlign = format.LineAlignment;
                if ((format.Flags & StringFormatFlags.DirectionRightToLeft) != 0)
                    align = align == StringAlignment.Near ? StringAlignment.Far : align == StringAlignment.Far ? StringAlignment.Near : align;
                float top = rect.Y;
                if (rect.Height > 0)
                {
                    if (lineAlign == StringAlignment.Center) top = rect.Y + (rect.Height - textHeight) / 2;
                    else if (lineAlign == StringAlignment.Far) top = rect.Bottom - textHeight;
                }
                else
                {
                    if (lineAlign == StringAlignment.Center) top = rect.Y - textHeight / 2;
                    else if (lineAlign == StringAlignment.Far) top = rect.Y - textHeight;
                }
                for (int i = 0; i < layout.Lines.Count; i++)
                {
                    Line line = layout.Lines[i];
                    float w = line.Width + line.EllipsisWidth;
                    float x;
                    if (rect.Width > 0)
                        x = align switch
                        {
                            StringAlignment.Center => rect.X + (rect.Width - w) / 2,
                            StringAlignment.Far => rect.Right - layout.Padding - w,
                            _ => rect.X + layout.Padding,
                        };
                    else
                        x = align switch
                        {
                            StringAlignment.Center => rect.X - w / 2,
                            StringAlignment.Far => rect.X - layout.Padding - w,
                            _ => rect.X + layout.Padding,
                        };
                    line.X = x;
                    line.Y = top + i * layout.LineHeight;
                }
                float boxX = rect.X, boxY = top;
                if (rect.Width > 0)
                {
                    if (align == StringAlignment.Center) boxX = rect.X + (rect.Width - boxWidth) / 2;
                    else if (align == StringAlignment.Far) boxX = rect.Right - boxWidth;
                    if (boxWidth > rect.Width) { boxWidth = rect.Width; boxX = rect.X; }
                }
                else
                {
                    if (align == StringAlignment.Center) boxX = rect.X - boxWidth / 2;
                    else if (align == StringAlignment.Far) boxX = rect.X - boxWidth;
                }
                float boxHeight = textHeight;
                if (rect.Height > 0 && boxHeight > rect.Height)
                {
                    boxHeight = rect.Height;
                    boxY = rect.Y;
                }
                layout.Box = layout.Vertical ? new RectangleF(boxY, boxX, boxHeight, boxWidth) : new RectangleF(boxX, boxY, boxWidth, boxHeight);
                return layout;
            }

            /// <summary>A paragraph's lines: greedy, at break opportunities (after spaces), a word too long at a character.</summary>
            private static void BreakParagraph(Layout layout, Cell[] cells, string text, int start, int end, bool wrap, float available, bool measureTrailing, GpStringFormat format)
            {
                // The hard break cell itself (\n) is not part of the line.
                int contentEnd = end;
                if (contentEnd > start && cells[contentEnd - 1].Break && (text[cells[contentEnd - 1].Source] == '\n' || text[cells[contentEnd - 1].Source] == '\r'))
                    contentEnd--;
                int lineStart = start;
                while (true)
                {
                    float x = 0;
                    int lastBreak = -1;     // the line may end after this cell
                    int i = lineStart;
                    for (; i < contentEnd; i++)
                    {
                        float width = TabWidth(cells, i, x, format, layout.EmSize);
                        if (wrap && x + width > available + 0.001f && !IsSpace(text[cells[i].Source]) && i > lineStart)
                            break;
                        x += width;
                        if (IsSpace(text[cells[i].Source]) || text[cells[i].Source] == '-')
                            lastBreak = i;
                    }
                    if (i >= contentEnd)
                    {
                        AddLine(layout, cells, text, lineStart, contentEnd, measureTrailing, format);
                        return;
                    }
                    int lineEnd = lastBreak >= lineStart ? lastBreak + 1 : i;   // after the last space, or at a character
                    AddLine(layout, cells, text, lineStart, lineEnd, measureTrailing, format);
                    lineStart = lineEnd;
                    while (lineStart < contentEnd && IsSpace(text[cells[lineStart].Source]) && lineStart > lastBreak)
                        lineStart++;
                    if (lineStart >= contentEnd)
                        return;
                }
            }

            private static float TabWidth(Cell[] cells, int i, float x, GpStringFormat format, float emSize)
            {
                if (!cells[i].Tab)
                    return cells[i].Width;
                float[] stops = format.TabStops;
                float position = format.FirstTabOffset;
                if (stops.Length == 0 || stops[0] <= 0)
                {
                    // No tab stops: every four ems (GDI+'s default).
                    float stop = emSize * 4;
                    return (float)(Math.Floor(x / stop) + 1) * stop - x;
                }
                int index = 0;
                while (position <= x)
                {
                    position += stops[Math.Min(index, stops.Length - 1)];
                    index++;
                    if (index > 10000) break;
                }
                return position - x;
            }

            private static void AddLine(Layout layout, Cell[] cells, string text, int start, int end, bool measureTrailing, GpStringFormat format)
            {
                float width = 0;
                int measuredEnd = end;
                if (!measureTrailing)
                    while (measuredEnd > start && IsSpace(text[cells[measuredEnd - 1].Source]))
                        measuredEnd--;
                for (int i = start; i < measuredEnd; i++)
                    width += TabWidth(cells, i, width, format, layout.EmSize);
                layout.Lines.Add(new Line { Start = start, End = end, Width = width });
            }

            /// <summary>A line trimmed to the width: at a character or a word, with an ellipsis or without.</summary>
            private static void Trim(Layout layout, Line line, Cell[] cells, string text, int start, int lineEnd, int restEnd, float available, GpStringFormat format, float emSize, GpGraphics? g, SKTypeface primary, bool moreText)
            {
                string ellipsis = "…";
                float ellipsisWidth = 0;
                bool withEllipsis = format.Trimming is StringTrimming.EllipsisCharacter or StringTrimming.EllipsisWord or StringTrimming.EllipsisPath;
                if (withEllipsis)
                {
                    using SKFont f = NewFont(primary, emSize, g);
                    ellipsisWidth = f.MeasureText(ellipsis);
                }
                if (!moreText && !withEllipsis && format.Trimming == StringTrimming.Character)
                {
                    // Characters that fit, whole.
                    float x = 0;
                    int k = start;
                    for (; k < lineEnd; k++)
                    {
                        if (x + cells[k].Width > available + 0.001f)
                            break;
                        x += cells[k].Width;
                    }
                    line.End = k;
                    line.Width = x;
                    return;
                }
                bool all = restEnd <= lineEnd;
                if (all && moreText)
                {
                    // Everything left fits on the line: nothing to trim.
                    line.End = restEnd;
                    float w = 0;
                    for (int k = start; k < restEnd; k++) w += cells[k].Width;
                    if (w <= available + 0.001f)
                    {
                        line.Width = w;
                        return;
                    }
                }
                float room = available - (withEllipsis ? ellipsisWidth : 0);
                if (format.Trimming == StringTrimming.EllipsisPath)
                {
                    // The start and the end of the text, the ellipsis between them.
                    float total = 0;
                    for (int k = start; k < restEnd; k++) total += cells[k].Width;
                    if (total <= available)
                    {
                        line.End = restEnd;
                        line.Width = total;
                        return;
                    }
                    int head = start, tail = restEnd;
                    float used = 0;
                    while (tail > head && used + cells[tail - 1].Width <= room / 2) { used += cells[tail - 1].Width; tail--; }
                    while (head < tail && used + cells[head].Width <= room) { used += cells[head].Width; head++; }
                    line.Start = start;
                    line.End = restEnd;
                    line.EllipsisAt = head;
                    line.EllipsisResume = tail;
                    line.Ellipsis = ellipsis;
                    line.EllipsisWidth = ellipsisWidth;
                    line.Width = used;
                    return;
                }
                float xx = 0;
                int end = start, lastWordEnd = -1;
                for (int k = start; k < restEnd; k++)
                {
                    if (xx + cells[k].Width > room + 0.001f)
                        break;
                    xx += cells[k].Width;
                    end = k + 1;
                    if (k + 1 < restEnd && IsSpace(text[cells[k + 1].Source]))
                        lastWordEnd = k + 1;
                }
                if ((format.Trimming == StringTrimming.Word || format.Trimming == StringTrimming.EllipsisWord) && lastWordEnd > start && end < restEnd)
                    end = lastWordEnd;
                float width = 0;
                int measured = end;
                while (measured > start && IsSpace(text[cells[measured - 1].Source])) measured--;
                for (int k = start; k < measured; k++) width += cells[k].Width;
                line.End = end;
                line.Width = width;
                if (withEllipsis && (end < restEnd || moreText && !all))
                {
                    line.Ellipsis = ellipsis;
                    line.EllipsisWidth = ellipsisWidth;
                }
            }

            private static float EmSizeFor(GpGraphics? g, GpFont font) => g != null ? font.WorldSize(g) : font.PixelSize(96);

            private static int CleanLength(string text, int length) => length < 0 ? (text.IndexOf('\0') is int z && z >= 0 ? z : text.Length) : Math.Min(length, text.Length);

            internal static int GdipMeasureString(HandleRef graphics, string textString, int length, HandleRef font, ref RectangleF layoutRect, HandleRef stringFormat, ref RectangleF boundingBox, out int codepointsFitted, out int linesFilled)
            {
                codepointsFitted = linesFilled = 0;
                if (!Is(graphics, out GpGraphics? g) || textString == null || !Is(font, out GpFont? f))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                GpStringFormat format = Get<GpStringFormat>(stringFormat.Handle) ?? s_genericDefault;
                string text = textString.Substring(0, CleanLength(textString, length));
                if (text.Length == 0)
                {
                    boundingBox = new RectangleF(layoutRect.X, layoutRect.Y, 0, 0);
                    return Ok;
                }
                Layout layout = LayOut(g, text, f, layoutRect, format, EmSizeFor(g, f));
                boundingBox = layout.Box;
                codepointsFitted = layout.CodepointsFitted;
                linesFilled = layout.Lines.Count;
                return Ok;
            }

            internal static int GdipDrawString(HandleRef graphics, string textString, int length, HandleRef font, ref RectangleF layoutRect, HandleRef stringFormat, HandleRef brush)
            {
                if (!Is(graphics, out GpGraphics? g) || textString == null || !Is(font, out GpFont? f) || !Is(brush, out GpBrush? b))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                GpStringFormat format = Get<GpStringFormat>(stringFormat.Handle) ?? s_genericDefault;
                string text = textString.Substring(0, CleanLength(textString, length));
                if (text.Length == 0)
                    return Ok;
                float emSize = EmSizeFor(g, f);
                Layout layout = LayOut(g, text, f, layoutRect, format, emSize);
                RectangleF rect = layoutRect;
                SKCanvas canvas = g.Begin();
                try
                {
                    if ((format.Flags & StringFormatFlags.NoClip) == 0 && rect.Width > 0 && rect.Height > 0)
                        canvas.ClipRect(new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom), SKClipOperation.Intersect, g.Antialias);
                    if (layout.Vertical)
                    {
                        // Vertical lines: the layout turned a quarter clockwise around the rectangle's corner.
                        canvas.Translate(rect.X + rect.Width, rect.Y);
                        canvas.RotateDegrees(90);
                        canvas.Translate(-rect.Y, -rect.X);
                    }
                    using SKPaint paint = g.NewPaint(true);
                    int status = b.Setup(paint, g);
                    if (status != Ok)
                        return status;
                    DrawLayout(canvas, layout, f, paint, g, emSize);
                    paint.Shader?.Dispose();
                }
                finally
                {
                    g.End();
                }
                return Ok;
            }

            private static void DrawLayout(SKCanvas canvas, Layout layout, GpFont font, SKPaint paint, GpGraphics g, float emSize)
            {
                using SKFont metricsFont = NewFont(font.Typeface, emSize, g);
                SKFontMetrics metrics = metricsFont.Metrics;
                bool underline = (font.Style & FontStyle.Underline) != 0, strikeout = (font.Style & FontStyle.Strikeout) != 0;
                float underlineY = metrics.UnderlinePosition ?? emSize * 0.1f, thickness = metrics.UnderlineThickness ?? Math.Max(emSize / 14, 1);
                float strikeY = metrics.StrikeoutPosition ?? -emSize * 0.3f, strikeThickness = metrics.StrikeoutThickness ?? thickness;
                var sb = new StringBuilder();
                foreach (Line line in layout.Lines)
                {
                    float baseline = line.Y + layout.Ascent;
                    float x = line.X;
                    void DrawCells(int from, int to)
                    {
                        for (int i = from; i < to;)
                        {
                            Cell cell = layout.Cells[i];
                            char c = layout.Text[cell.Source];
                            if (cell.Tab || (cell.Break && (c == '\n' || c == '\r')))
                            {
                                x += TabWidth(layout.Cells, i, x - line.X, s_genericDefault, layout.EmSize);
                                i++;
                                continue;
                            }
                            SKTypeface t = cell.Typeface;
                            int j = i;
                            sb.Clear();
                            float runWidth = 0;
                            while (j < to && layout.Cells[j].Typeface == t && !layout.Cells[j].Tab)
                            {
                                sb.Append(layout.Text, layout.Cells[j].Source, layout.Cells[j].Length);
                                if (layout.Cells[j].Hotkey)
                                    canvas.DrawRect(x + runWidth, baseline + underlineY, layout.Cells[j].Width, thickness, paint);
                                runWidth += layout.Cells[j].Width;
                                j++;
                            }
                            using SKFont skFont = NewFont(t, emSize, g);
                            using SKTextBlob? blob = SKTextBlob.Create(sb.ToString(), skFont);
                            if (blob != null)
                                canvas.DrawText(blob, x, baseline, paint);
                            x += runWidth;
                            i = j;
                        }
                    }
                    if (line.EllipsisAt >= 0)
                    {
                        DrawCells(line.Start, line.EllipsisAt);
                        canvas.DrawText(line.Ellipsis!, x, baseline, metricsFont, paint);
                        x += line.EllipsisWidth;
                        DrawCells(line.EllipsisResume, line.End);
                    }
                    else
                    {
                        DrawCells(line.Start, line.End);
                        if (line.Ellipsis != null)
                            canvas.DrawText(line.Ellipsis, line.X + line.Width, baseline, metricsFont, paint);
                    }
                    float lineWidth = line.Width + line.EllipsisWidth;
                    if (underline && lineWidth > 0)
                        canvas.DrawRect(line.X, baseline + underlineY, lineWidth, thickness, paint);
                    if (strikeout && lineWidth > 0)
                        canvas.DrawRect(line.X, baseline + strikeY, lineWidth, strikeThickness, paint);
                }
            }

            internal static int GdipMeasureCharacterRanges(HandleRef graphics, string textString, int length, HandleRef font, ref RectangleF layoutRect, HandleRef stringFormat, int characterCount, IntPtr[] region)
            {
                if (!Is(graphics, out GpGraphics? g) || textString == null || !Is(font, out GpFont? f) || region == null)
                    return InvalidParameter;
                GpStringFormat format = Get<GpStringFormat>(stringFormat.Handle) ?? s_genericDefault;
                CharacterRange[] ranges = format.CharacterRanges;
                if (characterCount < ranges.Length)
                    return InvalidParameter;
                string text = textString.Substring(0, CleanLength(textString, length));
                float emSize = EmSizeFor(g, f);
                Layout layout = LayOut(g, text, f, layoutRect, format, emSize);
                for (int r = 0; r < ranges.Length; r++)
                {
                    if (!Is(region[r], out GpRegion? target))
                        return InvalidParameter;
                    int first = ranges[r].First, last = ranges[r].First + ranges[r].Length;
                    if (ranges[r].Length < 0) { first = last; last = ranges[r].First; }
                    if (first < 0 || last > text.Length)
                        return InvalidParameter;
                    GpRegion result = GpRegion.Empty();
                    foreach (Line line in layout.Lines)
                    {
                        float x = line.X;
                        float? left = null, right = null;
                        for (int i = line.Start; i < line.End; i++)
                        {
                            Cell cell = layout.Cells[i];
                            float w = TabWidth(layout.Cells, i, x - line.X, format, layout.EmSize);
                            if (cell.Source >= first && cell.Source < last)
                            {
                                left ??= x;
                                right = x + w;
                            }
                            x += w;
                        }
                        if (left != null)
                        {
                            var rect = new RectangleF(left.Value, line.Y, right!.Value - left.Value, layout.LineHeight);
                            if (layout.Vertical)
                                rect = new RectangleF(rect.Y, rect.X, rect.Height, rect.Width);
                            using GpRegion piece = GpRegion.FromRect(rect);
                            result.Combine(piece, CombineMode.Union);
                        }
                    }
                    target.CopyFrom(result);
                    result.Dispose();
                }
                return Ok;
            }

            internal static int GdipAddPathString(IntPtr path, string s, int lenght, IntPtr family, int style, float emSize, ref RectangleF layoutRect, IntPtr format)
            {
                if (!Is(path, out GpPath? p) || s == null || lenght < -1 || !Is(family, out GpFontFamily? fam))
                    return InvalidParameter;
                string text = s.Substring(0, CleanLength(s, lenght));
                if (text.Length == 0)
                    return Ok;
                if (emSize == 0)
                    return GenericError;
                var font = new GpFont(fam, Math.Abs(emSize), (FontStyle)style, GraphicsUnit.Pixel);
                GpStringFormat stringFormat = Get<GpStringFormat>(format) ?? s_genericDefault;
                Layout layout = LayOut(null, text, font, layoutRect, stringFormat, Math.Abs(emSize));
                using var all = new SKPath();
                var sb = new StringBuilder();
                foreach (Line line in layout.Lines)
                {
                    float x = line.X, baseline = line.Y + layout.Ascent;
                    for (int i = line.Start; i < line.End;)
                    {
                        Cell cell = layout.Cells[i];
                        if (cell.Tab || (cell.Break && (text[cell.Source] == '\n' || text[cell.Source] == '\r')))
                        {
                            x += cell.Width;
                            i++;
                            continue;
                        }
                        int j = i;
                        sb.Clear();
                        float runWidth = 0;
                        while (j < line.End && layout.Cells[j].Typeface == cell.Typeface && !layout.Cells[j].Tab)
                        {
                            sb.Append(text, layout.Cells[j].Source, layout.Cells[j].Length);
                            runWidth += layout.Cells[j].Width;
                            j++;
                        }
                        using SKFont skFont = NewFont(cell.Typeface, Math.Abs(emSize), null);
                        using SKPath glyphs = skFont.GetTextPath(sb.ToString(), new SKPoint(x, baseline));
                        all.AddPath(glyphs);
                        x += runWidth;
                        i = j;
                    }
                }
                GpPath added = GpPath.FromSK(all, p.FillMode);
                p.Points.AddRange(added.Points);
                p.Types.AddRange(added.Types);
                p.StartNewFig = true;
                return Ok;
            }

            internal static int GdipAddPathStringI(IntPtr path, string s, int lenght, IntPtr family, int style, float emSize, ref Rectangle layoutRect, IntPtr format)
            {
                RectangleF rect = layoutRect;
                return GdipAddPathString(path, s, lenght, family, style, emSize, ref rect, format);
            }
        }
    }
}
