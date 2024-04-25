using AeFinder.TokenApp.GraphQL;
using AElf.Contracts.MultiToken;
using Shouldly;
using Xunit;

namespace AeFinder.TokenApp.Processors;

public class IssuedAndBurnedProcessorTests : TokenContractAppTestBase
{
    private readonly BurnedProcessor _burnedProcessor;
    
    public IssuedAndBurnedProcessorTests()
    {
        _burnedProcessor = GetRequiredService<BurnedProcessor>();
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
        token[0].Supply.ShouldBe(100);
        token[0].Issued.ShouldBe(100);
        token[0].HolderCount.ShouldBe(1);
        token[0].TransferCount.ShouldBe(1);
        
        var account = await Query.AccountInfo(AccountInfoReadOnlyRepository, ObjectMapper, new GetAccountInfoDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58()
        });
        account[0].TransferCount.ShouldBe(1);
        account[0].TokenHoldingCount.ShouldBe(1);
        
        var accountToken = await Query.AccountToken(AccountTokenReadOnlyRepository, ObjectMapper,new GetAccountTokenDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58(),
            Symbol = "ELF"
        });
        accountToken[0].Amount.ShouldBe(100);
        accountToken[0].FormatAmount.ShouldBe(0.000001m);
        accountToken[0].TransferCount.ShouldBe(1);
        
        var transfer = await Query.TransferInfo(TransferInfoReadOnlyRepository, ObjectMapper, new GetTransferDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58()
        });
        transfer.Count.ShouldBe(1);
        transfer[0].TransactionId.ShouldBe(TransactionId);
        transfer[0].From.ShouldBeNull();
        transfer[0].To.ShouldBe(TestAddress.ToBase58());
        transfer[0].Method.ShouldBe("Issue");
        transfer[0].Amount.ShouldBe(100);
        transfer[0].FormatAmount.ShouldBe(0.000001m);
        transfer[0].Token.Symbol.ShouldBe("ELF");
        transfer[0].Memo.ShouldBe("memo");
        transfer[0].FromChainId.ShouldBeNull();
        transfer[0].ToChainId.ShouldBeNull();
        transfer[0].IssueChainId.ShouldBeNull();
        transfer[0].ParentChainHeight.ShouldBe(0);
        transfer[0].TransferTransactionId.ShouldBeNull();
        
        var burned = new Burned
        {
            Amount = 10,
            Symbol = "ELF",
            Burner = TestAddress
        };
        var logEventContext = GenerateLogEventContext(burned);
        await _burnedProcessor.ProcessAsync(logEventContext);
        await SaveDataAsync();

        token = await Query.TokenInfo(TokenInfoReadOnlyRepository, ObjectMapper, new GetTokenInfoDto
        {
            ChainId = logEventContext.ChainId,
            Symbol = burned.Symbol
        });
        token[0].Supply.ShouldBe(90);
        token[0].Issued.ShouldBe(100);
        token[0].HolderCount.ShouldBe(1);
        token[0].TransferCount.ShouldBe(2);
        
        account = await Query.AccountInfo(AccountInfoReadOnlyRepository, ObjectMapper, new GetAccountInfoDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58()
        });
        account[0].TransferCount.ShouldBe(2);
        account[0].TokenHoldingCount.ShouldBe(1);

        accountToken = await Query.AccountToken(AccountTokenReadOnlyRepository, ObjectMapper,new GetAccountTokenDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58(),
            Symbol = "ELF"
        });
        accountToken[0].Amount.ShouldBe(90);
        accountToken[0].FormatAmount.ShouldBe(0.0000009m);
        accountToken[0].TransferCount.ShouldBe(2);
        
        transfer = await Query.TransferInfo(TransferInfoReadOnlyRepository, ObjectMapper, new GetTransferDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58()
        });
        transfer.Count.ShouldBe(2);
        transfer[1].TransactionId.ShouldBe(TransactionId);
        transfer[1].From.ShouldBe(TestAddress.ToBase58());
        transfer[1].To.ShouldBeNull();
        transfer[1].Method.ShouldBe("Burn");
        transfer[1].Amount.ShouldBe(10);
        transfer[1].FormatAmount.ShouldBe(0.0000001m);
        transfer[1].Token.Symbol.ShouldBe("ELF");
        transfer[1].Memo.ShouldBeNull();
        transfer[1].FromChainId.ShouldBeNull();
        transfer[1].ToChainId.ShouldBeNull();
        transfer[1].IssueChainId.ShouldBeNull();
        transfer[1].ParentChainHeight.ShouldBe(0);
        transfer[1].TransferTransactionId.ShouldBeNull();
    }
}