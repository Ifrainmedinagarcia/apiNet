using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class BookCreationDTo
{
    [Required]
    public string Title { get; set; }
}