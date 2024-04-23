using AeFinder.Sdk.Dtos;

namespace AeFinder.TokenApp.GraphQL;

public class AccountTokenDto : AeFinderEntityDto
{
    public string Address { get; set; }
    public TokenBaseDto Token { get; set; }
    public long Amount { get; set; }
    public decimal FormatAmount { get; set; }
    public long TransferCount { get; set; }
    public string FirstNftTransactionId { get; set; }
    public DateTime? FirstNftTime { get; set; }
}