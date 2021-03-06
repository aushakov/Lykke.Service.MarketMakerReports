using System;
using Lykke.Service.MarketMakerReports.Services.RealisedPnL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.MarketMakerReports.Services.Tests.RealisedPnL
{
    [TestClass]
    public class RealisedPnLCalculatorTests
    {
        [TestMethod]
        public void Open_Short_Position()
        {
            // arrange

            decimal tradePrice = 6543.40m;
            decimal tradeVolume = 10;
            bool inverted = false;
            int direction = 1;
            decimal prevCumulativeVolume = 0;
            decimal prevCumulativeOppositeVolume = 0;
            decimal rate = 6543.40m;
            decimal openRate = 0;
            decimal crossRate = 1;

            var expectedResult = new RealisedPnLResult
            {
                AvgPrice = tradePrice,
                Price = tradePrice,
                Volume = tradeVolume,
                OppositeVolume = tradeVolume * tradePrice,
                CumulativeVolume = tradeVolume * direction,
                CumulativeOppositeVolume = tradeVolume * tradePrice * -1 * direction,
                ClosedVolume = 0,
                RealisedPnL = 0,
                UnrealisedPnL = 0
            };

            // act

            RealisedPnLResult actualResult = RealisedPnLCalculator.Calculate(
                tradePrice,
                tradeVolume,
                inverted,
                direction,
                prevCumulativeVolume,
                prevCumulativeOppositeVolume,
                rate,
                openRate,
                crossRate);

            // assert

            Assert.IsTrue(AreEqual(expectedResult, actualResult));
        }

        [TestMethod]
        public void Open_Long_Position()
        {
            // arrange

            decimal tradePrice = 6543.40m;
            decimal tradeVolume = 10;
            bool inverted = false;
            int direction = -1;
            decimal prevCumulativeVolume = 0;
            decimal prevCumulativeOppositeVolume = 0;
            decimal rate = 6543.40m;
            decimal openRate = 0;
            decimal crossRate = 1;

            var expectedResult = new RealisedPnLResult
            {
                AvgPrice = tradePrice,
                Price = tradePrice,
                Volume = tradeVolume,
                OppositeVolume = tradeVolume * tradePrice,
                CumulativeVolume = tradeVolume * direction,
                CumulativeOppositeVolume = tradeVolume * tradePrice * -1 * direction,
                ClosedVolume = 0,
                RealisedPnL = 0,
                UnrealisedPnL = 0
            };

            // act

            RealisedPnLResult actualResult = RealisedPnLCalculator.Calculate(
                tradePrice,
                tradeVolume,
                inverted,
                direction,
                prevCumulativeVolume,
                prevCumulativeOppositeVolume,
                rate,
                openRate,
                crossRate);

            // assert

            Assert.IsTrue(AreEqual(expectedResult, actualResult));
        }

        [TestMethod]
        public void Increase_Short_Opened_Position()
        {
            // arrange

            decimal tradePrice = 5607.87m;
            decimal tradeVolume = 1;
            bool inverted = false;
            int direction = 1;
            decimal prevCumulativeVolume = 10;
            decimal prevCumulativeOppositeVolume = -65434.00m;
            decimal rate = 6545.88m;
            decimal openRate = 6543.40m;
            decimal crossRate = 1.16m;

            var expectedResult = new RealisedPnLResult
            {
                Price = tradePrice * crossRate,
                Volume = tradeVolume,
                OppositeVolume = tradeVolume * tradePrice * crossRate,
                CumulativeVolume = prevCumulativeVolume + tradeVolume * direction,
                CumulativeOppositeVolume =
                    prevCumulativeOppositeVolume + tradeVolume * tradePrice * crossRate * -1 * direction,
                ClosedVolume = 0,
                RealisedPnL = 0
            };

            expectedResult.AvgPrice =
                Math.Abs(expectedResult.CumulativeOppositeVolume / expectedResult.CumulativeVolume);

            expectedResult.UnrealisedPnL = (rate - expectedResult.AvgPrice) * expectedResult.CumulativeVolume;

            // act

            RealisedPnLResult actualResult = RealisedPnLCalculator.Calculate(
                tradePrice,
                tradeVolume,
                inverted,
                direction,
                prevCumulativeVolume,
                prevCumulativeOppositeVolume,
                rate,
                openRate,
                crossRate);

            // assert

            Assert.IsTrue(AreEqual(expectedResult, actualResult));
        }

        [TestMethod]
        public void Increase_Long_Opened_Position()
        {
            // arrange

            decimal tradePrice = 5607.87m;
            decimal tradeVolume = 1;
            bool inverted = false;
            int direction = -1;
            decimal prevCumulativeVolume = -10;
            decimal prevCumulativeOppositeVolume = 65434.00m;
            decimal rate = 6545.88m;
            decimal openRate = 6543.40m;
            decimal crossRate = 1.16m;

            var expectedResult = new RealisedPnLResult
            {
                Price = tradePrice * crossRate,
                Volume = tradeVolume,
                OppositeVolume = tradeVolume * tradePrice * crossRate,
                CumulativeVolume = prevCumulativeVolume + tradeVolume * direction,
                CumulativeOppositeVolume =
                    prevCumulativeOppositeVolume + tradeVolume * tradePrice * crossRate * -1 * direction,
                ClosedVolume = 0,
                RealisedPnL = 0
            };

            expectedResult.AvgPrice =
                Math.Abs(expectedResult.CumulativeOppositeVolume / expectedResult.CumulativeVolume);

            expectedResult.UnrealisedPnL = (rate - expectedResult.AvgPrice) * expectedResult.CumulativeVolume;

            // act

            RealisedPnLResult actualResult = RealisedPnLCalculator.Calculate(
                tradePrice,
                tradeVolume,
                inverted,
                direction,
                prevCumulativeVolume,
                prevCumulativeOppositeVolume,
                rate,
                openRate,
                crossRate);

            // assert

            Assert.IsTrue(AreEqual(expectedResult, actualResult));
        }

        [TestMethod]
        public void Increase_Short_Opened_Position_Inverted_AssetPair()
        {
            // arrange

            decimal tradePrice = 1 / 5607.87m;
            decimal tradeVolume = 10 * 5607.87m;
            bool inverted = true;
            int direction = -1;
            decimal prevCumulativeVolume = 10;
            decimal prevCumulativeOppositeVolume = -65434.00m;
            decimal rate = 6545.88m;
            decimal openRate = 6543.40m;
            decimal crossRate = 1.16m;


            var expectedResult = new RealisedPnLResult
            {
                Price = 1 / tradePrice * crossRate,
                Volume = tradePrice * tradeVolume,
                OppositeVolume = tradeVolume * crossRate,
                ClosedVolume = 0,
                RealisedPnL = 0
            };

            expectedResult.CumulativeVolume = prevCumulativeVolume + expectedResult.Volume * direction * -1;

            expectedResult.CumulativeOppositeVolume = prevCumulativeOppositeVolume +
                                                      expectedResult.Volume * expectedResult.Price * -1 * direction * -1;

            expectedResult.AvgPrice =
                Math.Abs(expectedResult.CumulativeOppositeVolume / expectedResult.CumulativeVolume);

            expectedResult.UnrealisedPnL = (rate - expectedResult.AvgPrice) * expectedResult.CumulativeVolume;

            // act

            RealisedPnLResult actualResult = RealisedPnLCalculator.Calculate(
                tradePrice,
                tradeVolume,
                inverted,
                direction,
                prevCumulativeVolume,
                prevCumulativeOppositeVolume,
                rate,
                openRate,
                crossRate);

            // assert

            Assert.IsTrue(AreEqual(expectedResult, actualResult));
        }

        [TestMethod]
        public void Partially_Close_Opened_Position()
        {
            // arrange

            decimal tradePrice = 6544.95m;
            decimal tradeVolume = 6;
            bool inverted = false;
            int direction = -1;
            decimal prevCumulativeVolume = 11;
            decimal prevCumulativeOppositeVolume = -71939.13m;
            decimal rate = 6545.70m;
            decimal openRate = 6539.92m;
            decimal crossRate = 1m;

            var expectedResult = new RealisedPnLResult
            {
                AvgPrice = openRate,
                Price = tradePrice * crossRate,
                Volume = tradeVolume,
                OppositeVolume = tradeVolume * tradePrice * crossRate,
                CumulativeVolume = prevCumulativeVolume - tradeVolume,
                CumulativeOppositeVolume = prevCumulativeOppositeVolume + tradeVolume * openRate * -1 * direction,
                ClosedVolume = tradeVolume
            };

            expectedResult.RealisedPnL = (expectedResult.Price - openRate) * expectedResult.ClosedVolume;

            expectedResult.UnrealisedPnL = (rate - expectedResult.AvgPrice) * expectedResult.CumulativeVolume;

            // act

            RealisedPnLResult actualResult = RealisedPnLCalculator.Calculate(
                tradePrice,
                tradeVolume,
                inverted,
                direction,
                prevCumulativeVolume,
                prevCumulativeOppositeVolume,
                rate,
                openRate,
                crossRate);

            // assert

            Assert.IsTrue(AreEqual(expectedResult, actualResult));
        }

        [TestMethod]
        public void Close_Short_Position_And_Open_Long_Position()
        {
            // arrange

            decimal tradePrice = 6543.65m;
            decimal tradeVolume = 8;
            bool inverted = false;
            int direction = -1;
            decimal prevCumulativeVolume = 5;
            decimal prevCumulativeOppositeVolume = -32699.61m;
            decimal rate = 6545.05m;
            decimal openRate = 6539.92m;
            decimal crossRate = 1m;

            var expectedResult = new RealisedPnLResult
            {
                AvgPrice = tradePrice * crossRate,
                Price = tradePrice * crossRate,
                Volume = tradeVolume,
                OppositeVolume = tradeVolume * tradePrice * crossRate,
                ClosedVolume = prevCumulativeVolume
            };

            expectedResult.CumulativeVolume = prevCumulativeVolume - expectedResult.Volume;

            expectedResult.CumulativeOppositeVolume =
                (expectedResult.Volume - prevCumulativeVolume) * expectedResult.Price * -1 * direction;

            expectedResult.RealisedPnL = (expectedResult.Price - openRate) * expectedResult.ClosedVolume;

            expectedResult.UnrealisedPnL = (rate - expectedResult.AvgPrice) * expectedResult.CumulativeVolume;

            // act

            RealisedPnLResult actualResult = RealisedPnLCalculator.Calculate(
                tradePrice,
                tradeVolume,
                inverted,
                direction,
                prevCumulativeVolume,
                prevCumulativeOppositeVolume,
                rate,
                openRate,
                crossRate);

            // assert

            Assert.IsTrue(AreEqual(expectedResult, actualResult));
        }

        [TestMethod]
        public void Close_Long_Position_And_Open_Short_Position()
        {
            // arrange

            decimal tradePrice = 6543.65m;
            decimal tradeVolume = 8;
            bool inverted = false;
            int direction = 1;
            decimal prevCumulativeVolume = -5;
            decimal prevCumulativeOppositeVolume = 32699.61m;
            decimal rate = 6545.05m;
            decimal openRate = 6539.92m;
            decimal crossRate = 1m;

            var expectedResult = new RealisedPnLResult
            {
                AvgPrice = tradePrice * crossRate,
                Price = tradePrice * crossRate,
                Volume = tradeVolume,
                OppositeVolume = tradeVolume * tradePrice * crossRate,
                ClosedVolume = Math.Abs(prevCumulativeVolume)
            };

            expectedResult.CumulativeVolume = tradeVolume - expectedResult.ClosedVolume;

            expectedResult.CumulativeOppositeVolume =
                expectedResult.CumulativeVolume * expectedResult.Price * -1 * direction;

            expectedResult.RealisedPnL = (expectedResult.Price - openRate) * expectedResult.ClosedVolume;

            expectedResult.UnrealisedPnL = (rate - expectedResult.AvgPrice) * expectedResult.CumulativeVolume;

            // act

            RealisedPnLResult actualResult = RealisedPnLCalculator.Calculate(
                tradePrice,
                tradeVolume,
                inverted,
                direction,
                prevCumulativeVolume,
                prevCumulativeOppositeVolume,
                rate,
                openRate,
                crossRate);

            // assert

            Assert.IsTrue(AreEqual(expectedResult, actualResult));
        }

        private static bool AreEqual(RealisedPnLResult a, RealisedPnLResult b)
        {
            return a.AvgPrice == b.AvgPrice &&
                   a.Price == b.Price &&
                   a.Volume == b.Volume &&
                   a.OppositeVolume == b.OppositeVolume &&
                   a.CumulativeVolume == b.CumulativeVolume &&
                   a.CumulativeOppositeVolume == b.CumulativeOppositeVolume &&
                   a.ClosedVolume == b.ClosedVolume &&
                   a.RealisedPnL == b.RealisedPnL &&
                   a.UnrealisedPnL == b.UnrealisedPnL;
        }
    }
}
