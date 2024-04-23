using AeFinder.Sdk.Dtos;

namespace AeFinder.TokenApp.GraphQL;

public class AccountInfoDto : AeFinderEntityDto
{
    public string Address { get; set; }
    public long TokenHoldingCount { get; set; }
    public long TransferCount { get; set; }
}