namespace API.DTOs;

public class BookDToWithAuthors : BookDTo
{
    public List<AuthorDto> Authors { get; set; }
    public List<CommentDTo> Comments { get; set; }
}