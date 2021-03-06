﻿using System;
using Autofac;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.Assets.Client;
using Lykke.Service.MarketMakerReports.Managers;
using Lykke.Service.MarketMakerReports.Rabbit.Subscribers;
using Lykke.Service.MarketMakerReports.Settings;
using Lykke.Service.MarketMakerReports.Settings.ServiceSettings.Rabbit;
using Lykke.Service.RateCalculator.Client;
using Lykke.SettingsReader;
using IStartable = Lykke.Service.MarketMakerReports.Managers.IStartable;

namespace Lykke.Service.MarketMakerReports
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public AutofacModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new Services.AutofacModule());
            
            builder.RegisterModule(new AzureRepositories.AutofacModule(
                _appSettings.Nested(o => o.MarketMakerReportsService.Db.DataConnectionString)));
            
            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            RegisterRabbit(builder);
            
            builder.RegisterRateCalculatorClient(_appSettings.CurrentValue.RateCalculatorServiceClient.ServiceUrl);
            
            builder.RegisterAssetsClient(AssetServiceSettings
                .Create(new Uri(_appSettings.CurrentValue.AssetsServiceClient.ServiceUrl),
                    _appSettings.CurrentValue.MarketMakerReportsService.AssetsCacheExpirationPeriod));
        }

        private void RegisterRabbit(ContainerBuilder builder)
        {
            RabbitSettings rabbitSettings = _appSettings.CurrentValue.MarketMakerReportsService.Rabbit;

            builder.RegisterType<AuditMessageSubscriber>()
                .As<IStartable>()
                .As<IStoppable>()
                .WithParameter(TypedParameter.From(rabbitSettings.AuditMessage))
                .SingleInstance();

            builder.RegisterType<InventorySnapshotSubscriber>()
                .As<IStartable>()
                .As<IStoppable>()
                .WithParameter(TypedParameter.From(rabbitSettings.InventorySnapshot))
                .SingleInstance();

            builder.RegisterType<LykkeTradeSubscriber>()
                .As<IStartable>()
                .As<IStoppable>()
                .WithParameter(TypedParameter.From(rabbitSettings.LykkeTrade))
                .SingleInstance();

            builder.RegisterType<ExternalTradeSubscriber>()
                .As<IStartable>()
                .As<IStoppable>()
                .WithParameter(TypedParameter.From(rabbitSettings.ExternalTrade))
                .SingleInstance();

            builder.RegisterType<HealthIssueSubscriber>()
                .As<IStartable>()
                .As<IStoppable>()
                .WithParameter(TypedParameter.From(rabbitSettings.HealthIssue))
                .SingleInstance();
        }
    }
}
