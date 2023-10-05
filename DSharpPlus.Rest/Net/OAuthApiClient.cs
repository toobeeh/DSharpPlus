namespace DSharpPlus.Net;

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Entities;
using Newtonsoft.Json;

/// <summary>
/// Wrapper for the OAuth API specific to for one application.
/// </summary>
public class OAuthApiClient
{
    private ulong _clientId;
    private string _clientSecret;
    private string _redirectUri;
    
    private HttpClient _httpClient;
    
    public OAuthApiClient(ulong clientId, string clientSecret, string redirectUri)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _redirectUri = redirectUri;
        
        _httpClient = new HttpClient();
        string authHeaderValue = Utilities.Base64Encode($"{clientId}:{clientSecret}");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
    }
    
    public async ValueTask<DiscordTokenResponse> GetTokenAsync(string code)
    {
        FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            {"grant_type", "authorization_code"},
            {"code", code},
            {"redirect_uri", _redirectUri}
        });
        
        HttpResponseMessage response = await _httpClient.PostAsync(OAuthEndpoints.Token, content);
        string responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<DiscordTokenResponse>(responseContent);
    }
}
