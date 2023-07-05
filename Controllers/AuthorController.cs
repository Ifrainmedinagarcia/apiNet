using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AuthorController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuthorDto>>> Get()
    {
        try
        {
            var authorsDb = await _context.Authors.ToListAsync();
            var authors = _mapper.Map<List<AuthorDto>>(authorsDb);
            return Ok(authors);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<AuthorDto>> GeyById(int id)
    {
        try
        {
            var authorDb = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);
            if (authorDb is null)
            {
                return NotFound($"The Author with Id {id} doesnt exist");
            }
            var author = _mapper.Map<AuthorDto>(authorDb);
            return Ok(author);

        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPost]
    public async Task<ActionResult> Post(AuthorCreationDTo authorCreationDTo)
    {
        try
        {
            var author = _mapper.Map<Author>(authorCreationDTo);
            _context.Add(author);
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
    public async Task<ActionResult> Put(int id, AuthorCreationDTo authorCreationDTo)
    {
        try
        {
            var isExist = await _context.Authors.AnyAsync(x => x.Id == id);
            if (!isExist)
            {
                return NotFound($"You are trying to update a register that doesnt exist");
            }

            var author = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);

            author.Name = authorCreationDTo.Name;
            
            await _context.SaveChangesAsync();
            return Ok();

        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpDelete]
    [Route("{id>int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var isExist = await _context.Authors.AnyAsync(x => x.Id == id);
            if (!isExist)
            {
                return NotFound($"You are trying to delete an Author that doesnt exist");
            }
            _context.Remove(new Author() { Id = id });
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}