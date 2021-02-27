using Microsoft.IdentityModel.Tokens;
using StatelessIdentity.Domain.Exceptions;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace StatelessIdentity.Domain
{
    public class TokenOptions
    {
        private const int MinSymmetricKeyBytes = 16;

        public TimeSpan ExpirationTimeSpan { get; set; }

        public JsonSerializerOptions JsonSerializerOptions { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public SigningCredentials SigningCredentials { get; set; }

        public EncryptingCredentials EncryptingCredentials { get; set; }

        public TokenOptions()
        {
            ExpirationTimeSpan = TimeSpan.FromDays(30);
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                IgnoreNullValues = true
            };
            Issuer = "*";
            Audience = "*";
        }

        public TokenOptions(string signingKey, string algorithm = SecurityAlgorithms.HmacSha512Signature) : this()
        {
            if (signingKey == null)
                throw new ArgumentNullException(nameof(signingKey));

            var bytes = Encoding.UTF8.GetBytes(signingKey);
            if (bytes.Length < MinSymmetricKeyBytes)
                throw new TokenException($"Symmetric key length must be greater than {MinSymmetricKeyBytes} bytes");

            var securityKey = new SymmetricSecurityKey(bytes);
            SigningCredentials = new SigningCredentials(securityKey, algorithm);
        }

        public TokenOptions(RSA signingRsa, string algorithm = SecurityAlgorithms.RsaSha512Signature) : this()
        {
            if (signingRsa == null)
                throw new ArgumentNullException(nameof(signingRsa));

            var securityKey = new RsaSecurityKey(signingRsa);
            SigningCredentials = new SigningCredentials(securityKey, algorithm);
        }
    }
}
