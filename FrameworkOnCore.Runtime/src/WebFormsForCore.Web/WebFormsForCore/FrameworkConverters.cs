// WebFormsForCore: a value converted from the text ObjectStateFormatter and ResX keep it as, as .NET Framework's converter
// made it. .NET's CultureInfoConverter gives the installed culture of the name (one of CultureInfo.GetCultures', which the
// converter keeps and gives every conversion: one an application changes is changed for the next; the user's overrides
// off), .NET Framework's a new CultureInfo of the name. Found by the round-trip tests (tests/RuntimeUnitTests/RoundTrip).

using System.ComponentModel;
using System.Globalization;

namespace System.Web.Util
{
	internal static class FrameworkConverters
	{
		/// <summary>
		/// Whether .NET Framework had the converter (its types written as text with it): not .NET's VersionConverter (.NET 7),
		/// a Version was serialized (BinaryFormatter).
		/// </summary>
		internal static bool IsFrameworkConverter(TypeConverter converter) => !(converter is VersionConverter);

		internal static object ConvertFromInvariantString(TypeConverter converter, string text)
		{
			if (converter is CultureInfoConverter && text != null)
			{
				string name = text.Trim();
				if (name.Length > 0 && name != "(Default)")
				{
					try { return new CultureInfo(name); }
					catch (CultureNotFoundException) { }
				}
			}
			return converter.ConvertFromInvariantString(text);
		}
	}
}
