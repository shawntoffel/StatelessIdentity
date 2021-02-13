using System;

namespace StatelessIdentity.Domain.Exceptions
{
    public class UnknownUserProviderException : Exception
    {
        public string UnsupportedProviderName { get; set; }

        public UnknownUserProviderException(string unsupportedProviderName) : base($"User provider with id '{unsupportedProviderName}' is not registered")
        {
            UnsupportedProviderName = unsupportedProviderName;
        }
    }
}
