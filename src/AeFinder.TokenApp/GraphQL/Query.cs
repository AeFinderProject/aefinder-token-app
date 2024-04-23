using AeFinder.Sdk;
using AeFinder.TokenApp.Entities;
using GraphQL;
using Volo.Abp.ObjectMapping;

namespace AeFinder.TokenApp.GraphQL;

public class Query
{
    public static async Task<List<TokenInfoDto>> TokenInfo(
        [FromServices] IReadOnlyRepository<TokenInfo> repository,
        [FromServices] IObjectMapper objectMapper, GetTokenInfoDto input)
    {
        input.Validate();
        
        var queryable = await repository.GetQueryableAsync();

        if (!input.ChainId.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.Metadata.ChainId == input.ChainId);
        }

        if (!input.Symbol.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.Symbol == input.Symbol);
        }
        
        if (!input.TokenName.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.TokenName == input.TokenName);
        }
        
        if (!input.Owner.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.Owner == input.Owner);
        }
        
        if (!input.Issuer.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.Issuer == input.Issuer);
        }
        
        // if (input.Types.Count > 0)
        // {
        //     var shouldQuery = input.Types.Select(type =>
        //         (Func<QueryContainerDescriptor<TokenInfo>, QueryContainer>)(q =>
        //             q.Term(i => i.Field(f => f.Type).Value(type)))).ToList();
        //     mustQuery.Add(q => q.Bool(b => b.Should(shouldQuery)));
        // }
        
        // if(!input.PartialSymbol.IsNullOrWhiteSpace())
        // {
        //     mustQuery.Add(s => s.Wildcard(i => i.Field(f => f.Symbol).Value($"*{input.PartialSymbol}*")));
        // }
        //
        // if(!input.PartialTokenName.IsNullOrWhiteSpace())
        // {
        //     mustQuery.Add(s => s.Wildcard(i => i.Field(f => f.NormalizedTokenName).Value($"*{input.PartialTokenName.ToUpper()}*")));
        // }
        
        var result = queryable.Skip(input.SkipCount)
            .Take(input.MaxResultCount).ToList();
        return objectMapper.Map<List<TokenInfo>, List<TokenInfoDto>>(result);
    }
    
    public static async Task<List<AccountInfoDto>> AccountInfo(
        [FromServices] IReadOnlyRepository<AccountInfo> repository,
        [FromServices] IObjectMapper objectMapper, GetAccountInfoDto input)
    {
        input.Validate();
        
        var queryable = await repository.GetQueryableAsync();

        if (!input.ChainId.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.Metadata.ChainId == input.ChainId);
        }
        
        if (!input.Address.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.Address == input.Address);
        }
        
        
        var result = queryable.Skip(input.SkipCount)
            .Take(input.MaxResultCount).ToList();
        return objectMapper.Map<List<AccountInfo>, List<AccountInfoDto>>(result);
    }
    
    public static async Task<List<AccountTokenDto>> AccountToken(
        [FromServices] IReadOnlyRepository<AccountToken> repository,
        [FromServices] IObjectMapper objectMapper, GetAccountTokenDto input)
    {
        input.Validate();
        
        var queryable = await repository.GetQueryableAsync();

        if (!input.ChainId.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.Metadata.ChainId == input.ChainId);
        }
        
        if (!input.Address.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.Address == input.Address);
        }
        
        if (!input.Symbol.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.Address == input.Address);
        }
        
        // if(!input.PartialSymbol.IsNullOrWhiteSpace())
        // {
        //     mustQuery.Add(s => s.Wildcard(i => i.Field(f => f.Token.Symbol).Value($"*{input.PartialSymbol}*")));
        // }
        
        var result = queryable.Skip(input.SkipCount)
            .Take(input.MaxResultCount).ToList();
        return objectMapper.Map<List<AccountToken>, List<AccountTokenDto>>(result);
    }
    
    public static async Task<List<TransferInfoDto>> TransferInfo(
        [FromServices] IReadOnlyRepository<TransferInfo> repository,
        [FromServices] IObjectMapper objectMapper, GetTransferDto input)
    {
        input.Validate();
        
        var queryable = await repository.GetQueryableAsync();
        
        if (!input.ChainId.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.Metadata.ChainId == input.ChainId);
        }
        
        if (!input.Address.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.From == input.Address || o.To == input.Address);
        }
        
        if (!input.From.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.From == input.From);
        }
        
        if (!input.To.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.From == input.From);
        }
        
        if (!input.Symbol.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.Token.Symbol == input.Symbol);
        }
        
        if (!input.TransactionId.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(o => o.TransactionId == input.TransactionId);
        }

        // if (input.Methods.Count > 0)
        // {
        //     var shouldQuery = input.Methods.Select(method =>
        //         (Func<QueryContainerDescriptor<TransferInfo>, QueryContainer>)(q =>
        //             q.Term(i => i.Field(f => f.Method).Value(method)))).ToList();
        //     mustQuery.Add(q => q.Bool(b => b.Should(shouldQuery)));
        // }

        var result = queryable.Skip(input.SkipCount)
            .Take(input.MaxResultCount).ToList();
        return objectMapper.Map<List<TransferInfo>, List<TransferInfoDto>>(result);
    }
}