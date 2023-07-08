using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.DTOs;

public class BookDTo
{
    public int Id { get; set; }
    public string Title { get; set; }
    
}