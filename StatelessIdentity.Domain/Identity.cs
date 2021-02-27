using Microsoft.IdentityModel.Tokens;
using StatelessIdentity.Domain.Constants;
using StatelessIdentity.Domain.Exceptions;
using StatelessIdentity.Domain.Extensions;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace StatelessIdentity.Domain
{
    public class Identity
    {
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

            Id = jwtSecurityToken.Claims.ValueOrDefault(JwtClaimNames.Id, Guid.Parse);
            User = jwtSecurityToken.Claims.ValueOrDefault(JwtClaimNames.User, (s) => JsonSerializer.Deserialize<User>(s, tokenOptions.JsonSerializerOptions));
        }

        /// <summary>
        /// Create a token.
        /// </summary>
        /// <param name="tokenOptions"></param>
        public string Token(TokenOptions tokenOptions)
        {
            if (tokenOptions == null)
                throw new ArgumentNullException(nameof(tokenOptions));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                Expires = DateTime.UtcNow.Add(tokenOptions.ExpirationTimeSpan),
                SigningCredentials = tokenOptions.SigningCredentials,
                EncryptingCredentials = tokenOptions.EncryptingCredentials
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
            claimsIdentity.AddUnlessEmpty(JwtClaimNames.Id, Id);
            claimsIdentity.AddUnlessEmpty(JwtClaimNames.User, JsonSerializer.Serialize(User, tokenOptions.JsonSerializerOptions));
            claimsIdentity.AddUnlessEmpty(JwtRegisteredClaimNames.Iss, tokenOptions.Issuer);
            claimsIdentity.AddUnlessEmpty(JwtRegisteredClaimNames.Aud, tokenOptions.Audience);

            securityTokenDescriptor.Subject = claimsIdentity;

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.CreateEncodedJwt(securityTokenDescriptor);
        }

        /// <summary>
        /// Parse a token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="tokenOptions"></param>
        public static Identity Parse(string token, TokenOptions tokenOptions = null)
        {
            tokenOptions ??= new TokenOptions();

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = tokenOptions.SigningCredentials?.Key,
                ValidIssuer = tokenOptions.Issuer,
                ValidAudience = tokenOptions.Audience,
                TokenDecryptionKey = tokenOptions.EncryptingCredentials?.Key
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
