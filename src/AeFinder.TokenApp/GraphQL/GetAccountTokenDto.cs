namespace AeFinder.TokenApp.GraphQL;

public class GetAccountTokenDto : PagedResultQueryDto
{
    public string ChainId { get; set; }
    public string Address { get; set; }
    public string Symbol { get; set; }
    public string PartialSymbol { get; set; }
}