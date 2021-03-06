using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.MarketMakerReports.Client.Models.Trades
{
    public class ExternalTradeModel
    {
        public string OrderId { get; set; }
        
        public string Exchange { get; set; }

        public string AssetPairId { get; set; }

        [JsonConverter(typeof (StringEnumConverter))]
        public TradeType Type { get; set; }

        public DateTime Time { get; set; }

        public decimal Price { get; set; }

        public decimal Volume { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal OriginalVolume { get; set; }

        public decimal Commission { get; set; }

        public decimal ExchangeExecuteVolume { get; set; }
        
        public string BaseAssetId { get; set; }
        
        public string QuoteAssetId { get; set; }
    }
}
