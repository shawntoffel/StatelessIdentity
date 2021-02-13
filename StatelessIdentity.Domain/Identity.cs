using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using StatelessIdentity.Domain.Extensions;
using StatelessIdentity.Domain.Constants;

namespace StatelessIdentity.Domain
{
    public class Identity
    {
        public Guid Id { get; set; }

        public Guid ProviderId { get; set; }

        public User User { get; set; }

        public Identity(Guid providerGuid, User user)
        {
            Id = Guid.NewGuid();
            ProviderId = providerGuid;
            User = user;
        }

        private Identity(JwtSecurityToken jwt)
        {
            Id = jwt.Claims.ValueOrDefault(JwtClaimTypes.Id, Guid.Parse);
            ProviderId = jwt.Claims.ValueOrDefault(JwtClaimTypes.ProviderId, Guid.Parse);
            User = new User()
            {
                ExternalId = jwt.Claims.ValueOrDefault(JwtClaimTypes.User.ExternalId),
                Name = jwt.Claims.ValueOrDefault(JwtClaimTypes.User.Name),
                AvatarUrl = jwt.Claims.ValueOrDefault(JwtClaimTypes.User.AvatarUrl),
            };
        }

        public string Token(string key)
        {
            return Token(key, DateTime.UtcNow.AddDays(30));
        }

        public string Token(string key, DateTime? expires)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                Expires = expires,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature)
            };

            return Token(tokenDescriptor);
        }

        public string Token(SecurityTokenDescriptor securityTokenDescriptor)
        {
            if (securityTokenDescriptor == null)
                throw new ArgumentNullException(nameof(securityTokenDescriptor));

            var claimsIdentity = securityTokenDescriptor.Subject ?? new ClaimsIdentity();
            claimsIdentity.AddUnlessEmpty(JwtClaimTypes.Id, Id);
            claimsIdentity.AddUnlessEmpty(JwtClaimTypes.ProviderId, ProviderId);
            claimsIdentity.AddUnlessEmpty(JwtClaimTypes.User.ExternalId, User.ExternalId);
            claimsIdentity.AddUnlessEmpty(JwtClaimTypes.User.Name, User.Name);
            claimsIdentity.AddUnlessEmpty(JwtClaimTypes.User.AvatarUrl, User.AvatarUrl);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.CreateEncodedJwt(securityTokenDescriptor);
        }

        public string GetHash()
        {
            return  ComputeBase64Hash(ProviderId.ToString() + User?.ExternalId);
        }

        private string ComputeBase64Hash(string input)
        {
            using var sha512Managed = new SHA512Managed();

            var bytes = Encoding.ASCII.GetBytes(input);
            var hash = sha512Managed.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        public static Identity Parse(string token, string key)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
            };

            return Parse(token, tokenValidationParameters);
        }

        public static Identity Parse(string token, TokenValidationParameters tokenValidationParameters)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validToken);

            return new Identity(validToken as JwtSecurityToken);
        }
    }
}
