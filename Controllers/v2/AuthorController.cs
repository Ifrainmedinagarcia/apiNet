using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.v2;

[ApiController]
[Route("api/v2/authors")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
public class AuthorController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthorController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
    }
    
    [HttpGet(Name = "GetAuthorsv2")]
    [AllowAnonymous]
    public async Task<ActionResult<List<AuthorDToWithBooks>>> Get()
    {
        try
        {
            var authorsDb = await _context.Authors
                .Include(x => x.AuthorsBooks)
                .ThenInclude(x => x.Book)
                .ToListAsync();
            var authors = _mapper.Map<List<AuthorDToWithBooks>>(authorsDb);
            authors.ForEach(author => author?.Name.ToUpper());
            return Ok(authors);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpGet("{id:int}", Name = "GetAuthorById2")]
    public async Task<ActionResult<AuthorDToWithBooks>> GeyById(int id)
    {
        try
        {
            var authorDb = await _context.Authors
                .Include(x => x.AuthorsBooks)
                .ThenInclude(x => x.Book)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (authorDb is null)
            {
                return NotFound($"The Author with Id {id} doesnt exist");
            }
            var author = _mapper.Map<AuthorDToWithBooks>(authorDb);
            return Ok(author);

        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpGet("{name}", Name = "GetAuthorByNamev2")]
    public async Task<ActionResult<List<AuthorDToWithBooks>>> GeyByName(string name)
    {
        var authorDb = await _context.Authors.Where(x => x.Name == name).ToListAsync();
        var author = _mapper.Map<List<AuthorDToWithBooks>>(authorDb);
        return Ok(author);
    }

    [HttpPost(Name = "CreateAuthorv2")]
    public async Task<ActionResult> Post(AuthorCreationDTo authorCreationDTo)
    {
        try
        {
            var author = _mapper.Map<Author>(authorCreationDTo);
            _context.Add(author);
            await _context.SaveChangesAsync();
            
            var authorDTo = _mapper.Map<AuthorDto>(author);
            return CreatedAtRoute("GetAuthorById ", new { id = author.Id }, authorDTo);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPut("{id:int}", Name = "UpdateAuthorv2")]
    public async Task<ActionResult> Put(int id, AuthorCreationDTo authorCreationDTo)
    {
        try
        {
            var isExist = await _context.Authors.AnyAsync(x => x.Id == id);
            if (!isExist)
            {
                return NotFound($"You are trying to update a register that doesn't exist");
            }

            var author = _mapper.Map<Author>(authorCreationDTo);
            
            author.Id = id;

            _context.Update(author);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpDelete("{id:int}", Name = "DeleteAuthorv2")]
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
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
    
    /*private void GenerateLinks(AuthorDto authorDto)
    {
        authorDto.Links.Add(new DataHATEOAS(
            link: Url.Link("GetAuthors", new {id = authorDto.Id}), 
            description: "self", method: "GET"));
        
        authorDto.Links.Add(new DataHATEOAS(
            link: Url.Link("CreateAuthor", new { }), 
            description: "create-author", method: "POST"));
        
        authorDto.Links.Add(new DataHATEOAS(
            link: Url.Link("UpdateAuthor", new {id = authorDto.Id}), 
            description: "author-update", method: "PUT"));
        
        authorDto.Links.Add(new DataHATEOAS(
            link: Url.Link("DeleteAuthor", new {id = authorDto.Id}), 
            description: "author-delete", method: "DELETE"));
    }*/
}