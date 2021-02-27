using System;
using System.Net;

namespace StatelessIdentity.Domain.Exceptions
{
    public class UserProviderException : Exception
    {
        public UserProviderException(string message) :base(message) { }

        public UserProviderException(HttpStatusCode code, string content) : base (
            $"Request from user provider failed with status code '{code}' and content '{content}'") { }
    }
}
