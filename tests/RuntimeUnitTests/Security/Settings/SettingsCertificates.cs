using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// The settings tests' certificates (SettingsProbes' HTTPS probes), made afresh for each run, self-signed: the server's (IIS's
/// binding, Kestrel's endpoint), a client's the server trusts (IIS: in the machine's trusted roots while recording; the
/// port: the host's trusted client certificates, AttackServer) and a client's it does not. Their subjects are what the
/// answers show; their keys, serial numbers and dates are not.
/// </summary>
static class SettingsCertificates
{
    public const string ServerSubject = "CN=foc-settings-server";
    public const string TrustedSubject = "CN=foc-settings-client";
    public const string UntrustedSubject = "CN=foc-settings-untrusted";
    public const string Password = "foc-settings";

    static readonly Lazy<X509Certificate2> trusted = new(() => Make(TrustedSubject, server: false));
    static readonly Lazy<X509Certificate2> untrusted = new(() => Make(UntrustedSubject, server: false));

    public static X509Certificate2 Trusted => trusted.Value;
    public static X509Certificate2 Untrusted => untrusted.Value;

    /// <summary>A self-signed certificate with its private key (RSA 2048, SHA-256), for a server (127.0.0.1) or a client.</summary>
    public static X509Certificate2 Make(string subject, bool server, X509KeyStorageFlags flags = X509KeyStorageFlags.Exportable)
    {
        using var key = RSA.Create(2048);
        var request = new CertificateRequest(subject, key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, false));
        request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid(server ? "1.3.6.1.5.5.7.3.1" : "1.3.6.1.5.5.7.3.2") }, false));
        if (server)
        {
            var names = new SubjectAlternativeNameBuilder();
            names.AddIpAddress(IPAddress.Loopback);
            names.AddDnsName("localhost");
            request.CertificateExtensions.Add(names.Build());
        }
        using var made = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(1));
        // (as a PFX: Windows' TLS takes no ephemeral key)
        return X509CertificateLoader.LoadPkcs12(made.Export(X509ContentType.Pfx, Password), Password, flags);
    }

    /// <summary>The certificate written as a PFX (the port's server's) or its public part (.cer: the client certificate it trusts).</summary>
    public static void Write(X509Certificate2 certificate, string file, bool withKey) =>
        File.WriteAllBytes(file, withKey ? certificate.Export(X509ContentType.Pfx, Password) : certificate.Export(X509ContentType.Cert));
}
