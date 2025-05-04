using Application.Models;

namespace Application.Common.Interfaces; 
public interface IEmailService {
    Task SendEmail(string? email, EmailSettings mail);
} 
