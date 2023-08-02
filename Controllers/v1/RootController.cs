using API.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

[ApiController]
[Route("api")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RootController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    public RootController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }
    
    [HttpGet(Name = "GetRoot")]
    [AllowAnonymous]
    public async Task<ActionResult<List<DataHATEOAS>>> Get()
    {
        var dataHateoas = new List<DataHATEOAS>();

        var isAdmin = await _authorizationService.AuthorizeAsync(User, "IsAdmin");
        
        dataHateoas.Add(new DataHATEOAS(
            link: Url.Link("GetRoot", new { }), 
            description: "self", method: "GET"));

        #region Authors
            dataHateoas.Add(new DataHATEOAS(
                link: Url.Link("GetAuthors", new { }),
                description: "authors", method: "GET"));
            
            dataHateoas.Add(new DataHATEOAS(
                link: Url.Link("CreateAuthor", new { }),
                description: "Create-author", method: "POST"));
        #endregion

        #region Books
            dataHateoas.Add(new DataHATEOAS(
                link: Url.Link("GetBooks", new { }),
                description: "Books", method: "GET"));
            
            dataHateoas.Add(new DataHATEOAS(
                link: Url.Link("CreateBook", new { }),
                description: "Create-book", method: "POST"));
        #endregion

        #region Comments
            /*dataHateoas.Add(new DataHATEOAS(
                link: Url.Link("GetComments", new { }),
                description: "Comments", method: "GET"));
                
            dataHateoas.Add(new DataHATEOAS(
                link: Url.Link("CreateComment", new { }),
                description: "Create-comment", method: "POST"));*/
        #endregion

        #region Counts
            dataHateoas.Add(new DataHATEOAS(
                link: Url.Link("UserRegister", new { }),
                description: "Register", method: "POST"));
                
            dataHateoas.Add(new DataHATEOAS(
                link: Url.Link("UserLogin", new { }),
                description: "Login-user", method: "POST"));
        #endregion

        return dataHateoas;
    }
}