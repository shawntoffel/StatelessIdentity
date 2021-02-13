
namespace StatelessIdentity.Domain.Constants
{
    public static class JwtClaimTypes
    {
        public const string Id = "id";
        public const string ProviderId = "pid";

        public static class User
        {
            public const string ExternalId = "uid";
            public const string Name = "un";
            public const string AvatarUrl = "uau";
        }
    }
}
