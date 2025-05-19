
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Models;
public class JwtTokenResponse {
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; } = null;


    [JsonPropertyName("scope")]
    public string Scope { get; set; }


    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    public string? Type => "Bearer";

    public bool Successful { get; set; } = false;

    public string? Error { get; set; } = null;

    public static JwtTokenResponse Failure(string error)
    {
        return new JwtTokenResponse { Error = error };
    }
} 
