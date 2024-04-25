using Orleans.TestingHost;
using AeFinder.TokenApp.TestBase;
using Volo.Abp.Modularity;

namespace AeFinder.TokenApp.Orleans.TestBase;

public abstract class AeFinderTokenAppOrleansTestBase<TStartupModule>:AeFinderTokenAppTestBase<TStartupModule> 
    where TStartupModule : IAbpModule
{
    protected readonly TestCluster Cluster;

    public AeFinderTokenAppOrleansTestBase()
    {
        Cluster = GetRequiredService<ClusterFixture>().Cluster;
    }
}