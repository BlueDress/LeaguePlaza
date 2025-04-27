using Newtonsoft.Json;

namespace LeaguePlaza.Infrastructure.Dropbox.Models.ResponseData
{
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = null!;

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = null!;

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
