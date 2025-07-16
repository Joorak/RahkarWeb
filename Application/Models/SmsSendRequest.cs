
using Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Models;
public class SmsSendRequest
{
    public string MobileNumber { get; set; }

    public string SmsContent { get; set; }
    public Priority SendPriority { get; set; }

    
} 
