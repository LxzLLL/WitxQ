using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Server.Test
{
    public class SwapStrategyConfig
    {
        public string Name { get; set; }

        /// <summary>
        /// Swap的AMM市场交易对，全部为中间“-”连字符的大写形式,例如：AMM-LRC-ETH
        /// </summary>
        public string SwapMarketPair { get; set; }


        /// <summary>
        /// 订单薄市场交易对，全部为中间“-”连字符的大写形式,例如：LRC-ETH
        /// </summary>
        public string MarketPair { get; set; }

        /// <summary>
        /// 目标TOKEN
        /// <para>
        /// 例如LRC-USDT，目标token为LRC时，先卖LRC后再买LRC，只为获取LRC
        /// </para>
        /// </summary>
        public string TargetToken { get; set; }

        /// <summary>
        /// 每次交易的目标通证数量（如果小于此值，则不交易）
        /// </summary>
        public decimal TradeAmount { get; set; }

        /// <summary>
        /// 滑点限制，百分比，默认0.0001
        /// </summary>
        public decimal SlipLimit { get; set; } = 0.0001M;

        /// <summary>
        /// swap闪兑的交易费用，默认为0.0025，loopring 终身荣誉vip为0.0021
        /// </summary>
        public decimal SwapFee { get; set; } = 0.0025M;

        /// <summary>
        /// 订单薄交易费用，默认为0.0006，loopring 终身荣誉vip为0.0006
        /// </summary>
        public decimal OrderBookFee { get; set; } = 0.0006M;

        /// <summary>
        /// 最小价差百分之多少才开始交易（未剔除Fee），默认0.01即1%
        /// </summary>
        public decimal MinProfitRatio { get; set; } = 0.01M;

        /// <summary>
        /// 是否开启交易，默认为false
        /// <para>
        /// 如果为false时，只记录日志，不进行下单交易
        /// </para>
        /// </summary>
        public bool IsTranAuto { get; set; } = false;

        /// <summary>
        /// 调用order方法的时间间隔（毫秒）
        /// <para>
        /// 由于交易所有接口调用限制
        /// </para>
        /// </summary>
        public int IntervalOrderCall { get; set; } = 0;

    }
}
