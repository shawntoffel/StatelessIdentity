using System;
using System.Security.Cryptography;
using System.Text;

namespace StatelessIdentity.Domain.Extensions
{
    public static class HashingExtensions
    {
        public static  byte[] ComputeHash(string input)
        {
            using var sha512Managed = new SHA512Managed();

            var bytes = Encoding.ASCII.GetBytes(input);
            return sha512Managed.ComputeHash(bytes);
        }

        public static string ComputeBase64Hash(string input)
        {
            var hash = ComputeHash(input);
            return Convert.ToBase64String(hash);
        }
    }
}
