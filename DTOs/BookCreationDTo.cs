using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class BookCreationDTo
{
    [Required]
    public string Title { get; set; }
    public DateTime DateUpdated { get; set; }
    public List<int> AuthorsIds { get; set; }
}