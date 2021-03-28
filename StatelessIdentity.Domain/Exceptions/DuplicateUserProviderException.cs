using System;

namespace StatelessIdentity.Domain.Exceptions
{
    public class DuplicateUserProviderException : Exception
    {
        public string UserProviderName { get; set; }

        public DuplicateUserProviderException(string userProviderName) : base($"User provider with name '{userProviderName}' has already been registered.")
        {
            UserProviderName = userProviderName;
        }
    }
}
