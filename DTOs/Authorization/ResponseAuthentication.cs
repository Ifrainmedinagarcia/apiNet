namespace API.DTOs;

public class ResponseAuthentication
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}