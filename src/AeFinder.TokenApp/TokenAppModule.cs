using AeFinder.Sdk.Processor;
using AeFinder.TokenApp.GraphQL;
using AeFinder.TokenApp.Processors;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace AeFinder.TokenApp;

public class TokenAppModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<TokenAppModule>(); });
        context.Services.AddSingleton<ISchema, TokenAppSchema>();
        
        context.Services.AddSingleton<ILogEventProcessor, BurnedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, CrossChainReceivedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, CrossChainTransferredProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, IssuedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, RentalChargedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, ResourceTokenClaimedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, TokenCreatedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, TransactionFeeChargedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, TransactionFeeClaimedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, TransferredProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, ChainPrimaryTokenSymbolSetProcessor>();
    }
}