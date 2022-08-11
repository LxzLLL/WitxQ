using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Exchange.Loopring.Models.Market;
using WitxQ.Exchange.Loopring.Models.Token;
using WitxQ.Exchange.Loopring.Tools;
using WitxQ.Interface.Spot;
using WitxQ.Model.Markets;

namespace WitxQ.Exchange.Loopring.Operation
{
    /// <summary>
    /// 市场信息的操作
    /// </summary>
    public class MarketInfoOperation : IMarket
    {
        #region IMarket
        /// <summary>
        /// 通过交易对字符串，获取交易对信息
        /// </summary>
        /// <param name="pair">交易对，全部为中间“-”连字符的大写形式</param>
        /// <returns></returns>
        public PairModel GetPairModel(string pair)
        {
            PairModel pairModel = null;
            MarketInfoModel marketModel = ExLoopring.MARKETS[pair];
            if(marketModel!=null)
            {
                pairModel = new PairModel();
                pairModel.PairSymbol = pair;

                TokenModel quoteToken = ExLoopring.TOKENS[marketModel.quoteTokenId];
                TokenModel baseToken = ExLoopring.TOKENS[marketModel.baseTokenId];
                pairModel.QuoteToken = quoteToken.symbol;
                pairModel.BaseToken = baseToken.symbol;
                pairModel.MinAmountQuoteToken = LoopringConvert.GetLoopringNumber(quoteToken.minOrderAmount, quoteToken.tokenId);
                pairModel.MinAmountBaseToken = LoopringConvert.GetLoopringNumber(baseToken.minOrderAmount, baseToken.tokenId);

                pairModel.PrecisionForPrice = marketModel.precisionForPrice;
            }
            return pairModel;
        }
        #endregion
    }
}
