using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Utils;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {

        #region GetMapper

        CreateMap<Author, AuthorDto>();
        
        CreateMap<Author, AuthorDToWithBooks>()
            .ForMember(x => x.Books,
                    options => options.MapFrom(MapAuthorDToBooks));

        CreateMap<Book, BookDTo>();
        
        CreateMap<Book, BookDToWithAuthors>()
            .ForMember(book => book.Authors, 
                    options => options.MapFrom(MapAuthorBooks));
        
        CreateMap<Comment, CommentDTo>();
        #endregion

        #region PostMapper
            CreateMap<AuthorCreationDTo, Author>();
            CreateMap<BookCreationDTo, Book>()
                .ForMember(x => x.AuthorsBooks, 
                    options => options.MapFrom(MapAuthorBooks));
            CreateMap<CommentCreationDTo, Comment>();
        #endregion
        
    }

    private static List<AuthorDto> MapAuthorBooks(Book book, BookDTo bookDTo)
    {
        var newAuthorDToList = new List<AuthorDto>();
        if (book.AuthorsBooks.Count == 0 || book.AuthorsBooks is null) 
            return newAuthorDToList;

        newAuthorDToList
            .AddRange(book.AuthorsBooks
                .Select(item => new AuthorDto() { Id = item.AuthorId, Name = item.Author.Name }));

        return newAuthorDToList;

    }

    private static List<BookDTo> MapAuthorDToBooks(Author author, AuthorDto authorDto)
    {
        var booksMapped = new List<BookDTo>();
        if (author.AuthorsBooks is null) return booksMapped;

        booksMapped
            .AddRange(author.AuthorsBooks
                .Select(book => new BookDTo() { Id = book.BookId, Title = book.Book.Title }));

        return booksMapped;
    }

    private static List<AuthorBook> MapAuthorBooks(BookCreationDTo bookCreationDTo, Book book)
    {
        var newRelationalAuthorWithBook = new List<AuthorBook>();

        if (bookCreationDTo.AuthorsIds is null) return newRelationalAuthorWithBook;

        newRelationalAuthorWithBook
            .AddRange(bookCreationDTo.AuthorsIds
                .Select(authorId => new AuthorBook() { AuthorId = authorId }));

        return newRelationalAuthorWithBook;
    }
}