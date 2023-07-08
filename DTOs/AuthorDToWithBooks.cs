namespace API.DTOs;

public class AuthorDToWithBooks : AuthorDto
{
    public List<BookDTo> Books { get; set; }
}