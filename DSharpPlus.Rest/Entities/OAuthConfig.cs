namespace DSharpPlus.Entities;

public class OAuthConfig
{
    public ulong ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RedirectUri { get; set; }
    public bool AutomaticRefresh { get; set; } = true;
    
}
