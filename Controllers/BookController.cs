using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("/api/books")]
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
    public async Task<ActionResult<List<BookDTo>>> Get()
    {
        try
        {
            var bookDb = await _context.Books.ToListAsync();
            var book = _mapper.Map<List<BookDTo>>(bookDb);
            return Ok(book);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<BookDTo>> GetById(int id)
    {
        try
        {
            var bookDb = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
            var book = _mapper.Map<BookDTo>(bookDb);
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
            var theBookDoesExist = await _context.Books.AnyAsync(x=> x.Title == bookCreationDTo.Title);
            if(theBookDoesExist) 
                return BadRequest();

            var newBook = _mapper.Map<Book>(bookCreationDTo);
            _context.Add(newBook);
            await _context.SaveChangesAsync();

            return Ok();

        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public async Task<ActionResult> Put(int id, BookCreationDTo bookCreationDTo)
    {
        try
        {
            var isExist = await _context.Books.AnyAsync(x => x.Id == id);
            if (!isExist)
                return BadRequest("You are trying to update a book that doesn't exist");

            var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
            book.Title = bookCreationDTo.Title;
                
            await _context.SaveChangesAsync();
            
            return Ok();

        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
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