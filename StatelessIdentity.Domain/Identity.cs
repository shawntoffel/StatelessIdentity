using Microsoft.IdentityModel.Tokens;
using StatelessIdentity.Domain.Constants;
using StatelessIdentity.Domain.Extensions;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace StatelessIdentity.Domain
{
    public class Identity
    {
        public static DateTime DefaultExpiration = DateTime.UtcNow.AddDays(30);

        public Guid Id { get; set; }

        public User User { get; set; }

        public Identity(User user)
        {
            Id = Guid.NewGuid();
            User = user;
        }

        private Identity(JwtSecurityToken jwtSecurityToken)
        {
            if (jwtSecurityToken == null)
                throw new ArgumentNullException(nameof(jwtSecurityToken));

            Id = jwtSecurityToken.Claims.ValueOrDefault(JwtClaimTypes.Id, Guid.Parse);
            User = jwtSecurityToken.Claims.ValueOrDefault(JwtClaimTypes.User, (s) => JsonSerializer.Deserialize<User>(s));
        }

        public string Token(string key, DateTime? expires = null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            return Token(signingCredentials, expires);
        }

        public string Token(RSA rsa, DateTime? expires = null)
        {
            var securityKey = new RsaSecurityKey(rsa);
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha512Signature);

            return Token(signingCredentials, expires);
        }

        public string Token(SigningCredentials signingCredentials, DateTime? expires = null)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                Expires = expires ?? DefaultExpiration,
                SigningCredentials = signingCredentials
            };

            return Token(tokenDescriptor);
        }

        public string Token(SecurityTokenDescriptor securityTokenDescriptor)
        {
            if (securityTokenDescriptor == null)
                throw new ArgumentNullException(nameof(securityTokenDescriptor));

            var claimsIdentity = securityTokenDescriptor.Subject ?? new ClaimsIdentity();
            claimsIdentity.AddUnlessEmpty(JwtClaimTypes.Id, Id);
            claimsIdentity.AddUnlessEmpty(JwtClaimTypes.User, JsonSerializer.Serialize(User));

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.CreateEncodedJwt(securityTokenDescriptor);
        }

        public static Identity Parse(string token, string key)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

            return Parse(token, securityKey);
        }

        public static Identity Parse(string token, RSA rsa)
        {
            var securityKey = new RsaSecurityKey(rsa);

            return Parse(token, securityKey);
        }

        public static Identity Parse(string token, SecurityKey securityKey)
        {
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
