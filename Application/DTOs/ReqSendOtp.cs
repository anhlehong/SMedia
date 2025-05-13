using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class ReqSendOtp
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string UserName { get; set; }
}