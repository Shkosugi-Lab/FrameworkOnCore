//------------------------------------------------------------------------------
// <copyright file="NetFXCryptoService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.Web.Security.Cryptography {
    using System;
#if NETCOREAPP
    using System.Collections.Generic;
#endif
    using System.IO;
    using System.Security.Cryptography;

    /******************************************************************
    * !! WARNING !!                                                  *
    * This class contains cryptographic code. If you make changes to *
    * this class, please have it reviewed by the appropriate people. *
    ******************************************************************/

    // Uses .NET Framework classes to encrypt (SymmetricAlgorithm) and sign (KeyedHashAlgorithm) data.
    //
    // [PROTECT]
    // INPUT: clearData
    // OUTPUT: protectedData
    // ALGORITHM:
    //   protectedData := IV || Enc(Kenc, IV, clearData) || Sign(Kval, IV || Enc(Kenc, IV, clearData))
    //
    // [UNPROTECT]
    // INPUT: protectedData
    // OUTPUT: clearData
    // ALGORITHM:
    //   1) Assume protectedData := IV || Enc(Kenc, IV, clearData) || Sign(Kval, IV || Enc(Kenc, IV, clearData))
    //   2) Validate the signature over the payload and strip it from the end
    //   3) Strip off the IV from the beginning of the payload
    //   4) Decrypt what remains of the payload, and return it as clearData

    internal sealed class NetFXCryptoService : ICryptoService {

        private readonly ICryptoAlgorithmFactory _cryptoAlgorithmFactory;
        private readonly CryptographicKey _encryptionKey;
        private readonly bool _predictableIV;
        private readonly CryptographicKey _validationKey;

        public NetFXCryptoService(ICryptoAlgorithmFactory cryptoAlgorithmFactory, CryptographicKey encryptionKey, CryptographicKey validationKey, bool predictableIV = false) {
            _cryptoAlgorithmFactory = cryptoAlgorithmFactory;
            _encryptionKey = encryptionKey;
            _validationKey = validationKey;
            _predictableIV = predictableIV;
        }

#if NETCOREAPP
        // Port: a page's view state is protected (encrypted and signed) in every request, each time with a new algorithm keyed
        // anew (an AES key's native context made, an HMAC's key padded and hashed, finalizable handles left to the finalizer):
        // a tenth of a small page's CPU. The built-in algorithms (<machineKey>'s AES, 3DES, DES; HMACSHA1/256/384/512) keyed
        // with a key are kept for that key on the thread (an algorithm is not to be used by two threads at once) and used
        // with .NET's one-shot CBC methods: the same IV || Enc || Sign, the same CBC with PKCS7 padding. Keys are the same
        // objects for the same purpose (AspNetCryptoServiceProvider keeps the derived keys); a custom algorithm ("alg:")
        // goes the original way.
        [ThreadStatic] private static Dictionary<(CryptographicKey, bool), IDisposable> _keyedAlgorithms;
        private const int KeyedAlgorithmsKept = 128;

        // Room for this service's two algorithms, made before either is taken: dropping the kept ones (disposed) between
        // taking the one and the other would dispose the one taken.
        private static void MakeRoomForKeyedAlgorithms() {
            var algorithms = _keyedAlgorithms;
            if (algorithms != null && algorithms.Count > KeyedAlgorithmsKept - 2) {
                foreach (IDisposable algorithm in algorithms.Values) {
                    algorithm.Dispose();
                }
                algorithms.Clear();
            }
        }

        private T Keyed<T>(CryptographicKey key, bool encryption, Func<IDisposable> create) where T : class {
            var algorithms = _keyedAlgorithms ??= new Dictionary<(CryptographicKey, bool), IDisposable>();
            if (!algorithms.TryGetValue((key, encryption), out IDisposable kept)) {
                kept = create();
                algorithms[(key, encryption)] = kept;
            }
            return kept as T;
        }

        // The built-in algorithm of the factory's kind, keyed; null for a custom one.
        private SymmetricAlgorithm KeyedEncryptionAlgorithm() {
            return Keyed<SymmetricAlgorithm>(_encryptionKey, true, () => {
                SymmetricAlgorithm algorithm;
                using (SymmetricAlgorithm configured = _cryptoAlgorithmFactory.GetEncryptionAlgorithm()) {
#pragma warning disable SYSLIB0021, CS0618
                    algorithm = configured is AesCryptoServiceProvider ? Aes.Create()
                        : configured is TripleDESCryptoServiceProvider ? TripleDES.Create()
                        : configured is DESCryptoServiceProvider ? DES.Create()
                        : null;
#pragma warning restore SYSLIB0021, CS0618
                }
                if (algorithm == null) {
                    return new NotKept();
                }
                algorithm.Key = _encryptionKey.GetKeyMaterial();
                return algorithm;
            });
        }

        private KeyedHashAlgorithm KeyedValidationAlgorithm() {
            return Keyed<KeyedHashAlgorithm>(_validationKey, false, () => {
                KeyedHashAlgorithm algorithm = _cryptoAlgorithmFactory.GetValidationAlgorithm();
                if (!(algorithm is HMACSHA1 || algorithm is HMACSHA256 || algorithm is HMACSHA384 || algorithm is HMACSHA512) || algorithm.GetType().Assembly != typeof(HMAC).Assembly) {
                    algorithm.Dispose();
                    return new NotKept();
                }
                algorithm.Key = _validationKey.GetKeyMaterial();
                return algorithm;
            });
        }

        // A custom algorithm's mark: none kept, one created each time.
        private sealed class NotKept : IDisposable {
            public void Dispose() { }
        }

        public byte[] Protect(byte[] clearData) {
            MakeRoomForKeyedAlgorithms();
            SymmetricAlgorithm encryptionAlgorithm = KeyedEncryptionAlgorithm();
            KeyedHashAlgorithm signingAlgorithm = KeyedValidationAlgorithm();
            if (encryptionAlgorithm == null || signingAlgorithm == null) {
                return ProtectNew(clearData);
            }
            checked {
                int ivByteCount = encryptionAlgorithm.BlockSize / 8;
                byte[] iv = _predictableIV
                    ? CryptoUtil.CreatePredictableIV(clearData, encryptionAlgorithm.BlockSize)
                    : RandomNumberGenerator.GetBytes(ivByteCount);
                int encryptedByteCount = encryptionAlgorithm.GetCiphertextLengthCbc(clearData.Length, PaddingMode.PKCS7);
                int signatureByteCount = signingAlgorithm.HashSize / 8;

                // protectedData := IV || Enc(Kenc, IV, clearData) || Sign(Kval, IV || Enc(Kenc, IV, clearData))
                byte[] protectedData = new byte[ivByteCount + encryptedByteCount + signatureByteCount];
                Buffer.BlockCopy(iv, 0, protectedData, 0, ivByteCount);
                encryptionAlgorithm.EncryptCbc(clearData, iv, protectedData.AsSpan(ivByteCount, encryptedByteCount), PaddingMode.PKCS7);
                if (!signingAlgorithm.TryComputeHash(protectedData.AsSpan(0, ivByteCount + encryptedByteCount), protectedData.AsSpan(ivByteCount + encryptedByteCount), out int signed) || signed != signatureByteCount) {
                    throw new CryptographicException();
                }
                return protectedData;
            }
        }

        public byte[] Unprotect(byte[] protectedData) {
            MakeRoomForKeyedAlgorithms();
            SymmetricAlgorithm decryptionAlgorithm = KeyedEncryptionAlgorithm();
            KeyedHashAlgorithm validationAlgorithm = KeyedValidationAlgorithm();
            if (decryptionAlgorithm == null || validationAlgorithm == null) {
                return UnprotectNew(protectedData);
            }
            checked {
                int ivByteCount = decryptionAlgorithm.BlockSize / 8;
                int signatureByteCount = validationAlgorithm.HashSize / 8;
                int encryptedPayloadByteCount = protectedData.Length - ivByteCount - signatureByteCount;
                if (encryptedPayloadByteCount <= 0) {
                    return null;
                }

                byte[] computedSignature = new byte[signatureByteCount];
                if (!validationAlgorithm.TryComputeHash(protectedData.AsSpan(0, ivByteCount + encryptedPayloadByteCount), computedSignature, out int signed) || signed != signatureByteCount) {
                    throw new CryptographicException();
                }
                if (!CryptoUtil.BuffersAreEqual(
                    buffer1: protectedData, buffer1Offset: ivByteCount + encryptedPayloadByteCount, buffer1Count: signatureByteCount,
                    buffer2: computedSignature, buffer2Offset: 0, buffer2Count: computedSignature.Length)) {
                    return null;
                }

                return decryptionAlgorithm.DecryptCbc(protectedData.AsSpan(ivByteCount, encryptedPayloadByteCount), protectedData.AsSpan(0, ivByteCount), PaddingMode.PKCS7);
            }
        }

        private byte[] ProtectNew(byte[] clearData) {
#else
        public byte[] Protect(byte[] clearData) {
#endif
            // The entire operation is wrapped in a 'checked' block because any overflows should be treated as failures.
            checked {

                // These SymmetricAlgorithm instances are single-use; we wrap it in a 'using' block.
                using (SymmetricAlgorithm encryptionAlgorithm = _cryptoAlgorithmFactory.GetEncryptionAlgorithm()) {
                    // Initialize the algorithm with the specified key and an appropriate IV
                    encryptionAlgorithm.Key = _encryptionKey.GetKeyMaterial();

                    if (_predictableIV) {
                        // The caller wanted the output to be predictable (e.g. for caching), so we'll create an
                        // appropriate IV directly from the input buffer. The IV length is equal to the block size.
                        encryptionAlgorithm.IV = CryptoUtil.CreatePredictableIV(clearData, encryptionAlgorithm.BlockSize);
                    }
                    else {
                        // If the caller didn't ask for a predictable IV, just let the algorithm itself choose one.
                        encryptionAlgorithm.GenerateIV();
                    }
                    byte[] iv = encryptionAlgorithm.IV;

                    using (MemoryStream memStream = new MemoryStream()) {
                        memStream.Write(iv, 0, iv.Length);

                        // At this point:
                        // memStream := IV

                        // Write the encrypted payload to the memory stream.
                        using (ICryptoTransform encryptor = encryptionAlgorithm.CreateEncryptor()) {
                            using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write)) {
                                cryptoStream.Write(clearData, 0, clearData.Length);
                                cryptoStream.FlushFinalBlock();

                                // At this point:
                                // memStream := IV || Enc(Kenc, IV, clearData)

                                // These KeyedHashAlgorithm instances are single-use; we wrap it in a 'using' block.
                                using (KeyedHashAlgorithm signingAlgorithm = _cryptoAlgorithmFactory.GetValidationAlgorithm()) {
                                    // Initialize the algorithm with the specified key
                                    signingAlgorithm.Key = _validationKey.GetKeyMaterial();

                                    // Compute the signature
                                    byte[] signature = signingAlgorithm.ComputeHash(memStream.GetBuffer(), 0, (int)memStream.Length);

                                    // At this point:
                                    // memStream := IV || Enc(Kenc, IV, clearData)
                                    // signature := Sign(Kval, IV || Enc(Kenc, IV, clearData))

                                    // Append the signature to the encrypted payload
                                    memStream.Write(signature, 0, signature.Length);

                                    // At this point:
                                    // memStream := IV || Enc(Kenc, IV, clearData) || Sign(Kval, IV || Enc(Kenc, IV, clearData))

                                    // Algorithm complete
                                    byte[] protectedData = memStream.ToArray();
                                    return protectedData;
                                }
                            }
                        }
                    }
                }
            }
        }

