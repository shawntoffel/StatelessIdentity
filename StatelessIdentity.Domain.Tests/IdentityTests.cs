using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using StatelessIdentity.Domain.Exceptions;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace StatelessIdentity.Domain.Tests
{
    [TestFixture]
    public class IdentityTests
    {
        private const string SymmetricKey = "asdv234234^&%&^%&^hjsdfb2%%%";
        private readonly RSA AsymmetricKey = RSA.Create(2048);

        [SetUp]
        public void SetUp()
        {
            IdentityModelEventSource.ShowPII = true;
        }

        [Test]
        public void TestCreateAndParseTokenSymmetricKey()
        {
            var user = new User("provider", "externalId");
            var identity = new Identity(user);

            var token = identity.Token(new TokenOptions(SymmetricKey));
            Assert.NotNull(token);

            var parsedIdentity = Identity.Parse(token, new TokenOptions(SymmetricKey));
            Assert.AreEqual(identity.Id, parsedIdentity.Id);
            Assert.AreEqual(identity.User.Digest, parsedIdentity.User.Digest);
        }

        [Test]
        public void TestCreateAndParseTokenAsymmetricKey()
        {
            var user = new User("provider", "externalId");

            var identity = new Identity(user);

            var token = identity.Token(new TokenOptions(AsymmetricKey));
            Assert.NotNull(token);

            var parsedIdentity = Identity.Parse(token, new TokenOptions(AsymmetricKey));
            Assert.AreEqual(identity.Id, parsedIdentity.Id);
            Assert.AreEqual(identity.User.Digest, parsedIdentity.User.Digest);
        }

        [Test]
        public void TestCreateAndParseTokenEncryption()
        {
            var user = new User("provider", "externalId");
            var identity = new Identity(user);

            var cert = BuildSelfSignedServerCertificate();

            var tokenOptions = new TokenOptions(SymmetricKey)
            {
                EncryptingCredentials = new X509EncryptingCredentials(cert, SecurityAlgorithms.RsaOaepKeyWrap, SecurityAlgorithms.Aes256CbcHmacSha512)
            };

            var token = identity.Token(tokenOptions);
            Assert.NotNull(token);

            var parsedIdentity = Identity.Parse(token, tokenOptions);
            Assert.AreEqual(identity.Id, parsedIdentity.Id);
            Assert.AreEqual(identity.User.Digest, parsedIdentity.User.Digest);
        }

        [Test]
        public void TestSymmetricKeyCreate_SmallKey_ThrowsException()
        {
            var identity = new Identity(new User("provider", "externalId"));
            Assert.Throws<TokenException>(() => identity.Token(new TokenOptions("test")));
        }

        private X509Certificate2 BuildSelfSignedServerCertificate()
        {
            using RSA rsa = RSA.Create(2048);
            var request = new CertificateRequest("CN=localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)), new DateTimeOffset(DateTime.UtcNow.AddDays(3650)));
        }
    }
}
