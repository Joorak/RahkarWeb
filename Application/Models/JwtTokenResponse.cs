
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

    
} 
