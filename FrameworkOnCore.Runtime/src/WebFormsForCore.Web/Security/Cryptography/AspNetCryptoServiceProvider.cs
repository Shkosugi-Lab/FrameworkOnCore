//------------------------------------------------------------------------------
// <copyright file="AspNetCryptoServiceProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.Web.Security.Cryptography {
    using System;
    using System.Web.Configuration;

    // The central ASP.NET class for providing ICryptoService instances.
    // Get an instance of this class via the static Instance property.

    internal sealed class AspNetCryptoServiceProvider : ICryptoServiceProvider {

        private static readonly Lazy<AspNetCryptoServiceProvider> _singleton = new Lazy<AspNetCryptoServiceProvider>(GetSingletonCryptoServiceProvider);

        private readonly ICryptoAlgorithmFactory _cryptoAlgorithmFactory;
        private readonly IDataProtectorFactory _dataProtectorFactory;
        private readonly bool _isDataProtectorEnabled;
        private KeyDerivationFunction _keyDerivationFunction;
        private readonly MachineKeySection _machineKeySection;
        private readonly IMasterKeyProvider _masterKeyProvider;

        // This constructor is used only for testing purposes and by the singleton provider
        // and should not otherwise be called during ASP.NET request processing.
        internal AspNetCryptoServiceProvider(MachineKeySection machineKeySection = null, ICryptoAlgorithmFactory cryptoAlgorithmFactory = null, IMasterKeyProvider masterKeyProvider = null, IDataProtectorFactory dataProtectorFactory = null, KeyDerivationFunction keyDerivationFunction = null) {
            _machineKeySection = machineKeySection;
            _cryptoAlgorithmFactory = cryptoAlgorithmFactory;
            _masterKeyProvider = masterKeyProvider;
            _dataProtectorFactory = dataProtectorFactory;
            _keyDerivationFunction = keyDerivationFunction;

            // This CryptoServiceProvider is active if specified as such in the <system.web/machineKey> section
            IsDefaultProvider = (machineKeySection != null && machineKeySection.CompatibilityMode >= MachineKeyCompatibilityMode.Framework45);

            // The DataProtectorCryptoService is active if specified as such in config
            _isDataProtectorEnabled = (machineKeySection != null && !String.IsNullOrWhiteSpace(machineKeySection.DataProtectorType));
        }

        internal static AspNetCryptoServiceProvider Instance {
            get {
                return _singleton.Value;
            }
        }

        // Returns a value indicating whether this crypto service provider is the default
        // provider for the current application.
        internal bool IsDefaultProvider {
            get;
            private set;
        }

        public ICryptoService GetCryptoService(Purpose purpose, CryptoServiceOptions options = CryptoServiceOptions.None) {
            ICryptoService cryptoService;
            if (_isDataProtectorEnabled && options == CryptoServiceOptions.None) {
                // We can only use DataProtector if it's configured and the caller didn't ask for any special behavior like cacheability
                cryptoService = GetDataProtectorCryptoService(purpose);
            }
            else {
                // Otherwise we fall back to using the <machineKey> algorithms for cryptography
                cryptoService = GetNetFXCryptoService(purpose, options);
            }

            // always homogenize errors returned from the crypto service
            return new HomogenizingCryptoServiceWrapper(cryptoService);
        }

        private DataProtectorCryptoService GetDataProtectorCryptoService(Purpose purpose) {
            // just return the ICryptoService directly
            return new DataProtectorCryptoService(_dataProtectorFactory, purpose);
        }

#if NETCOREAPP
        // Port: a page's view state purpose is made in each request (its page's type, its directory, ViewStateUserKey as
        // specific purposes), its keys derived again each time (SP800-108, HMACSHA512), new key objects each time (the keyed
        // algorithms NetFXCryptoService keeps are kept for a key object). The keys derived for a purpose kept here by its
        // primary and specific purposes, as many as DerivedKeysKept (more, all of them dropped: purposes with a user's key in
        // them are as many as the users).
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, CryptographicKey[]> _derivedKeys = new System.Collections.Concurrent.ConcurrentDictionary<string, CryptographicKey[]>(StringComparer.Ordinal);
        private const int DerivedKeysKept = 1000;

        private CryptographicKey[] GetDerivedKeys(Purpose purpose) {
            var name = new System.Text.StringBuilder(purpose.PrimaryPurpose);
            foreach (string specificPurpose in purpose.SpecificPurposes) {
                name.Append('\0').Append(specificPurpose.Length).Append(':').Append(specificPurpose);
            }
            string key = name.ToString();
            if (_derivedKeys.TryGetValue(key, out CryptographicKey[] keys)) {
                return keys;
            }
            keys = new[] {
                purpose.GetDerivedEncryptionKey(_masterKeyProvider, _keyDerivationFunction),
                purpose.GetDerivedValidationKey(_masterKeyProvider, _keyDerivationFunction),
            };
            if (_derivedKeys.Count >= DerivedKeysKept) {
                _derivedKeys.Clear();
            }
            return _derivedKeys.GetOrAdd(key, keys);
        }
#endif

        private NetFXCryptoService GetNetFXCryptoService(Purpose purpose, CryptoServiceOptions options) {
            // Extract the encryption and validation keys from the provided Purpose object
#if NETCOREAPP
            CryptographicKey encryptionKey, validationKey;
            if (purpose.DerivedEncryptionKey == null && purpose.DerivedValidationKey == null) {
                CryptographicKey[] keys = GetDerivedKeys(purpose);
                encryptionKey = keys[0];
                validationKey = keys[1];
            }
            else {
                encryptionKey = purpose.GetDerivedEncryptionKey(_masterKeyProvider, _keyDerivationFunction);
                validationKey = purpose.GetDerivedValidationKey(_masterKeyProvider, _keyDerivationFunction);
            }
#else
            CryptographicKey encryptionKey = purpose.GetDerivedEncryptionKey(_masterKeyProvider, _keyDerivationFunction);
            CryptographicKey validationKey = purpose.GetDerivedValidationKey(_masterKeyProvider, _keyDerivationFunction);
#endif

            // and return the ICryptoService
            // (predictable IV turned on if the caller requested cacheable output)
            return new NetFXCryptoService(_cryptoAlgorithmFactory, encryptionKey, validationKey, predictableIV: (options == CryptoServiceOptions.CacheableOutput));
        }

        private static AspNetCryptoServiceProvider GetSingletonCryptoServiceProvider() {
            // Provides all of the necessary dependencies for an application-level
            // AspNetCryptoServiceProvider.

            MachineKeySection machineKeySection = MachineKeySection.GetApplicationConfig();

            return new AspNetCryptoServiceProvider(
                machineKeySection: machineKeySection,
                cryptoAlgorithmFactory: new MachineKeyCryptoAlgorithmFactory(machineKeySection),
                masterKeyProvider: new MachineKeyMasterKeyProvider(machineKeySection),
                dataProtectorFactory: new MachineKeyDataProtectorFactory(machineKeySection),
                keyDerivationFunction: SP800_108.DeriveKey);
        }

    }
}
