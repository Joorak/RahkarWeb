namespace Application.Exceptions; 
public class ForbiddenAccessException : Exception { 
    public ForbiddenAccessException() : base("You do not have permission to access this resource.") { } 
} 
