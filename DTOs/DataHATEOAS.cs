namespace API.DTOs;

public class DataHATEOAS
{
    public string Link { get; private set; }
    public string Description { get; private set; }
    public string Method { get; private set; }

    public DataHATEOAS(string method, string description, string link)
    {
        Method = method;
        Description = description;
        Link = link;
    }
}