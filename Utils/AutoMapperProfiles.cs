using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Utils;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        //Get
        CreateMap<Author, AuthorDto>();
        CreateMap<Book, BookDTo>();
        
        //Post
        CreateMap<AuthorCreationDTo, Author>();
        CreateMap<BookCreationDTo, Book>();
    }
}