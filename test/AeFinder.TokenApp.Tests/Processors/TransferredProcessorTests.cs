using AeFinder.TokenApp.GraphQL;
using AElf.Contracts.MultiToken;
using AElf.Types;
using Shouldly;
using Xunit;

namespace AeFinder.TokenApp.Processors;

public class TransferredProcessorTests: TokenContractAppTestBase
{
    private readonly TransferredProcessor _transferredProcessor;

    public TransferredProcessorTests()
    {
        _transferredProcessor = GetRequiredService<TransferredProcessor>();
    }

    [Fact]
    public async Task HandleEvent_Test()
    {
        await CreateTokenAsync();
        

        var transferred = new Transferred
        {
            Amount = 1,
            From = TestAddress,
            Symbol = "ELF",
            To = Address.FromBase58("2XDRhxzMbaYRCTe3NxRpARkBpjfQpyWdBKscQpc3Tph3m6dqHG"),
            Memo = "memo"
        };
        var logEventContext = GenerateLogEventContext(transferred);
        await _transferredProcessor.ProcessAsync(logEventContext);
        await SaveDataAsync();
        
        var token = await Query.TokenInfo(TokenInfoReadOnlyRepository, ObjectMapper, new GetTokenInfoDto
        {
            ChainId = ChainId,
            Symbol = "ELF"
        });
        token[0].HolderCount.ShouldBe(2);
        token[0].TransferCount.ShouldBe(2);
        
        var accountFrom = await Query.AccountInfo(AccountInfoReadOnlyRepository, ObjectMapper, new GetAccountInfoDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58()
        });
        accountFrom[0].TransferCount.ShouldBe(2);
        accountFrom[0].TokenHoldingCount.ShouldBe(1);
        
        var accountTo = await Query.AccountInfo(AccountInfoReadOnlyRepository, ObjectMapper, new GetAccountInfoDto
        {
            ChainId = ChainId,
            Address = transferred.To.ToBase58()
        });
        accountTo[0].TransferCount.ShouldBe(1);
        accountTo[0].TokenHoldingCount.ShouldBe(1);
        
