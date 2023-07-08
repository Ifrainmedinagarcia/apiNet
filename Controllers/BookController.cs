using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[Route("api/books")]
public class BookController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BookController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookDToWithAuthors>>> Get()
    {
        try
        {
            var bookDb = await _context.Books
                .Include(book => book.Comments)
                .Include(book=> book.AuthorsBooks)
                .ThenInclude(x => x.Author)
                .ToListAsync();
            
            var book = _mapper.Map<List<BookDToWithAuthors>>(bookDb);
            return Ok(book);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpGet("{id:int}", Name = "GetBookById")]
    public async Task<ActionResult<BookDToWithAuthors>> GetById(int id)
    {
        try
        {
            var bookDb = await _context.Books
                .Include(x => x.Comments)
                .Include(x=> x.AuthorsBooks)
                .ThenInclude(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id == id);
            var book = _mapper.Map<BookDToWithAuthors>(bookDb);
            return Ok(book);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPost]
    public async Task<ActionResult> Post(BookCreationDTo bookCreationDTo)
    {
        try
        {
            var theBookDoesExist = await _context.Books
                .AnyAsync(x=> x.Title == bookCreationDTo.Title);
            if(theBookDoesExist) 
                return BadRequest();

            var newBook = _mapper.Map<Book>(bookCreationDTo);
            _context.Add(newBook);
            await _context.SaveChangesAsync();

            var book = _mapper.Map<BookDTo>(newBook);

            return CreatedAtRoute("GetBookById", new { id = newBook.Id }, book);

        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, BookCreationDTo bookCreationDTo)
    {
        try
        {
            var isExist = await _context.Books
                .AnyAsync(x => x.Id == id);
            
            if (!isExist)
                return BadRequest("You are trying to update a book that doesn't exist");

            var book = _mapper.Map<Book>(bookCreationDTo);
            book.Id = id;
            
            _context.Update(book);
            await _context.SaveChangesAsync();
            
            return NoContent();

        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpDelete(("{id:int}"))]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var isExist = await _context.Books.AnyAsync(x => x.Id == id);
            if (!isExist) return BadRequest("You are trying to delete a book that doesn't exist");

            var bookToDelete = new Book() { Id = id };

            _context.Remove(bookToDelete);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}