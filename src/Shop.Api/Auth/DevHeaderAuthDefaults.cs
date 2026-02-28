namespace Shop.Api.Auth;

public static class DevHeaderAuthDefaults
{
    public const string Scheme = "DevHeader";

    public const string HeaderUserId = "X-Dev-UserId";
    public const string HeaderRole = "X-Dev-Role";
    public const string HeaderName = "X-Dev-Name";
}