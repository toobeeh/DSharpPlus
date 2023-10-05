namespace DSharpPlus.Net;

public class OAuthEndpoints
{
    public const string BaseUri = "https://discord.com/api/v10";
    public const string Authorize = "https://discord.com/oauth2/authorize";
    public const string Token = $"{BaseUri}/oauth2/token";
    public const string Revoke = $"{BaseUri}/oauth2/token/revoke";
    public const string CurrentApplication = $"{BaseUri}/oauth2/applications/@me";
    public const string CurrentAuthentication = $"{BaseUri}/oauth2/@me";
}