#if NETCOREAPP
        private byte[] UnprotectNew(byte[] protectedData) {
#else
        public byte[] Unprotect(byte[] protectedData) {
#endif
            // The entire operation is wrapped in a 'checked' block because any overflows should be treated as failures.
            checked {

                // We want to check that the input is in the form:
                // protectedData := IV || Enc(Kenc, IV, clearData) || Sign(Kval, IV || Enc(Kenc, IV, clearData))

                // Definitions used in this method:
                // encryptedPayload := Enc(Kenc, IV, clearData)
                // signature := Sign(Kval, IV || encryptedPayload)

                // These SymmetricAlgorithm instances are single-use; we wrap it in a 'using' block.
                using (SymmetricAlgorithm decryptionAlgorithm = _cryptoAlgorithmFactory.GetEncryptionAlgorithm()) {
                    decryptionAlgorithm.Key = _encryptionKey.GetKeyMaterial();

                    // These KeyedHashAlgorithm instances are single-use; we wrap it in a 'using' block.
                    using (KeyedHashAlgorithm validationAlgorithm = _cryptoAlgorithmFactory.GetValidationAlgorithm()) {
                        validationAlgorithm.Key = _validationKey.GetKeyMaterial();

                        // First, we need to verify that protectedData is even long enough to contain
                        // the required components (IV, encryptedPayload, signature).

                        int ivByteCount = decryptionAlgorithm.BlockSize / 8; // IV length is equal to the block size
                        int signatureByteCount = validationAlgorithm.HashSize / 8;
                        int encryptedPayloadByteCount = protectedData.Length - ivByteCount - signatureByteCount;
                        if (encryptedPayloadByteCount <= 0) {
                            // protectedData doesn't meet minimum length requirements
                            return null;
                        }

                        // If that check passes, we need to detect payload tampering.

                        // Compute the signature over the IV and encrypted payload
                        // computedSignature := Sign(Kval, IV || encryptedPayload)
                        byte[] computedSignature = validationAlgorithm.ComputeHash(protectedData, 0, ivByteCount + encryptedPayloadByteCount);

                        if (!CryptoUtil.BuffersAreEqual(
                            buffer1: protectedData, buffer1Offset: ivByteCount + encryptedPayloadByteCount, buffer1Count: signatureByteCount,
                            buffer2: computedSignature, buffer2Offset: 0, buffer2Count: computedSignature.Length)) {

                            // the computed signature didn't match the incoming signature, which is a sign of payload tampering
                            return null;
                        }

                        // At this point, we're certain that we generated the signature over this payload,
                        // so we can go ahead with decryption.

                        // Populate the IV from the incoming stream
                        byte[] iv = new byte[ivByteCount];
                        Buffer.BlockCopy(protectedData, 0, iv, 0, iv.Length);
                        decryptionAlgorithm.IV = iv;

                        // Write the decrypted payload to the memory stream.
                        using (MemoryStream memStream = new MemoryStream()) {
                            using (ICryptoTransform decryptor = decryptionAlgorithm.CreateDecryptor()) {
                                using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Write)) {
                                    cryptoStream.Write(protectedData, ivByteCount, encryptedPayloadByteCount);
                                    cryptoStream.FlushFinalBlock();

                                    // At this point
                                    // memStream := clearData

                                    byte[] clearData = memStream.ToArray();
                                    return clearData;
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
