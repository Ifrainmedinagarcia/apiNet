using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers.v1;

[ApiController]
[Route("api/v1/counts")]
public class CountsController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly HashService _hashService;
    private readonly IDataProtector _dataProtectionProvider;

    public CountsController(UserManager<IdentityUser> userManager, 
        IConfiguration configuration, 
        SignInManager<IdentityUser> signInManager, 
        IDataProtectionProvider dataProtectionProvider,
        HashService hashService
    )
    {
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
        _hashService = hashService;
        _dataProtectionProvider = dataProtectionProvider.CreateProtector("unique_value_and_secret");
    }

    [HttpGet("{hashPlaneText}")]
    public ActionResult ToDoHash(string hashPlaneText)
    {
        var resultOne = _hashService.Hash(hashPlaneText);
        var resultTwo = _hashService.Hash(hashPlaneText);
        return Ok(new
        {
            hashPlaneText,
            hashOne = resultOne,
            hashTwo = resultTwo ,
        });
    }

    [HttpGet("Encrypt", Name = "GetEncrypt")]
    public ActionResult Encrypt()
    {
        const string planeText = "Ifrain Medina";
        var encryptedText = _dataProtectionProvider.Protect(planeText);
        var unEncrypted = _dataProtectionProvider.Unprotect(encryptedText);
        
        return Ok(new
        {
            planeText,
            encryptedText,
            unEncrypted
        });
    }
    
    [HttpGet("EncryptByTime", Name = "GetEncryptByTime")]
    public ActionResult EncryptByTime()
    {
        var protectLimitByTime = _dataProtectionProvider.ToTimeLimitedDataProtector();
        const string planeText = "Ifrain Medina";
        var encryptedText = protectLimitByTime.Protect(planeText, lifetime: TimeSpan.FromMicroseconds(5));
        Thread.Sleep(6000);
        var unEncrypted = protectLimitByTime.Unprotect(encryptedText);
        
        return Ok(new
        {
            planeText,
            encryptedText,
            unEncrypted
        });
    }

    [HttpPost("register", Name = "UserRegister")]
    public async Task<ActionResult<ResponseAuthentication>> Register(CredentialsUser credentialsUser)
    {

        var user = new IdentityUser() { UserName = credentialsUser.Email, Email = credentialsUser.Email };
        var result = await _userManager.CreateAsync(user, credentialsUser.Password);
        if (result.Succeeded)
            return await BuildToken(credentialsUser);

        return BadRequest(result.Errors);
    }

    [HttpPost("login", Name = "UserLogin")]
    public async Task<ActionResult<ResponseAuthentication>> Login(CredentialsUser credentialsUser)
    {
        var result = await _signInManager.PasswordSignInAsync(
            credentialsUser.Email, credentialsUser.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
            return await BuildToken(credentialsUser);

        return BadRequest("Something error");
    }

    [HttpGet("refreshToke", Name = "RefreshToken")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<ResponseAuthentication>> RefreshToken()
    {
        var emailClaims = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "email");
        var email = emailClaims?.Value;
        
        var userCredentials = new CredentialsUser()
        {
            Email = email 
        };
        
        return await BuildToken(userCredentials);
    }

    [HttpPost("assignRoleAdmin", Name = "AssignRoles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> ToMakeAdmin(EditAdminDTo editAdminDTo)
    {
        var user = await _userManager.FindByEmailAsync(editAdminDTo.Email);
        await _userManager.AddClaimAsync(user!, new Claim("IsAdmin", "1"));
        return NoContent();
    }
    
    [HttpPost("removeRoleAdmin", Name = "RemoveRoles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> RemoveAdmin(EditAdminDTo editAdminDTo)
    {
        var user = await _userManager.FindByEmailAsync(editAdminDTo.Email);
        await _userManager.RemoveClaimAsync(user!, new Claim("IsAdmin", "1"));
        return NoContent();
    }

    private async Task<ResponseAuthentication> BuildToken(CredentialsUser credentialsUser)
    {
        var claims = new List<Claim>()
        {
            new("email", credentialsUser.Email)
        };

        var user = await _userManager.FindByEmailAsync(credentialsUser.Email);
        var claimsDb = await _userManager.GetClaimsAsync(user!);

        claims.AddRange(claimsDb);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["KeyJwt"] ?? string.Empty));
        var creeds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresTime = DateTime.UtcNow.AddYears(1);

        var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims, expires: expiresTime,
            signingCredentials: creeds);

        return new ResponseAuthentication()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
            Expiration = expiresTime
        };
    }
}