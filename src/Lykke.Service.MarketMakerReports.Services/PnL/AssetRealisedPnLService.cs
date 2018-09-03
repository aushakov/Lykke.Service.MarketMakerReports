using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.MarketMakerReports.Core.Domain;
using Lykke.Service.MarketMakerReports.Core.Domain.PnL;
using Lykke.Service.MarketMakerReports.Core.Domain.Settings;
using Lykke.Service.MarketMakerReports.Core.Domain.Trades;
using Lykke.Service.MarketMakerReports.Core.Math;
using Lykke.Service.MarketMakerReports.Core.Repositories;
using Lykke.Service.MarketMakerReports.Core.Services;
using Lykke.Service.RateCalculator.Client;
using Lykke.Service.RateCalculator.Client.AutorestClient.Models;
using AssetPair = Lykke.Service.Assets.Client.Models.AssetPair;

namespace Lykke.Service.MarketMakerReports.Services.PnL
{
    [UsedImplicitly]
    public class AssetRealisedPnLService : IAssetRealisedPnLService
    {
        private readonly IAssetRealisedPnLRepository _assetRealisedPnLRepository;
        private readonly IAssetRealisedPnLCalculator _assetRealisedPnLCalculator;
        private readonly IAssetRealisedPnLSettingsService _assetRealisedPnLSettingsService;
        private readonly IRateCalculatorClient _rateCalculatorClient;
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly ILog _log;
        
        private readonly AssetRealisedPnLCache _cache = new AssetRealisedPnLCache();

        public AssetRealisedPnLService(
            IAssetRealisedPnLRepository assetRealisedPnLRepository,
            IAssetRealisedPnLCalculator assetRealisedPnLCalculator,
            IAssetRealisedPnLSettingsService assetRealisedPnLSettingsService,
            IRateCalculatorClient rateCalculatorClient,
            IAssetsServiceWithCache assetsServiceWithCache,
            ILogFactory logFactory)
        {
            _assetRealisedPnLRepository = assetRealisedPnLRepository;
            _assetRealisedPnLCalculator = assetRealisedPnLCalculator;
            _assetRealisedPnLSettingsService = assetRealisedPnLSettingsService;
            _rateCalculatorClient = rateCalculatorClient;
            _assetsServiceWithCache = assetsServiceWithCache;
            _log = logFactory.CreateLog(this);
        }

        public Task<IReadOnlyList<AssetRealisedPnL>> GetAsync(string assetId, int? limit)
        {
            return _assetRealisedPnLRepository.GetAsync(assetId, limit);
        }

        public async Task CalculateAsync(Trade trade)
        {
            try
            {
                AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(trade.AssetPairId);

                AssetRealisedPnLSettings assetRealisedPnLSettings = await _assetRealisedPnLSettingsService.GetAsync();

                if (!assetRealisedPnLSettings.Assets.Contains(assetPair.BaseAssetId))
                    return;

                AssetRealisedPnL lastAssetRealisedPnL = _cache.Get(assetPair.BaseAssetId);

                if (lastAssetRealisedPnL == null)
                {
                    lastAssetRealisedPnL = await _assetRealisedPnLRepository.GetLastAsync(assetPair.BaseAssetId);

                    if (lastAssetRealisedPnL != null)
                        _cache.Set(lastAssetRealisedPnL);
                    else
                        lastAssetRealisedPnL = new AssetRealisedPnL();
                }

                MarketProfile marketProfile = await _rateCalculatorClient.GetMarketProfileAsync();

                Quote quote =
                    await GetQuoteAsync(marketProfile, assetPair.BaseAssetId, assetRealisedPnLSettings.AssetId);

                Quote crossQuote =
                    await GetQuoteAsync(marketProfile, assetPair.QuotingAssetId, assetRealisedPnLSettings.AssetId);

                AssetRealisedPnL assetRealisedPnL = _assetRealisedPnLCalculator
                    .Calculate(lastAssetRealisedPnL, trade, quote, crossQuote, assetRealisedPnLSettings.AssetId);

                await _assetRealisedPnLRepository.InsertAsync(assetRealisedPnL);

                _cache.Set(assetRealisedPnL);
            }
            catch (Exception exception)
            {
                _log.Warning("An error occured while processing trade", exception, trade);
            }
        }

        private async Task<Quote> GetQuoteAsync(MarketProfile marketProfile, string baseAssetId, string quoteAssetId)
        {
            string directAssetPairId = $"{baseAssetId}{quoteAssetId}";

            string assetPairId = directAssetPairId;

            bool inverted = false;

            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(assetPairId);

            if (assetPair == null)
            {
                assetPairId = $"{quoteAssetId}{baseAssetId}";
                inverted = true;

                assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(assetPairId);
            }

            if (assetPair == null)
            {
                throw new InvalidOperationException(
                    $"Asset pair does not exist for '{baseAssetId}'/'{quoteAssetId}'");
            }

            FeedData feedData = marketProfile.Profile.FirstOrDefault(o => o.Asset == assetPairId);

            if (feedData == null)
                throw new InvalidOperationException($"No quote for asset pair '{assetPairId}'");

            decimal rate = 1;

            if (inverted)
                rate = 1 / (((decimal) feedData.Ask + (decimal) feedData.Bid) / 2m);

            return new Quote(directAssetPairId, feedData.DateTime, (decimal) feedData.Ask * rate,
                (decimal) feedData.Bid * rate);
        }
    }
}