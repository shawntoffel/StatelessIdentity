using System;

namespace StatelessIdentity.Domain.Exceptions
{
    public class UnknownUserProviderException : Exception
    {
        public string UnknownUserProviderName { get; set; }

        public UnknownUserProviderException(string unknownUserProviderName) : base($"User provider with name '{unknownUserProviderName}' is not registered")
        {
            UnknownUserProviderName = unknownUserProviderName;
        }
    }
}
