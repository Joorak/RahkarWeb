using Newtonsoft.Json;

namespace Application.Models;
public class JwtTokenResponse {
    [JsonProperty("access_token")]
    public string? AccessToken { get; set; } = null;


    [JsonProperty("scope")]
    public string Scope { get; set; }


    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    public string? Type => "Bearer";

    public bool Successful { get; set; } = false;

    public string? Error { get; set; } = null;

    public static JwtTokenResponse Failure(string error)
    {
        return new JwtTokenResponse { Error = error };
    }
} 
