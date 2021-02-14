using Microsoft.IdentityModel.Tokens;
using StatelessIdentity.Domain.Constants;
using StatelessIdentity.Domain.Exceptions;
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
        public static int MinSymmetricKeyBytes = 16;
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

        /// <summary>
        /// Create a token using a symmetric security key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expires"></param>
        public string Token(string key, DateTime? expires = null)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var bytes = Encoding.UTF8.GetBytes(key);
            if (bytes.Length < MinSymmetricKeyBytes)
                throw new TokenException($"Symmetric key length must be greater than {MinSymmetricKeyBytes} bytes");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            return Token(signingCredentials, expires);
        }

        /// <summary>
        /// Create a token using an asymmetric security key.
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="expires"></param>
        public string Token(RSA rsa, DateTime? expires = null)
        {
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));

            var securityKey = new RsaSecurityKey(rsa);
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha512Signature);

            return Token(signingCredentials, expires);
        }

        /// <summary>
        /// Create a token using the provided SigningCredentials.
        /// </summary>
        /// <param name="signingCredentials"></param>
        /// <param name="expires"></param>
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

        /// <summary>
        /// Create a token using the provided SecurityTokenDescriptor.
        /// Id and User claims will be added to the existing Subject.
        /// </summary>
        /// <param name="securityTokenDescriptor"></param>
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

        /// <summary>
        /// Parse a token using a symmetric security key.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="key"></param>
        public static Identity Parse(string token, string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            return Parse(token, securityKey);
        }

        /// <summary>
        /// Parse a token using an asymmetric security key.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="rsa"></param>
        public static Identity Parse(string token, RSA rsa)
        {
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));

            var securityKey = new RsaSecurityKey(rsa);

            return Parse(token, securityKey);
        }

        /// <summary>
        /// Parse a token using the provided SecurityKey.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="securityKey"></param>
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

        /// <summary>
        /// Parse a token using the provided TokenValidationParameters.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="tokenValidationParameters"></param>
        public static Identity Parse(string token, TokenValidationParameters tokenValidationParameters)
        {
            if (tokenValidationParameters == null)
                throw new ArgumentNullException(nameof(tokenValidationParameters));

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validToken);

                return new Identity(validToken as JwtSecurityToken);
            } 
            catch(Exception e) 
            {
                throw new TokenException("Token parse failed", e);
            }
        }
    }
}
