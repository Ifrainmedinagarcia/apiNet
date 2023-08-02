namespace API.DTOs;

public class PaginationDTo
{
    public int Page { get; set; } = 1;
    private int _recordsPerPage = 10;
    private const int CountMaxPerPage = 50;

    public int RecordsPerPage
    {
        get => _recordsPerPage;
        set => _recordsPerPage = (value > CountMaxPerPage) ? CountMaxPerPage : value;
    }
}