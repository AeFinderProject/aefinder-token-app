using AeFinder.TokenApp.GraphQL;
using AElf.Contracts.MultiToken;
using Shouldly;
using Xunit;

namespace AeFinder.TokenApp.Processors;

public class ChainPrimaryTokenSymbolSetProcessorTests: TokenContractAppTestBase
{
    private readonly ChainPrimaryTokenSymbolSetProcessor _chainPrimaryTokenSymbolSetProcessor;

    public ChainPrimaryTokenSymbolSetProcessorTests()
    {
        _chainPrimaryTokenSymbolSetProcessor = GetRequiredService<ChainPrimaryTokenSymbolSetProcessor>();
    }

    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await CreateTokenAsync();
        var token = await Query.TokenInfo(TokenInfoReadOnlyRepository, ObjectMapper, new GetTokenInfoDto
        {
            ChainId = ChainId,
            Symbol = "ELF"
        });
        token[0].IsPrimaryToken.ShouldBeFalse();
        
        var chainPrimaryTokenSymbolSet = new ChainPrimaryTokenSymbolSet
        {
            TokenSymbol = "ELF"
        };
        var logEventContext = GenerateLogEventContext(chainPrimaryTokenSymbolSet);
        await _chainPrimaryTokenSymbolSetProcessor.ProcessAsync(logEventContext);
        await SaveDataAsync();
        
        token = await Query.TokenInfo(TokenInfoReadOnlyRepository, ObjectMapper, new GetTokenInfoDto
        {
            ChainId = ChainId,
            Symbol = "ELF"
        });
        token[0].IsPrimaryToken.ShouldBeTrue();
    }
}