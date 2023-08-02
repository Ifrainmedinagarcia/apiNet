using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.DTOs;

public class AuthorDto : Resource
{
    public int Id { get; set; }
    public string Name { get; set; }
}