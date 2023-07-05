using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class AuthorCreationDTo
{
    [Required]
    public string Name { get; set; }
}