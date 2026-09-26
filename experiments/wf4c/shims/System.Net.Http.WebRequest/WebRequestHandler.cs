using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace System.Net.Http
{
    /// <summary>
    /// .NET Framework's WebRequestHandler: an HttpClientHandler with the HttpWebRequest-era
    /// settings. On .NET the handler underneath is SocketsHttpHandler; the settings it has an
    /// equivalent for are passed through, the rest are kept so code that sets them runs.
    /// </summary>
    public class WebRequestHandler : HttpClientHandler
    {
        private RemoteCertificateValidationCallback _serverCertificateValidationCallback;

        public bool AllowPipelining { get; set; } = true;

        public AuthenticationLevel AuthenticationLevel { get; set; } = AuthenticationLevel.MutualAuthRequested;

        public RequestCachePolicy CachePolicy { get; set; }

        public TokenImpersonationLevel ImpersonationLevel { get; set; } = TokenImpersonationLevel.Delegation;

        public int MaxResponseHeadersLength { get; set; } = 64;

        public int ReadWriteTimeout { get; set; } = 300000;

        public TimeSpan ContinueTimeout { get; set; } = TimeSpan.FromMilliseconds(350);

        public bool UnsafeAuthenticatedConnectionSharing { get; set; }

        /// <summary>Mapped onto HttpClientHandler.ServerCertificateCustomValidationCallback.</summary>
        public RemoteCertificateValidationCallback ServerCertificateValidationCallback
        {
            get => _serverCertificateValidationCallback;
            set
            {
                _serverCertificateValidationCallback = value;
                ServerCertificateCustomValidationCallback = value is null
                    ? null
                    : (request, certificate, chain, errors) => value(request, certificate, chain, errors);
            }
        }
    }
}
