namespace API.Entities;

public class AuthorBook
{
    public int AuthorId { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public Author Author { get; set; }
}