        var accountTokenFrom = await Query.AccountToken(AccountTokenReadOnlyRepository, ObjectMapper,new GetAccountTokenDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58(),
            Symbol = "ELF"
        });
        accountTokenFrom[0].Amount.ShouldBe(99);
        accountTokenFrom[0].FormatAmount.ShouldBe(0.00000099m);
        accountTokenFrom[0].TransferCount.ShouldBe(2);
        accountTokenFrom[0].FirstNftTransactionId.ShouldBeNull();
        accountTokenFrom[0].FirstNftTime.ShouldBeNull();
        
        var accountTokenTo = await Query.AccountToken(AccountTokenReadOnlyRepository, ObjectMapper,new GetAccountTokenDto
        {
            ChainId = ChainId,
            Address = transferred.To.ToBase58(),
            Symbol = "ELF"
        });
        accountTokenTo[0].Amount.ShouldBe(1);
        accountTokenTo[0].FormatAmount.ShouldBe(0.00000001m);
        accountTokenTo[0].TransferCount.ShouldBe(1);
        accountTokenTo[0].FirstNftTransactionId.ShouldBeNull();
        accountTokenTo[0].FirstNftTime.ShouldBeNull();
        
        var transferFrom = await Query.TransferInfo(TransferInfoReadOnlyRepository, ObjectMapper, new GetTransferDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58()
        });
        transferFrom.Count.ShouldBe(2);
        transferFrom[1].TransactionId.ShouldBe(TransactionId);
        transferFrom[1].From.ShouldBe(transferred.From.ToBase58());
        transferFrom[1].To.ShouldBe(transferred.To.ToBase58());
        transferFrom[1].Method.ShouldBe("Transfer");
        transferFrom[1].Amount.ShouldBe(transferred.Amount);
        transferFrom[1].FormatAmount.ShouldBe(0.00000001m);
        transferFrom[1].Token.Symbol.ShouldBe(transferred.Symbol);
        transferFrom[1].Memo.ShouldBe(transferred.Memo);
        transferFrom[1].FromChainId.ShouldBeNull();
        transferFrom[1].ToChainId.ShouldBeNull();
        transferFrom[1].IssueChainId.ShouldBeNull();
        transferFrom[1].ParentChainHeight.ShouldBe(0);
        transferFrom[1].TransferTransactionId.ShouldBeNull();
        
        var transferTo = await Query.TransferInfo(TransferInfoReadOnlyRepository, ObjectMapper, new GetTransferDto
        {
            ChainId = ChainId,
            Address = transferred.To.ToBase58()
        });
        transferTo.Count.ShouldBe(1);
        transferTo[0].TransactionId.ShouldBe(TransactionId);
        transferTo[0].From.ShouldBe(transferred.From.ToBase58());
        transferTo[0].To.ShouldBe(transferred.To.ToBase58());
        transferTo[0].Method.ShouldBe("Transfer");
        transferTo[0].Amount.ShouldBe(transferred.Amount);
        transferTo[0].FormatAmount.ShouldBe(0.00000001m);
        transferTo[0].Token.Symbol.ShouldBe(transferred.Symbol);
        transferTo[0].Memo.ShouldBe(transferred.Memo);
        transferTo[0].FromChainId.ShouldBeNull();
        transferTo[0].ToChainId.ShouldBeNull();
        transferTo[0].IssueChainId.ShouldBeNull();
        transferTo[0].ParentChainHeight.ShouldBe(0);
        transferTo[0].TransferTransactionId.ShouldBeNull();
    }
    
    [Fact]
    public async Task HandleEvent_Collection_Test()
    {
        await CreateCollectionTokenAsync();

        var transferred = new Transferred
        {
            Amount = 1,
            From = TestAddress,
            Symbol = "NFT-0",
            To = Address.FromBase58("2XDRhxzMbaYRCTe3NxRpARkBpjfQpyWdBKscQpc3Tph3m6dqHG"),
            Memo = "memo"
        };
        var logEventContext = GenerateLogEventContext(transferred);
        await _transferredProcessor.ProcessAsync(logEventContext);
        await SaveDataAsync();
        
        var token = await Query.TokenInfo(TokenInfoReadOnlyRepository, ObjectMapper, new GetTokenInfoDto
        {
            ChainId = ChainId,
            Symbol = transferred.Symbol
        });
        token[0].HolderCount.ShouldBe(0);
        token[0].TransferCount.ShouldBe(2);
        
        var accountFrom = await Query.AccountInfo(AccountInfoReadOnlyRepository, ObjectMapper, new GetAccountInfoDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58()
        });
        accountFrom[0].TransferCount.ShouldBe(2);
        accountFrom[0].TokenHoldingCount.ShouldBe(1);
        
        var accountTo = await Query.AccountInfo(AccountInfoReadOnlyRepository, ObjectMapper, new GetAccountInfoDto
        {
            ChainId = ChainId,
            Address = transferred.To.ToBase58()
        });
        accountTo[0].TransferCount.ShouldBe(1);
        accountTo[0].TokenHoldingCount.ShouldBe(1);
        
        var accountTokenFrom = await Query.AccountToken(AccountTokenReadOnlyRepository, ObjectMapper,new GetAccountTokenDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58(),
            Symbol = transferred.Symbol
        });
        accountTokenFrom[0].TransferCount.ShouldBe(2);
        accountTokenFrom[0].FirstNftTransactionId.ShouldBeNull();
        accountTokenFrom[0].FirstNftTime.ShouldBeNull();
        
        var accountTokenTo = await Query.AccountToken(AccountTokenReadOnlyRepository, ObjectMapper,new GetAccountTokenDto
        {
            ChainId = ChainId,
            Address = transferred.To.ToBase58(),
            Symbol = transferred.Symbol
        });
        accountTokenTo[0].TransferCount.ShouldBe(1);
        accountTokenFrom[0].FirstNftTransactionId.ShouldBeNull();
        accountTokenFrom[0].FirstNftTime.ShouldBeNull();
    }
    
    [Fact]
    public async Task HandleEvent_Nft_Test()
    {
        await CreateCollectionTokenAsync();
        await CreateNftTokenAsync();
        

        var collectionSymbol = "NFT-0";
        var transferred = new Transferred
        {
            Amount = 1,
            From = TestAddress,
            Symbol = "NFT-1",
            To = Address.FromBase58("2XDRhxzMbaYRCTe3NxRpARkBpjfQpyWdBKscQpc3Tph3m6dqHG"),
            Memo = "memo"
        };
        var logEventContext = GenerateLogEventContext(transferred);
        await _transferredProcessor.ProcessAsync(logEventContext);
        await SaveDataAsync();
        
        var tokenNft = await Query.TokenInfo(TokenInfoReadOnlyRepository, ObjectMapper, new GetTokenInfoDto
        {
            ChainId = ChainId,
            Symbol = transferred.Symbol
        });
        tokenNft[0].HolderCount.ShouldBe(2);
        tokenNft[0].TransferCount.ShouldBe(2);
        
        var tokenCollection = await Query.TokenInfo(TokenInfoReadOnlyRepository, ObjectMapper, new GetTokenInfoDto
        {
            ChainId = ChainId,
            Symbol = collectionSymbol
        });
        tokenCollection[0].HolderCount.ShouldBe(2);
        tokenCollection[0].TransferCount.ShouldBe(3);
        
        var accountFrom = await Query.AccountInfo(AccountInfoReadOnlyRepository, ObjectMapper, new GetAccountInfoDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58()
        });
        accountFrom[0].TransferCount.ShouldBe(3);
        accountFrom[0].TokenHoldingCount.ShouldBe(2);
        
        var accountTo = await Query.AccountInfo(AccountInfoReadOnlyRepository, ObjectMapper, new GetAccountInfoDto
        {
            ChainId = ChainId,
            Address = transferred.To.ToBase58()
        });
        accountTo[0].TransferCount.ShouldBe(1);
        accountTo[0].TokenHoldingCount.ShouldBe(1);
        
        var accountNftTokenFrom = await Query.AccountToken(AccountTokenReadOnlyRepository, ObjectMapper,new GetAccountTokenDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58(),
            Symbol = transferred.Symbol
        });
        accountNftTokenFrom[0].TransferCount.ShouldBe(2);
        accountNftTokenFrom[0].FirstNftTransactionId.ShouldBe(TransactionId);
        accountNftTokenFrom[0].FirstNftTime.ShouldNotBeNull();
        
        var accountCollectionTokenFrom = await Query.AccountToken(AccountTokenReadOnlyRepository, ObjectMapper,new GetAccountTokenDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58(),
            Symbol = collectionSymbol
        });
        accountCollectionTokenFrom[0].TransferCount.ShouldBe(3);
        accountCollectionTokenFrom[0].FirstNftTransactionId.ShouldBeNull();
        accountCollectionTokenFrom[0].FirstNftTime.ShouldBeNull();

        var accountNftTokenTo = await Query.AccountToken(AccountTokenReadOnlyRepository, ObjectMapper,new GetAccountTokenDto
        {
            ChainId = ChainId,
            Address = transferred.To.ToBase58(),
            Symbol = transferred.Symbol
        });
        accountNftTokenTo[0].TransferCount.ShouldBe(1);
        accountNftTokenTo[0].FirstNftTransactionId.ShouldBe(TransactionId);
        accountNftTokenTo[0].FirstNftTime.ShouldBe(logEventContext.Block.BlockTime);
        
        var accountCollectionTokenTo = await Query.AccountToken(AccountTokenReadOnlyRepository, ObjectMapper,new GetAccountTokenDto
        {
            ChainId = ChainId,
            Address = transferred.To.ToBase58(),
            Symbol = collectionSymbol
        });
        accountCollectionTokenTo[0].TransferCount.ShouldBe(1);
        accountCollectionTokenTo[0].FirstNftTransactionId.ShouldBeNull();
        accountCollectionTokenTo[0].FirstNftTime.ShouldBeNull();
    }
    
    [Fact]
    public async Task HandleEvent_BalanceTo0_Test()
    {
        await CreateTokenAsync();
        

        var transferred = new Transferred
        {
            Amount = 100,
            From = TestAddress,
            Symbol = "ELF",
            To = Address.FromBase58("2XDRhxzMbaYRCTe3NxRpARkBpjfQpyWdBKscQpc3Tph3m6dqHG"),
            Memo = "memo"
        };
        var logEventContext = GenerateLogEventContext(transferred);
        await _transferredProcessor.ProcessAsync(logEventContext);
        await SaveDataAsync();
        
        var token = await Query.TokenInfo(TokenInfoReadOnlyRepository, ObjectMapper, new GetTokenInfoDto
        {
            ChainId = ChainId,
            Symbol = "ELF"
        });
        token[0].HolderCount.ShouldBe(1);
        token[0].TransferCount.ShouldBe(2);
        
        var accountFrom = await Query.AccountInfo(AccountInfoReadOnlyRepository, ObjectMapper, new GetAccountInfoDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58()
        });
        accountFrom[0].TransferCount.ShouldBe(2);
        accountFrom[0].TokenHoldingCount.ShouldBe(0);
        
        var accountTo = await Query.AccountInfo(AccountInfoReadOnlyRepository, ObjectMapper, new GetAccountInfoDto
        {
            ChainId = ChainId,
            Address = transferred.To.ToBase58()
        });
        accountTo[0].TransferCount.ShouldBe(1);
        accountTo[0].TokenHoldingCount.ShouldBe(1);
        
        var accountTokenFrom = await Query.AccountToken(AccountTokenReadOnlyRepository, ObjectMapper,new GetAccountTokenDto
        {
            ChainId = ChainId,
            Address = TestAddress.ToBase58(),
            Symbol = "ELF"
        });
        accountTokenFrom[0].Amount.ShouldBe(0);
        accountTokenFrom[0].FormatAmount.ShouldBe(0);
        accountTokenFrom[0].TransferCount.ShouldBe(2);
        
        var accountTokenTo = await Query.AccountToken(AccountTokenReadOnlyRepository, ObjectMapper,new GetAccountTokenDto
        {
            ChainId = ChainId,
            Address = transferred.To.ToBase58(),
            Symbol = "ELF"
        });
        accountTokenTo[0].Amount.ShouldBe(100);
        accountTokenTo[0].FormatAmount.ShouldBe(0.000001m);
        accountTokenTo[0].TransferCount.ShouldBe(1);
    }
}