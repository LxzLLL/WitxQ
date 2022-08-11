using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Model.Markets;

namespace WitxQ.Interface.Spot
{
    /// <summary>
    /// 闪兑的市场接口
    /// </summary>
    public interface ISwapMarket
    {
        /// <summary>
        /// 通过交易对字符串，获取交易对信息
        /// </summary>
        /// <param name="swapMarketPairName">交易对，全部为中间“-”连字符的大写形式，例如AMM-LRC-USDT</param>
        /// <returns></returns>
        public SwapMarketPairModel GetSwapMarketPairModel(string swapMarketPairName);
    }
}
