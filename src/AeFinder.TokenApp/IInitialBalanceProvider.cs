using Volo.Abp.DependencyInjection;

namespace AeFinder.TokenApp;

public interface IInitialBalanceProvider
{
    List<string> GetInitialBalances(string chainId, long blockHeight);
}

public class InitialBalanceProvider : IInitialBalanceProvider, ISingletonDependency
{
    private readonly Dictionary<string, Dictionary<long, List<string>>> _initialBalances = new();

    public InitialBalanceProvider()
    {
        InitBalance();
    }

    public List<string> GetInitialBalances(string chainId, long blockHeight)
    {
        if (_initialBalances.TryGetValue(chainId, out var chainBalances) &&
            chainBalances.TryGetValue(blockHeight, out var balances))
        {
            return balances;
        }

        return new List<string>();
    }

    private void InitBalance()
    {
        
    }
}