using AeFinder.TokenApp.Entities;

namespace AeFinder.TokenApp.GraphQL;

public class GetTokenInfoDto : PagedResultQueryDto
{
    public string ChainId { get; set; }
    public string Symbol { get; set; }
    public string PartialSymbol { get; set; }
    public string TokenName { get; set; }
    public string PartialTokenName { get; set; }
    public string Owner { get; set; }
    public string Issuer { get; set; }
    public List<SymbolType> Types { get; set; } = new();
}