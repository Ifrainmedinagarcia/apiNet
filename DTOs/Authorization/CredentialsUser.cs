using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CredentialsUser
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}