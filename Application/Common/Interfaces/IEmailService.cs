using Application.Models;

namespace Application.Interfaces; 
public interface IEmailService {
    Task SendEmail(string? email, EmailSettings mail);
} 
