using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CommentCreationDTo
{ 
    [Required]
    public string Text { get; set; }
}