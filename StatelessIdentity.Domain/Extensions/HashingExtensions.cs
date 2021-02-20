using System;
using System.Security.Cryptography;
using System.Text;

namespace StatelessIdentity.Domain.Extensions
{
    internal static class HashingExtensions
    {
        internal static  byte[] ComputeSHA512Hash(string input)
        {
            using var sha512Managed = new SHA512Managed();

            var bytes = Encoding.ASCII.GetBytes(input);
            return sha512Managed.ComputeHash(bytes);
        }

        internal static string ComputeSHA512Base64Hash(string input)
        {
            var hash = ComputeSHA512Hash(input);
            return Convert.ToBase64String(hash);
        }
    }
}
