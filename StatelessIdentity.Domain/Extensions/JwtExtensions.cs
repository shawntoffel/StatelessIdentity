using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace StatelessIdentity.Domain.Extensions
{
    internal static class JwtExtensions
    {
        internal static void AddUnlessEmpty(this ClaimsIdentity claimsIdentity, string type, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            claimsIdentity.AddClaim(new Claim(type, value));
        }

        internal static void AddUnlessEmpty<T>(this ClaimsIdentity claimsIdentity, string type, T value)
        {
            claimsIdentity.AddUnlessEmpty(type, value.ToString());
        }

        internal static string ValueOrDefault(this IEnumerable<Claim> claims, string type)
        {
            return ValueOrDefault(claims, type, (s) => s);
        }

        internal static T ValueOrDefault<T>(this IEnumerable<Claim> claims, string type, Func<string, T> parseFunc)
        {
            var first = claims.FirstOrDefault(c => c.Type == type);
            if (first == null)
                return default;

            return parseFunc(first.Value);
        }
    }
}
