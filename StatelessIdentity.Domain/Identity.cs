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
        private const int MinSymmetricKeyBytes = 16;

        public Guid Id { get; }

        public User User { get; set; }

        public Identity(User user)
        {
            Id = Guid.NewGuid();
            User = user;
        }

        private Identity(JwtSecurityToken jwtSecurityToken, TokenOptions tokenOptions)
        {
            if (jwtSecurityToken == null)
                throw new ArgumentNullException(nameof(jwtSecurityToken));

            if (tokenOptions == null)
                throw new ArgumentNullException(nameof(tokenOptions));

            Id = jwtSecurityToken.Claims.ValueOrDefault(JwtClaimTypes.Id, Guid.Parse);
            User = jwtSecurityToken.Claims.ValueOrDefault(JwtClaimTypes.User, (s) => JsonSerializer.Deserialize<User>(s, tokenOptions.JsonSerializerOptions));
        }

        /// <summary>
        /// Create a token using a symmetric security key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="tokenOptions"></param>
        public string Token(string key, TokenOptions tokenOptions = null)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var bytes = Encoding.UTF8.GetBytes(key);
            if (bytes.Length < MinSymmetricKeyBytes)
                throw new TokenException($"Symmetric key length must be greater than {MinSymmetricKeyBytes} bytes");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            return Token(signingCredentials, tokenOptions);
        }

        /// <summary>
        /// Create a token using an asymmetric security key.
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="tokenOptions"></param>
        public string Token(RSA rsa, TokenOptions tokenOptions = null)
        {
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));

            var securityKey = new RsaSecurityKey(rsa);
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha512Signature);

            return Token(signingCredentials, tokenOptions);
        }

        /// <summary>
        /// Create a token using the provided SigningCredentials.
        /// </summary>
        /// <param name="signingCredentials"></param>
        /// <param name="tokenOptions"></param>
        public string Token(SigningCredentials signingCredentials, TokenOptions tokenOptions = null)
        {
            tokenOptions ??= new TokenOptions();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                Expires = DateTime.UtcNow.Add(tokenOptions.ExpirationTimeSpan),
                SigningCredentials = signingCredentials
            };

            return Token(tokenDescriptor, tokenOptions);
        }

        /// <summary>
        /// Create a token using the provided SecurityTokenDescriptor.
        /// Id and User claims will be added to the existing Subject.
        /// </summary>
        /// <param name="securityTokenDescriptor"></param>
        /// <param name="tokenOptions"></param>
        public string Token(SecurityTokenDescriptor securityTokenDescriptor, TokenOptions tokenOptions = null)
        {
            if (securityTokenDescriptor == null)
                throw new ArgumentNullException(nameof(securityTokenDescriptor));

            tokenOptions ??= new TokenOptions();

            var claimsIdentity = securityTokenDescriptor.Subject ?? new ClaimsIdentity();
            claimsIdentity.AddUnlessEmpty(JwtClaimTypes.Id, Id);
            claimsIdentity.AddUnlessEmpty(JwtClaimTypes.User, JsonSerializer.Serialize(User, tokenOptions.JsonSerializerOptions));
            claimsIdentity.AddUnlessEmpty(JwtRegisteredClaimNames.Iss, tokenOptions.Issuer);
            claimsIdentity.AddUnlessEmpty(JwtRegisteredClaimNames.Aud, tokenOptions.Audience);

            securityTokenDescriptor.Subject = claimsIdentity;

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.CreateEncodedJwt(securityTokenDescriptor);
        }

        /// <summary>
        /// Parse a token using a symmetric security key.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="key"></param>
        /// <param name="tokenOptions"></param>
        public static Identity Parse(string token, string key, TokenOptions tokenOptions = null)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            return Parse(token, securityKey, tokenOptions);
        }

        /// <summary>
        /// Parse a token using an asymmetric security key.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="rsa"></param>
        /// <param name="tokenOptions"></param>
        public static Identity Parse(string token, RSA rsa, TokenOptions tokenOptions = null)
        {
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));

            var securityKey = new RsaSecurityKey(rsa);

            return Parse(token, securityKey, tokenOptions);
        }

        /// <summary>
        /// Parse a token using the provided SecurityKey.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="securityKey"></param>
        /// <param name="tokenOptions"></param>
        public static Identity Parse(string token, SecurityKey securityKey, TokenOptions tokenOptions = null)
        {
            tokenOptions ??= new TokenOptions();

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidIssuer = tokenOptions.Issuer,
                ValidAudience = tokenOptions.Audience
            };

            return Parse(token, tokenValidationParameters, tokenOptions);
        }

        /// <summary>
        /// Parse a token using the provided TokenValidationParameters.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="tokenValidationParameters"></param>
        /// <param name="tokenOptions"></param>
        public static Identity Parse(string token, TokenValidationParameters tokenValidationParameters, TokenOptions tokenOptions = null)
        {
            if (tokenValidationParameters == null)
                throw new ArgumentNullException(nameof(tokenValidationParameters));

            tokenOptions ??= new TokenOptions();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validToken);

                return new Identity(validToken as JwtSecurityToken, tokenOptions);
            } 
            catch(Exception e) 
            {
                throw new TokenException("Token parse failed", e);
            }
        }
    }
}
