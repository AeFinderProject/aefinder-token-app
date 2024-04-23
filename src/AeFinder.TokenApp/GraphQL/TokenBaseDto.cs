using AeFinder.Sdk.Dtos;
using AeFinder.TokenApp.Entities;

namespace AeFinder.TokenApp.GraphQL;

public class TokenBaseDto : AeFinderEntityDto
{
    public string Symbol { get; set; }
    public string CollectionSymbol { get; set; }
    public SymbolType Type { get; set; }
    public int Decimals { get; set; }
}