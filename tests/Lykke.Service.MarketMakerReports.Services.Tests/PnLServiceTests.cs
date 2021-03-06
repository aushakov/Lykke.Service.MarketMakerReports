using System.Collections.Generic;
using Lykke.Service.MarketMakerReports.Core.Domain.InventorySnapshots;
using Lykke.Service.MarketMakerReports.Services.TradingPnL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.MarketMakerReports.Services.Tests
{
    [TestClass]
    public class PnLServiceTests
    {
        private string _btc = "BTC";
        private string _eth = "ETH";
        private string _lykkeExchange = "lykke";

        [TestMethod]
        public void BalancesNotChanged_ZeroPnL()
        {
            var startSnapshot = CreateSnapshot(_btc, 100, 100);
            var endSnapshot = CreateSnapshot(_btc, 100, 100);

            var result = PnLCalculator.GetPnL(startSnapshot, endSnapshot);

            Assert.AreEqual(0, result.OnAllExchanges.Total);
        }

        [TestMethod]
        public void BalanceInUsdGrowth_PositivePnL()
        {
            var changeInBalanceUsd = 10;
            var startSnapshot = CreateSnapshot(_btc, 100, 100);
            var endSnapshot = CreateSnapshot(_btc, 100, 100 + changeInBalanceUsd);

            var result = PnLCalculator.GetPnL(startSnapshot, endSnapshot);

            Assert.AreEqual(changeInBalanceUsd, result.OnAllExchanges.Total);
        }

        [TestMethod]
        public void BalanceInUsdDrop_NegativePnL()
        {
            var changeInBalanceUsd = 10;
            var startSnapshot = CreateSnapshot(_btc, 100, 100);
            var endSnapshot = CreateSnapshot(_btc, 100, 100 - changeInBalanceUsd);

            var result = PnLCalculator.GetPnL(startSnapshot, endSnapshot);

            Assert.AreEqual(-changeInBalanceUsd, result.OnAllExchanges.Total);
        }

        [TestMethod]
        public void BalanceGrowth_PositivePnL()
        {
            var changeInBalanceUsd = 10;
            var startSnapshot = CreateSnapshot(_btc, 100, 100);
            var endSnapshot = CreateSnapshot(_btc, 100 + changeInBalanceUsd, 100 + changeInBalanceUsd);

            var result = PnLCalculator.GetPnL(startSnapshot, endSnapshot);

            Assert.AreEqual(0, result.OnAllExchanges.Total);
        }

        [TestMethod]
        public void NoInfoForAssetAtStartPoint_PositivePnL()
        {
            decimal balanceUsd = 100;

            var startSnapshot = CreateSnapshot(_btc, 0, 0); // no ETH in start snapshot
            var endSnapshot = CreateSnapshot(_eth, 1, balanceUsd);

            var result = PnLCalculator.GetPnL(startSnapshot, endSnapshot);

            Assert.AreEqual(0, result.OnAllExchanges.Total);
        }

        [TestMethod]
        public void NoInfoForAssetAtEndPoint_NegativePnL()
        {
            decimal balanceUsd = 100;

            var startSnapshot = CreateSnapshot(_btc, 1, balanceUsd);
            var endSnapshot = CreateSnapshot(_eth, 0, 0); // no BTC in end snapshot

            var result = PnLCalculator.GetPnL(startSnapshot, endSnapshot);

            Assert.AreEqual(-balanceUsd, result.OnAllExchanges.Total);
        }

        private InventorySnapshot CreateSnapshot(string assetId, decimal balance, decimal balanceUsd)
        {
            return new InventorySnapshot
            {
                Assets = new List<AssetBalanceInventory>
                {
                    new AssetBalanceInventory
                    {
                        AssetId = assetId,
                        Balances = new List<AssetBalance>
                        {
                            new AssetBalance
                            {
                                Exchange = _lykkeExchange,
                                Amount = balance,
                                AmountInUsd = balanceUsd
                            }
                        },
                        Inventories = new List<AssetInventory>()
                    }
                }
            };
        }
    }
}
