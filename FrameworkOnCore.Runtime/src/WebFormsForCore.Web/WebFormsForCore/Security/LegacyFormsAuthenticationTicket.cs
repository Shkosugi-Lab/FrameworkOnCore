// FrameworkOnCore: the forms authentication ticket in its legacy format (appSettings
// aspnet:UseLegacyFormsAuthenticationTicketCompatibility = true, as applications sharing their cookie with older ones
// set it), which .NET Framework writes and reads with webengine4.dll's CookieAuthConstructTicket and
// CookieAuthParseTicket. IIS' DLL is not on .NET: a login failed with DllNotFoundException. The same bytes and the same
// results (measured against .NET Framework 4.8's on 20,000 tickets of every field, cut ones, small buffers):
//   [8 bytes the caller gives][version][name, UTF-16 with NUL][issue date, 8 bytes][persistent][expiration, 8 bytes]
//   [user data, UTF-16 with NUL][cookie path, UTF-16 with NUL]
// the dates as FILETIMEs, little-endian. A null argument, a buffer length of 0 to read into or below 18 to write into:
// E_INVALIDARG; too little room, a field cut short, a string longer than its buffer: E_UNEXPECTED.

using System.Text;

namespace System.Web.Security
{
    internal static class LegacyFormsAuthenticationTicket
    {
        const int E_UNEXPECTED = unchecked((int)0x8000FFFF);
        const int E_INVALIDARG = unchecked((int)0x80070057);
        const int Prefix = 8;

        /// <summary>CookieAuthConstructTicket: the ticket written after pData's first 8 bytes; its length, or an error.</summary>
        internal static int Construct(byte[] pData, int iDataLen, string szName, string szData, string szPath, byte[] pBytes, long[] pDates)
        {
            // Below 18 bytes (the prefix, the version, the persistence and a date) the native function rejects the buffer as
            // an argument, whatever the strings.
            if (pData == null || szName == null || szData == null || szPath == null || iDataLen < 18)
                return E_INVALIDARG;
            int length = Prefix + 1 + StringSize(szName) + 8 + 1 + 8 + StringSize(szData) + StringSize(szPath);
            if (length > iDataLen || length > pData.Length)
                return E_UNEXPECTED;

            int at = Prefix;
            pData[at++] = pBytes[0];
            at = PutString(pData, at, szName);
            at = PutInt64(pData, at, pDates[0]);
            pData[at++] = pBytes[1];
            at = PutInt64(pData, at, pDates[1]);
            at = PutString(pData, at, szData);
            at = PutString(pData, at, szPath);
            return at;
        }

        /// <summary>CookieAuthParseTicket: 0, or an error.</summary>
        internal static int Parse(byte[] pData, int iDataLen, StringBuilder szName, int iNameLen, StringBuilder szData, int iUserDataLen,
                                  StringBuilder szPath, int iPathLen, byte[] pBytes, long[] pDates)
        {
            if (pData == null || iNameLen <= 0 || iUserDataLen <= 0 || iPathLen <= 0)
                return E_INVALIDARG;
            if (iDataLen > pData.Length)
                return E_UNEXPECTED;
            int at = Prefix;
            if (at + 1 > iDataLen)
                return E_UNEXPECTED;
            byte version = pData[at++];
            string name, userData, path;
            if (!GetString(pData, iDataLen, ref at, iNameLen, out name) || at + 8 + 1 + 8 > iDataLen)
                return E_UNEXPECTED;
            long issued = GetInt64(pData, at);
            at += 8;
            byte persistent = pData[at++];
            long expires = GetInt64(pData, at);
            at += 8;
            if (!GetString(pData, iDataLen, ref at, iUserDataLen, out userData) || !GetString(pData, iDataLen, ref at, iPathLen, out path))
                return E_UNEXPECTED;

            pBytes[0] = version;
            pBytes[1] = persistent;
            pDates[0] = issued;
            pDates[1] = expires;
            szName.Append(name);
            szData.Append(userData);
            szPath.Append(path);
            return 0;
        }

        static int StringSize(string s) => (s.Length + 1) * 2;

        static int PutString(byte[] buffer, int at, string s)
        {
            foreach (char c in s)
            {
                buffer[at++] = (byte)c;
                buffer[at++] = (byte)(c >> 8);
            }
            buffer[at++] = 0;
            buffer[at++] = 0;
            return at;
        }

        // A NUL-terminated UTF-16 string from at; its characters and the NUL must fit the caller's buffer.
        static bool GetString(byte[] buffer, int length, ref int at, int bufferLength, out string s)
        {
            s = null;
            var text = new StringBuilder();
            while (true)
            {
                if (at + 2 > length)
                    return false;
                char c = (char)(buffer[at] | (buffer[at + 1] << 8));
                at += 2;
                if (c == '\0')
                    break;
                text.Append(c);
            }
            if (text.Length + 1 > bufferLength)
                return false;
            s = text.ToString();
            return true;
        }

        static int PutInt64(byte[] buffer, int at, long value)
        {
            for (int i = 0; i < 8; i++)
                buffer[at++] = (byte)(value >> (8 * i));
            return at;
        }

        static long GetInt64(byte[] buffer, int at)
        {
            long value = 0;
            for (int i = 7; i >= 0; i--)
                value = (value << 8) | buffer[at + i];
            return value;
        }
    }
}
