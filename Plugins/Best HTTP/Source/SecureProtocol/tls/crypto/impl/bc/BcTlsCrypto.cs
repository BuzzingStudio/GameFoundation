#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Tls.Crypto.Impl.BC
{
    using System;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Agreement.Srp;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Digests;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Macs;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Prng;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Math;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

    /**
     * Class for providing cryptographic services for TLS based on implementations in the BC light-weight API.
     * <p>
     *     This class provides default implementations for everything. If you need to customise it, extend the class
     *     and override the appropriate methods.
     * </p>
     */
    public class BcTlsCrypto
        : AbstractTlsCrypto
    {
        public BcTlsCrypto(SecureRandom entropySource) { this.SecureRandom = entropySource; }

        internal virtual BcTlsSecret AdoptLocalSecret(byte[] data) { return new BcTlsSecret(this, data); }

        public override SecureRandom SecureRandom { get; }

        public override TlsCertificate CreateCertificate(byte[] encoding) { return new BcTlsCertificate(this, encoding); }

        public override TlsCipher CreateCipher(TlsCryptoParameters cryptoParams, int encryptionAlgorithm,
            int macAlgorithm)
        {
            switch (encryptionAlgorithm)
            {
                case EncryptionAlgorithm.AES_128_CBC:
                case EncryptionAlgorithm.ARIA_128_CBC:
                case EncryptionAlgorithm.CAMELLIA_128_CBC:
                case EncryptionAlgorithm.SEED_CBC:
                case EncryptionAlgorithm.SM4_CBC:
                    return this.CreateCipher_Cbc(cryptoParams, encryptionAlgorithm, 16, macAlgorithm);

                case EncryptionAlgorithm.cls_3DES_EDE_CBC:
                    return this.CreateCipher_Cbc(cryptoParams, encryptionAlgorithm, 24, macAlgorithm);

                case EncryptionAlgorithm.AES_256_CBC:
                case EncryptionAlgorithm.ARIA_256_CBC:
                case EncryptionAlgorithm.CAMELLIA_256_CBC:
                    return this.CreateCipher_Cbc(cryptoParams, encryptionAlgorithm, 32, macAlgorithm);

                case EncryptionAlgorithm.AES_128_CCM:
                    // NOTE: Ignores macAlgorithm
                    return this.CreateCipher_Aes_Ccm(cryptoParams, 16, 16);
                case EncryptionAlgorithm.AES_128_CCM_8:
                    // NOTE: Ignores macAlgorithm
                    return this.CreateCipher_Aes_Ccm(cryptoParams, 16, 8);
                case EncryptionAlgorithm.AES_128_GCM:
                    // NOTE: Ignores macAlgorithm
                    return this.CreateCipher_Aes_Gcm(cryptoParams, 16, 16);
                case EncryptionAlgorithm.AES_256_CCM:
                    // NOTE: Ignores macAlgorithm
                    return this.CreateCipher_Aes_Ccm(cryptoParams, 32, 16);
                case EncryptionAlgorithm.AES_256_CCM_8:
                    // NOTE: Ignores macAlgorithm
                    return this.CreateCipher_Aes_Ccm(cryptoParams, 32, 8);
                case EncryptionAlgorithm.AES_256_GCM:
                    // NOTE: Ignores macAlgorithm
                    return this.CreateCipher_Aes_Gcm(cryptoParams, 32, 16);
                case EncryptionAlgorithm.ARIA_128_GCM:
                    // NOTE: Ignores macAlgorithm
                    return this.CreateCipher_Aria_Gcm(cryptoParams, 16, 16);
                case EncryptionAlgorithm.ARIA_256_GCM:
                    // NOTE: Ignores macAlgorithm
                    return this.CreateCipher_Aria_Gcm(cryptoParams, 32, 16);
                case EncryptionAlgorithm.CAMELLIA_128_GCM:
                    // NOTE: Ignores macAlgorithm
                    return this.CreateCipher_Camellia_Gcm(cryptoParams, 16, 16);
                case EncryptionAlgorithm.CAMELLIA_256_GCM:
                    // NOTE: Ignores macAlgorithm
                    return this.CreateCipher_Camellia_Gcm(cryptoParams, 32, 16);
                case EncryptionAlgorithm.CHACHA20_POLY1305:
                    // NOTE: Ignores macAlgorithm
                    return this.CreateChaCha20Poly1305(cryptoParams);
                case EncryptionAlgorithm.NULL:
                    return this.CreateNullCipher(cryptoParams, macAlgorithm);
                case EncryptionAlgorithm.SM4_CCM:
                    // NOTE: Ignores macAlgorithm
                    return this.CreateCipher_SM4_Ccm(cryptoParams);
                case EncryptionAlgorithm.SM4_GCM:
                    // NOTE: Ignores macAlgorithm
                    return this.CreateCipher_SM4_Gcm(cryptoParams);

                case EncryptionAlgorithm.DES40_CBC:
                case EncryptionAlgorithm.DES_CBC:
                case EncryptionAlgorithm.IDEA_CBC:
                case EncryptionAlgorithm.RC2_CBC_40:
                case EncryptionAlgorithm.RC4_128:
                case EncryptionAlgorithm.RC4_40:
                default:
                    throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        public override TlsDHDomain CreateDHDomain(TlsDHConfig dhConfig) { return new BcTlsDHDomain(this, dhConfig); }

        public override TlsECDomain CreateECDomain(TlsECConfig ecConfig)
        {
            switch (ecConfig.NamedGroup)
            {
                case NamedGroup.x25519:
                    return new BcX25519Domain(this);
                case NamedGroup.x448:
                    return new BcX448Domain(this);
                default:
                    return new BcTlsECDomain(this, ecConfig);
            }
        }

        public override TlsNonceGenerator CreateNonceGenerator(byte[] additionalSeedMaterial)
        {
            var digest = this.CreateDigest(CryptoHashAlgorithm.sha256);

            var seed = new byte[digest.GetDigestSize()];
            this.SecureRandom.NextBytes(seed);

            var randomGenerator = new DigestRandomGenerator(digest);
            randomGenerator.AddSeedMaterial(additionalSeedMaterial);
            randomGenerator.AddSeedMaterial(seed);

            return new BcTlsNonceGenerator(randomGenerator);
        }

        public override bool HasAllRawSignatureAlgorithms()
        {
            // TODO[RFC 8422] Revisit the need to buffer the handshake for "Intrinsic" hash signatures
            return !this.HasSignatureAlgorithm(SignatureAlgorithm.ed25519)
                   && !this.HasSignatureAlgorithm(SignatureAlgorithm.ed448);
        }

        public override bool HasDHAgreement() { return true; }

        public override bool HasECDHAgreement() { return true; }

        public override bool HasEncryptionAlgorithm(int encryptionAlgorithm)
        {
            switch (encryptionAlgorithm)
            {
                case EncryptionAlgorithm.DES40_CBC:
                case EncryptionAlgorithm.DES_CBC:
                case EncryptionAlgorithm.IDEA_CBC:
                case EncryptionAlgorithm.RC2_CBC_40:
                case EncryptionAlgorithm.RC4_128:
                case EncryptionAlgorithm.RC4_40:
                    return false;

                default:
                    return true;
            }
        }

        public override bool HasCryptoHashAlgorithm(int cryptoHashAlgorithm) { return true; }

        public override bool HasCryptoSignatureAlgorithm(int cryptoSignatureAlgorithm)
        {
            switch (cryptoSignatureAlgorithm)
            {
                case CryptoSignatureAlgorithm.rsa:
                case CryptoSignatureAlgorithm.dsa:
                case CryptoSignatureAlgorithm.ecdsa:
                case CryptoSignatureAlgorithm.rsa_pss_rsae_sha256:
                case CryptoSignatureAlgorithm.rsa_pss_rsae_sha384:
                case CryptoSignatureAlgorithm.rsa_pss_rsae_sha512:
                case CryptoSignatureAlgorithm.ed25519:
                case CryptoSignatureAlgorithm.ed448:
                case CryptoSignatureAlgorithm.rsa_pss_pss_sha256:
                case CryptoSignatureAlgorithm.rsa_pss_pss_sha384:
                case CryptoSignatureAlgorithm.rsa_pss_pss_sha512:
                    return true;

                // TODO[draft-smyshlyaev-tls12-gost-suites-10]
                case CryptoSignatureAlgorithm.gostr34102012_256:
                case CryptoSignatureAlgorithm.gostr34102012_512:

                // TODO[RFC 8998]
                case CryptoSignatureAlgorithm.sm2:

                default:
                    return false;
            }
        }

        public override bool HasMacAlgorithm(int macAlgorithm) { return true; }

        public override bool HasNamedGroup(int namedGroup) { return NamedGroup.RefersToASpecificGroup(namedGroup); }

        public override bool HasRsaEncryption() { return true; }

        public override bool HasSignatureAlgorithm(short signatureAlgorithm)
        {
            switch (signatureAlgorithm)
            {
                case SignatureAlgorithm.rsa:
                case SignatureAlgorithm.dsa:
                case SignatureAlgorithm.ecdsa:
                case SignatureAlgorithm.ed25519:
                case SignatureAlgorithm.ed448:
                case SignatureAlgorithm.rsa_pss_rsae_sha256:
                case SignatureAlgorithm.rsa_pss_rsae_sha384:
                case SignatureAlgorithm.rsa_pss_rsae_sha512:
                case SignatureAlgorithm.rsa_pss_pss_sha256:
                case SignatureAlgorithm.rsa_pss_pss_sha384:
                case SignatureAlgorithm.rsa_pss_pss_sha512:
                case SignatureAlgorithm.ecdsa_brainpoolP256r1tls13_sha256:
                case SignatureAlgorithm.ecdsa_brainpoolP384r1tls13_sha384:
                case SignatureAlgorithm.ecdsa_brainpoolP512r1tls13_sha512:
                    return true;

                // TODO[draft-smyshlyaev-tls12-gost-suites-10]
                case SignatureAlgorithm.gostr34102012_256:
                case SignatureAlgorithm.gostr34102012_512:
                // TODO[RFC 8998]
                //case SignatureAlgorithm.sm2:
                default:
                    return false;
            }
        }

        public override bool HasSignatureAndHashAlgorithm(SignatureAndHashAlgorithm sigAndHashAlgorithm)
        {
            var signature = sigAndHashAlgorithm.Signature;

            switch (sigAndHashAlgorithm.Hash)
            {
                case HashAlgorithm.md5:
                    return SignatureAlgorithm.rsa == signature && this.HasSignatureAlgorithm(signature);
                default:
                    return this.HasSignatureAlgorithm(signature);
            }
        }

        public override bool HasSignatureScheme(int signatureScheme)
        {
            switch (signatureScheme)
            {
                case SignatureScheme.sm2sig_sm3:
                    return false;
                default:
                {
                    var signature = SignatureScheme.GetSignatureAlgorithm(signatureScheme);

                    switch (SignatureScheme.GetCryptoHashAlgorithm(signatureScheme))
                    {
                        case CryptoHashAlgorithm.md5:
                            return SignatureAlgorithm.rsa == signature && this.HasSignatureAlgorithm(signature);
                        default:
                            return this.HasSignatureAlgorithm(signature);
                    }
                }
            }
        }

        public override bool HasSrpAuthentication() { return true; }

        public override TlsSecret CreateSecret(byte[] data) { return this.AdoptLocalSecret(Arrays.Clone(data)); }

        public override TlsSecret GenerateRsaPreMasterSecret(ProtocolVersion version)
        {
            var data = new byte[48];
            this.SecureRandom.NextBytes(data);
            TlsUtilities.WriteVersion(version, data, 0);
            return this.AdoptLocalSecret(data);
        }

        public virtual IDigest CloneDigest(int cryptoHashAlgorithm, IDigest digest)
        {
            switch (cryptoHashAlgorithm)
            {
                case CryptoHashAlgorithm.md5:
                    return new MD5Digest((MD5Digest)digest);
                case CryptoHashAlgorithm.sha1:
                    return new Sha1Digest((Sha1Digest)digest);
                case CryptoHashAlgorithm.sha224:
                    return new Sha224Digest((Sha224Digest)digest);
                case CryptoHashAlgorithm.sha256:
                    return new Sha256Digest((Sha256Digest)digest);
                case CryptoHashAlgorithm.sha384:
                    return new Sha384Digest((Sha384Digest)digest);
                case CryptoHashAlgorithm.sha512:
                    return new Sha512Digest((Sha512Digest)digest);
                case CryptoHashAlgorithm.sm3:
                    return new SM3Digest((SM3Digest)digest);
                default:
                    throw new ArgumentException("invalid CryptoHashAlgorithm: " + cryptoHashAlgorithm);
            }
        }

        public virtual IDigest CreateDigest(int cryptoHashAlgorithm)
        {
            switch (cryptoHashAlgorithm)
            {
                case CryptoHashAlgorithm.md5:
                    return new MD5Digest();
                case CryptoHashAlgorithm.sha1:
                    return new Sha1Digest();
                case CryptoHashAlgorithm.sha224:
                    return new Sha224Digest();
                case CryptoHashAlgorithm.sha256:
                    return new Sha256Digest();
                case CryptoHashAlgorithm.sha384:
                    return new Sha384Digest();
                case CryptoHashAlgorithm.sha512:
                    return new Sha512Digest();
                case CryptoHashAlgorithm.sm3:
                    return new SM3Digest();
                default:
                    throw new ArgumentException("invalid CryptoHashAlgorithm: " + cryptoHashAlgorithm);
            }
        }

        public override TlsHash CreateHash(int cryptoHashAlgorithm) { return new BcTlsHash(this, cryptoHashAlgorithm); }

        protected virtual IBlockCipher CreateBlockCipher(int encryptionAlgorithm)
        {
            switch (encryptionAlgorithm)
            {
                case EncryptionAlgorithm.cls_3DES_EDE_CBC:
                    return this.CreateDesEdeEngine();
                case EncryptionAlgorithm.AES_128_CBC:
                case EncryptionAlgorithm.AES_256_CBC:
                    return this.CreateAesEngine();
                case EncryptionAlgorithm.ARIA_128_CBC:
                case EncryptionAlgorithm.ARIA_256_CBC:
                    return this.CreateAriaEngine();
                case EncryptionAlgorithm.CAMELLIA_128_CBC:
                case EncryptionAlgorithm.CAMELLIA_256_CBC:
                    return this.CreateCamelliaEngine();
                case EncryptionAlgorithm.SEED_CBC:
                    return this.CreateSeedEngine();
                case EncryptionAlgorithm.SM4_CBC:
                    return this.CreateSM4Engine();
                default:
                    throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        protected virtual IBlockCipher CreateCbcBlockCipher(IBlockCipher blockCipher) { return new CbcBlockCipher(blockCipher); }

        protected virtual IBlockCipher CreateCbcBlockCipher(int encryptionAlgorithm) { return this.CreateCbcBlockCipher(this.CreateBlockCipher(encryptionAlgorithm)); }

        protected virtual TlsCipher CreateChaCha20Poly1305(TlsCryptoParameters cryptoParams)
        {
            var encrypt = new BcChaCha20Poly1305(true);
            var decrypt = new BcChaCha20Poly1305(false);

            return new TlsAeadCipher(cryptoParams, encrypt, decrypt, 32, 16, TlsAeadCipher.AEAD_CHACHA20_POLY1305);
        }

        protected virtual TlsAeadCipher CreateCipher_Aes_Ccm(TlsCryptoParameters cryptoParams, int cipherKeySize,
            int macSize)
        {
            var encrypt = new BcTlsAeadCipherImpl(this.CreateAeadBlockCipher_Aes_Ccm(), true);
            var decrypt = new BcTlsAeadCipherImpl(this.CreateAeadBlockCipher_Aes_Ccm(), false);

            return new TlsAeadCipher(cryptoParams, encrypt, decrypt, cipherKeySize, macSize, TlsAeadCipher.AEAD_CCM);
        }

        protected virtual TlsAeadCipher CreateCipher_Aes_Gcm(TlsCryptoParameters cryptoParams, int cipherKeySize,
            int macSize)
        {
            var encrypt = new BcTlsAeadCipherImpl(this.CreateAeadBlockCipher_Aes_Gcm(), true);
            var decrypt = new BcTlsAeadCipherImpl(this.CreateAeadBlockCipher_Aes_Gcm(), false);

            return new TlsAeadCipher(cryptoParams, encrypt, decrypt, cipherKeySize, macSize, TlsAeadCipher.AEAD_GCM);
        }

        protected virtual TlsAeadCipher CreateCipher_Aria_Gcm(TlsCryptoParameters cryptoParams, int cipherKeySize,
            int macSize)
        {
            var encrypt = new BcTlsAeadCipherImpl(this.CreateAeadBlockCipher_Aria_Gcm(), true);
            var decrypt = new BcTlsAeadCipherImpl(this.CreateAeadBlockCipher_Aria_Gcm(), false);

            return new TlsAeadCipher(cryptoParams, encrypt, decrypt, cipherKeySize, macSize, TlsAeadCipher.AEAD_GCM);
        }

        protected virtual TlsAeadCipher CreateCipher_Camellia_Gcm(TlsCryptoParameters cryptoParams, int cipherKeySize,
            int macSize)
        {
            var encrypt = new BcTlsAeadCipherImpl(this.CreateAeadBlockCipher_Camellia_Gcm(), true);
            var decrypt = new BcTlsAeadCipherImpl(this.CreateAeadBlockCipher_Camellia_Gcm(), false);

            return new TlsAeadCipher(cryptoParams, encrypt, decrypt, cipherKeySize, macSize, TlsAeadCipher.AEAD_GCM);
        }

        protected virtual TlsCipher CreateCipher_Cbc(TlsCryptoParameters cryptoParams, int encryptionAlgorithm,
            int cipherKeySize, int macAlgorithm)
        {
            var encrypt = new BcTlsBlockCipherImpl(this.CreateCbcBlockCipher(encryptionAlgorithm), true);
            var decrypt = new BcTlsBlockCipherImpl(this.CreateCbcBlockCipher(encryptionAlgorithm), false);

            var clientMac = this.CreateMac(cryptoParams, macAlgorithm);
            var serverMac = this.CreateMac(cryptoParams, macAlgorithm);

            return new TlsBlockCipher(cryptoParams, encrypt, decrypt, clientMac, serverMac, cipherKeySize);
        }

        protected virtual TlsAeadCipher CreateCipher_SM4_Ccm(TlsCryptoParameters cryptoParams)
        {
            var encrypt = new BcTlsAeadCipherImpl(this.CreateAeadBlockCipher_SM4_Ccm(), true);
            var decrypt = new BcTlsAeadCipherImpl(this.CreateAeadBlockCipher_SM4_Ccm(), false);

            return new TlsAeadCipher(cryptoParams, encrypt, decrypt, 16, 16, TlsAeadCipher.AEAD_CCM);
        }

        protected virtual TlsAeadCipher CreateCipher_SM4_Gcm(TlsCryptoParameters cryptoParams)
        {
            var encrypt = new BcTlsAeadCipherImpl(this.CreateAeadBlockCipher_SM4_Gcm(), true);
            var decrypt = new BcTlsAeadCipherImpl(this.CreateAeadBlockCipher_SM4_Gcm(), false);

            return new TlsAeadCipher(cryptoParams, encrypt, decrypt, 16, 16, TlsAeadCipher.AEAD_GCM);
        }

        protected virtual TlsNullCipher CreateNullCipher(TlsCryptoParameters cryptoParams, int macAlgorithm)
        {
            return new TlsNullCipher(cryptoParams, this.CreateMac(cryptoParams, macAlgorithm), this.CreateMac(cryptoParams, macAlgorithm));
        }

        protected virtual IBlockCipher CreateAesEngine() { return new AesEngine(); }

        protected virtual IBlockCipher CreateAriaEngine() { return new AriaEngine(); }

        protected virtual IBlockCipher CreateCamelliaEngine() { return new CamelliaEngine(); }

        protected virtual IBlockCipher CreateDesEdeEngine() { return new DesEdeEngine(); }

        protected virtual IBlockCipher CreateSeedEngine() { return new SeedEngine(); }

        protected virtual IBlockCipher CreateSM4Engine() { return new SM4Engine(); }

        protected virtual IAeadBlockCipher CreateCcmMode(IBlockCipher engine) { return new CcmBlockCipher(engine); }

        protected virtual IAeadBlockCipher CreateGcmMode(IBlockCipher engine)
        {
            // TODO Consider allowing custom configuration of multiplier
            return new GcmBlockCipher(engine);
        }

        protected virtual IAeadBlockCipher CreateAeadBlockCipher_Aes_Ccm() { return this.CreateCcmMode(this.CreateAesEngine()); }

        protected virtual IAeadBlockCipher CreateAeadBlockCipher_Aes_Gcm() { return this.CreateGcmMode(this.CreateAesEngine()); }

        protected virtual IAeadBlockCipher CreateAeadBlockCipher_Aria_Gcm() { return this.CreateGcmMode(this.CreateAriaEngine()); }

        protected virtual IAeadBlockCipher CreateAeadBlockCipher_Camellia_Gcm() { return this.CreateGcmMode(this.CreateCamelliaEngine()); }

        protected virtual IAeadBlockCipher CreateAeadBlockCipher_SM4_Ccm() { return this.CreateCcmMode(this.CreateSM4Engine()); }

        protected virtual IAeadBlockCipher CreateAeadBlockCipher_SM4_Gcm() { return this.CreateGcmMode(this.CreateSM4Engine()); }

        public override TlsHmac CreateHmac(int macAlgorithm) { return this.CreateHmacForHash(TlsCryptoUtilities.GetHashForHmac(macAlgorithm)); }

        public override TlsHmac CreateHmacForHash(int cryptoHashAlgorithm) { return new BcTlsHmac(new HMac(this.CreateDigest(cryptoHashAlgorithm))); }

        protected virtual TlsHmac CreateHmac_Ssl(int macAlgorithm)
        {
            switch (macAlgorithm)
            {
                case MacAlgorithm.hmac_md5:
                    return new BcSsl3Hmac(this.CreateDigest(CryptoHashAlgorithm.md5));
                case MacAlgorithm.hmac_sha1:
                    return new BcSsl3Hmac(this.CreateDigest(CryptoHashAlgorithm.sha1));
                case MacAlgorithm.hmac_sha256:
                    return new BcSsl3Hmac(this.CreateDigest(CryptoHashAlgorithm.sha256));
                case MacAlgorithm.hmac_sha384:
                    return new BcSsl3Hmac(this.CreateDigest(CryptoHashAlgorithm.sha384));
                case MacAlgorithm.hmac_sha512:
                    return new BcSsl3Hmac(this.CreateDigest(CryptoHashAlgorithm.sha512));
                default:
                    throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        protected virtual TlsHmac CreateMac(TlsCryptoParameters cryptoParams, int macAlgorithm)
        {
            if (TlsImplUtilities.IsSsl(cryptoParams))
                return this.CreateHmac_Ssl(macAlgorithm);
            return this.CreateHmac(macAlgorithm);
        }

        public override TlsSrp6Client CreateSrp6Client(TlsSrpConfig srpConfig)
        {
            var ng       = srpConfig.GetExplicitNG();
            var srpGroup = new Srp6GroupParameters(ng[0], ng[1]);

            var srp6Client = new Srp6Client();
            srp6Client.Init(srpGroup, this.CreateDigest(CryptoHashAlgorithm.sha1), this.SecureRandom);

            return new BcTlsSrp6Client(srp6Client);
        }

        public override TlsSrp6Server CreateSrp6Server(TlsSrpConfig srpConfig, BigInteger srpVerifier)
        {
            var ng       = srpConfig.GetExplicitNG();
            var srpGroup = new Srp6GroupParameters(ng[0], ng[1]);

            var srp6Server = new Srp6Server();
            srp6Server.Init(srpGroup, srpVerifier, this.CreateDigest(CryptoHashAlgorithm.sha1), this.SecureRandom);

            return new BcTlsSrp6Server(srp6Server);
        }

        public override TlsSrp6VerifierGenerator CreateSrp6VerifierGenerator(TlsSrpConfig srpConfig)
        {
            var ng = srpConfig.GetExplicitNG();

            var srp6VerifierGenerator = new Srp6VerifierGenerator();
            srp6VerifierGenerator.Init(ng[0], ng[1], this.CreateDigest(CryptoHashAlgorithm.sha1));

            return new BcTlsSrp6VerifierGenerator(srp6VerifierGenerator);
        }

        public override TlsSecret HkdfInit(int cryptoHashAlgorithm) { return this.AdoptLocalSecret(new byte[TlsCryptoUtilities.GetHashOutputSize(cryptoHashAlgorithm)]); }
    }
}
#pragma warning restore
#endif