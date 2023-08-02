using API.DTOs;
using API.Entities;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers.v1;

[ApiController]
[Route("api/v1/books")]
public class BookController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BookController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet(Name = "GetBooks")]
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

    [HttpGet("{title}", Name = "GetBookByName")]
    public async Task<ActionResult<List<BookDTo>>> GetBookByName(string title)
    {
        var bookDb = await _context.Books.Where(x => x.Title == title).ToListAsync();
        var book = _mapper.Map<List<BookDTo>>(bookDb);
        return Ok(book);
    }

    [HttpPost(Name = "CreateBook")]
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

    [HttpPut("{id:int}", Name = "UpdateBook")]
    public async Task<ActionResult> Put(int id, BookCreationDTo bookCreationDTo)
    {
        try
        {
            var bookDb = await _context.Books
                .Include(x => x.AuthorsBooks)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (bookDb is null)
                return NotFound();

            _mapper.Map(bookCreationDTo, bookDb);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPatch("{id:int}", Name = "UpdateBookPartial")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDTo> patchDocument)
    {
        try
        {
            if (patchDocument is null)
                return BadRequest();

            var bookDb = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (bookDb is null)
                return NotFound();

            var bookDTo = _mapper.Map<BookPatchDTo>(bookDb);
            
            patchDocument.ApplyTo(bookDTo, ModelState);

            var isValid = TryValidateModel(bookDb);

            if (!isValid)
                return BadRequest(ModelState);

            _mapper.Map(bookDTo, bookDb);

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpDelete("{id:int}", Name = "DeleteBook")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var isExist = await _context.Books.AnyAsync(x => x.Id == id);
            if (!isExist) return BadRequest("You are trying to delete a book that doesn't exist");

            var bookToDelete = new Book() { Id = id };

            _context.Remove(bookToDelete);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}