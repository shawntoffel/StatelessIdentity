using System;

namespace StatelessIdentity.Domain.Exceptions
{
    public class TokenException : Exception
    {
        public TokenException(string message, Exception e = null) : base(message, e) { }
    }
}
