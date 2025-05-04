using System; 
namespace Application.Common.Security; 
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)] 
public class AuthorizeAttribute : Attribute { 
    public string[] Roles { get; set; } = Array.Empty<string>(); 
    public AuthorizeAttribute(params string[] roles) { Roles = roles; } 
} 
