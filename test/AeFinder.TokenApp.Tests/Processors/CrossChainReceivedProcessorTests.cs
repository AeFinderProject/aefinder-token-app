using AeFinder.TokenApp.GraphQL;
using AElf;
using AElf.Contracts.MultiToken;
using AElf.Types;
using Shouldly;
using Xunit;

namespace AeFinder.TokenApp.Processors;

public class CrossChainReceivedProcessorTests: TokenContractAppTestBase
{
    private readonly CrossChainReceivedProcessor _crossChainReceivedProcessor;

    public CrossChainReceivedProcessorTests()
    {
        _crossChainReceivedProcessor = GetRequiredService<CrossChainReceivedProcessor>();
    }

    [Fact]
    public async Task HandleEvent_Test()
    {
        await CreateTokenAsync();

        var @event = new CrossChainReceived
        {
            Amount = 1,
            From = TestAddress,
            Symbol = "ELF",
            To = Address.FromBase58("2XDRhxzMbaYRCTe3NxRpARkBpjfQpyWdBKscQpc3Tph3m6dqHG"),
            Memo = "memo",
            IssueChainId = 1,
            FromChainId = 1,
            ParentChainHeight = 100,
            TransferTransactionId = Hash.LoadFromHex("cd29ff43ce541c76752638cbc67ce8d4723fd5142cacffa36a95a40c93d30a4c")
        };
        var logEventContext = GenerateLogEventContext(@event);
        await _crossChainReceivedProcessor.ProcessAsync(logEventContext);
        await SaveDataAsync();
        
        var token = await Query.TokenInfo(TokenInfoReadOnlyRepository, ObjectMapper, new GetTokenInfoDto
        {
            ChainId = ChainId,
            Symbol = @event.Symbol
        });
        token[0].Supply.ShouldBe(101);
        token[0].TransferCount.ShouldBe(2);
        token[0].HolderCount.ShouldBe(2);
        
        var transfer = await Query.TransferInfo(TransferInfoReadOnlyRepository, ObjectMapper, new GetTransferDto
        {
            ChainId = ChainId,
            Address = @event.To.ToBase58()
        });
        transfer.Count.ShouldBe(1);
        transfer[0].TransactionId.ShouldBe(TransactionId);
        transfer[0].From.ShouldBe(TestAddress.ToBase58());
        transfer[0].To.ShouldBe(@event.To.ToBase58());
        transfer[0].Method.ShouldBe("CrossChainReceive");
        transfer[0].Amount.ShouldBe(1);
        transfer[0].FormatAmount.ShouldBe((decimal)0.00000001);
        transfer[0].Token.Symbol.ShouldBe(@event.Symbol);
        transfer[0].Memo.ShouldBe(@event.Memo);
        transfer[0].FromChainId.ShouldBe(ChainHelper.ConvertChainIdToBase58(@event.FromChainId));
        transfer[0].ToChainId.ShouldBe(logEventContext.ChainId);
        transfer[0].IssueChainId.ShouldBe(ChainHelper.ConvertChainIdToBase58(@event.IssueChainId));
        transfer[0].ParentChainHeight.ShouldBe(@event.ParentChainHeight);
        transfer[0].TransferTransactionId.ShouldBe(@event.TransferTransactionId.ToHex());
        
        var account = await Query.AccountInfo(AccountInfoReadOnlyRepository, ObjectMapper, new GetAccountInfoDto
        {
            ChainId = ChainId,
            Address = @event.To.ToBase58()
        });
        account[0].TransferCount.ShouldBe(1);
        account[0].TokenHoldingCount.ShouldBe(1);

        var accountToken = await Query.AccountToken(AccountTokenReadOnlyRepository, ObjectMapper,new GetAccountTokenDto
        {
            ChainId = ChainId,
            Address = @event.To.ToBase58(),
            Symbol = @event.Symbol
        });
        accountToken[0].Amount.ShouldBe(1);
        accountToken[0].FormatAmount.ShouldBe((decimal)0.00000001);
        accountToken[0].TransferCount.ShouldBe(1);
        accountToken[0].Token.Symbol.ShouldBe(@event.Symbol);
    }
}