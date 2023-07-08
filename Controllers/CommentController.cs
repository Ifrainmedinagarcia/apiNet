using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/{bookId:int}/comments")]
public class CommentController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CommentController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<CommentDTo>>> Get(int bookId)
    {
        var comments = await _context.Comments
            .Where(x => x.Id == bookId)
            .ToListAsync();

        var commentsDTo = _mapper.Map<List<CommentDTo>>(comments);
        
        return Ok(commentsDTo);
    }

    [HttpGet("{id:int}", Name = "GetCommentById")]
    public async Task<ActionResult<CommentDTo>> GetById(int id)
    {
        try
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (comment is null) return NotFound();

            var commentDTo = _mapper.Map<CommentDTo>(comment);
            return Ok(commentDTo);

        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPost]
    public async Task<ActionResult> Post(int bookId, CommentCreationDTo commentCreationDTo)
    {
        var bookExist = await _context.Books.AnyAsync(x => x.Id == bookId);
        if (!bookExist) return NotFound("The book doesn't exist");
        
        var newComment = _mapper.Map<Comment>(commentCreationDTo);
        newComment.BookId = bookId;

        _context.Add(newComment);
        await _context.SaveChangesAsync();

        var comment = _mapper.Map<CommentDTo>(newComment);
        
        return CreatedAtRoute("GetCommentById", new {id = newComment.Id, bookId}, comment);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int bookId, int id, CommentCreationDTo commentCreationDTo)
    {
        var isExistComment = await _context.Comments.AnyAsync(x => x.Id == id);
        if (!isExistComment) return NotFound();
        
        var isExistBook = await _context.Books.AnyAsync(x => x.Id == id);
        if (!isExistBook) return NotFound();

        var comment = _mapper.Map<Comment>(commentCreationDTo);

        comment.Id = id;
        comment.BookId = bookId;
        
        _context.Update(comment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}