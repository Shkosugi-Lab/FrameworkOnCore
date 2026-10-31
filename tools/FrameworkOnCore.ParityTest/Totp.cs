using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FrameworkOnCore.ParityTest;

/// <summary>
/// The time-based codes of ASP.NET Identity 2 (Microsoft.AspNet.Identity.Core's Rfc6238AuthenticationService): what its
/// TOTP token providers (EmailTokenProvider, PhoneNumberTokenProvider) and UserManager.GenerateChangePhoneNumberToken
/// send by e-mail or SMS. An application whose message services send nothing (WingtipToys) never shows them: the
/// scenario makes the same code from the user's security stamp, as the application's own provider checks it.
/// The key is the security stamp in UTF-16, the time step three minutes, the modifier appended to the step number
/// (the token provider's: "Email:" + purpose + ":" + e-mail, "PhoneNumber:" + purpose + ":" + number; the phone
/// number's change: the number).
/// </summary>
public static class Totp
{
    private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly TimeSpan Timestep = TimeSpan.FromMinutes(3);

    public static string Code(string securityStamp, string? modifier, DateTime? utcNow = null)
    {
        var step = (ulong)((utcNow ?? DateTime.UtcNow) - UnixEpoch).Ticks / (ulong)Timestep.Ticks;
        using var hmac = new HMACSHA1(Encoding.Unicode.GetBytes(securityStamp));
        var stepBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long)step));
        var input = modifier == null ? stepBytes : [.. stepBytes, .. Encoding.UTF8.GetBytes(modifier)];
        var hash = hmac.ComputeHash(input);
        var offset = hash[^1] & 0xf;
        var binary = (hash[offset] & 0x7f) << 24 | (hash[offset + 1] & 0xff) << 16 | (hash[offset + 2] & 0xff) << 8 | (hash[offset + 3] & 0xff);
        return (binary % 1000000).ToString("D6", System.Globalization.CultureInfo.InvariantCulture);
    }
}
