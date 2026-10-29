using System.Web.Security.Cryptography;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>CryptoUtil's hashes, .NET's one-shot SHA256 in the port.</summary>
public class CryptoUtilTests
{
    [Fact] // FIPS 180-2's SHA-256 example ("abc"); ComputeSHA256Hash's documentation: the hash of the buffer's segment
    public void The_sha256_hash_of_a_segment()
    {
        const string abc = "BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD";
        Assert.Equal(abc, Convert.ToHexString(CryptoUtil.ComputeSHA256Hash("abc"u8.ToArray())));
        Assert.Equal(abc, Convert.ToHexString(CryptoUtil.ComputeSHA256Hash("xxabcxx"u8.ToArray(), 2, 3)));
    }

    [Fact] // CreatePredictableIV's documentation: SHA256(buffer), then SHA256 of the previous block, as many as the length asks for
    public void A_predictable_iv_is_the_iterated_sha256()
    {
        var buffer = "abc"u8.ToArray();
        var first = System.Security.Cryptography.SHA256.HashData(buffer);
        var iv = CryptoUtil.CreatePredictableIV(buffer, 384);
        Assert.Equal(48, iv.Length);
        Assert.Equal(first, iv.Take(32));
        Assert.Equal(CryptoUtil.CreatePredictableIV(buffer, 128), iv.Take(16));
    }
}
