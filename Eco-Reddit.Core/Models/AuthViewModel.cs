using Newtonsoft.Json;

namespace Selva.Core.Models
{
    public class AuthViewModel
    {
        [JsonProperty("access_token")] public string AccessToken;

        [JsonProperty("expires_in")] public int ExpiresIn;

        [JsonProperty("refresh_token")] public string RefreshToken;

        [JsonProperty("scope")] public string Scope;

        [JsonProperty("token_type")] public string TokenType;
    }
}
