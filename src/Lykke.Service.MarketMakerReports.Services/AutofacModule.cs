﻿using Autofac;
using JetBrains.Annotations;
using Lykke.Service.MarketMakerReports.Core.Services;
using Lykke.Service.MarketMakerReports.Services.RealisedPnL;
using Lykke.Service.MarketMakerReports.Services.Settings;
using Lykke.Service.MarketMakerReports.Services.Trades;
using Lykke.Service.MarketMakerReports.Services.TradingPnL;

namespace Lykke.Service.MarketMakerReports.Services
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuditMessageService>()
                .As<IAuditMessageService>()
                .SingleInstance();

            builder.RegisterType<InventorySnapshotService>()
                .As<IInventorySnapshotService>()
                .SingleInstance();
            
            builder.RegisterType<LykkeTradeService>()
                .As<ILykkeTradeService>()
                .SingleInstance();
            
            builder.RegisterType<ExternalTradeService>()
                .As<IExternalTradeService>()
                .SingleInstance();

            builder.RegisterType<PnLService>()
                .As<IPnLService>()
                .SingleInstance();

            builder.RegisterType<WalletSettingsService>()
                .As<IWalletSettingsService>()
                .SingleInstance();
            
            builder.RegisterType<HealthMonitorService>()
                .As<IHealthMonitorService>()
                .SingleInstance();
            
            builder.RegisterType<RealisedPnLService>()
                .As<IRealisedPnLService>()
                .SingleInstance();
            
            builder.RegisterType<QuoteService>()
                .As<IQuoteService>()
                .SingleInstance();
        }
    }
}
