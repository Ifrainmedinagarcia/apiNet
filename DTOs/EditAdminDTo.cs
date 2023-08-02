using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class EditAdminDTo
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}