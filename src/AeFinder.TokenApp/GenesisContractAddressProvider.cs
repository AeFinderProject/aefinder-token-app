using Volo.Abp.DependencyInjection;

namespace AeFinder.TokenApp;

public interface ITokenContractAddressProvider
{
    string GetContractAddress(string chainId);
}

public class TokenContractAddressProvider : ITokenContractAddressProvider, ISingletonDependency
{
    private readonly Dictionary<string, string> _contractAddresses = new()
    {
        { "AELF", "" },
        { "tDVV", "" }
    };

    public string GetContractAddress(string chainId)
    {
        return _contractAddresses[chainId];
    }
}