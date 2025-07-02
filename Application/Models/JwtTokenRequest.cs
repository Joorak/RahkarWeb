
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Models;
public class JwtTokenRequest {
    public string AccountId { get; set; }

    public string Role { get; set; }

    
} 
