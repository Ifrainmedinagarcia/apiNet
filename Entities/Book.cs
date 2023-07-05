namespace API.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<Comment> Comments { get; set; }
}