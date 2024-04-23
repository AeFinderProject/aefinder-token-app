namespace AeFinder.TokenApp.GraphQL;

public class GetTransferDto : PagedResultQueryDto
{
    public string ChainId { get; set; }
    public string TransactionId { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string Address { get; set; }
    public List<string> Methods { get; set; } = new();
    public string Symbol { get; set; }
}