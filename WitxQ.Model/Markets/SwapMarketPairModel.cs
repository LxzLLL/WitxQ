using System;
using System.Collections.Generic;
using System.Text;

/***************************************************************************************************************
 1、本系统默认AmmMarket格式为 Token符号的大写形式，且中间有“-”连字符，例如：AMM-LRC-ETH，AMM-LRC-USDT，AMM-ETH-USDT，
   在涉及本系统的计算均使用此格式，例如在策略中使用的便是ETH、LRC、USDT等大写字符格式的Token，对接到其他交易所中，需要将此Token
   格式转换为交易所能使用的格式
 2、Token的格式为Token符号的大写形式，使用方式类似1
 3、如此设计方便对接各种市场，例如股市、汇市、贵金属等
***************************************************************************************************************/
namespace WitxQ.Model.Markets
{
    /// <summary>
    /// Swap闪兑市场上的交易对信息
    /// </summary>
    public class SwapMarketPairModel : BaseModel
    {

        /// <summary>
        /// 市场名称，例如:"AMM-LRC-USDT"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 市场，例如:"AMM-LRC-USDT"
        /// </summary>
        public string Market { get; set; }

        /// <summary>
        /// 合约地址，例如"0x97241525fe425C90eBe5A41127816dcFA5954b06"
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 基础货币（此交易对的 交易token），全部为大写形式
        /// <para>
        /// 例如：LRC-ETH（不同交易所的交易对字符串不同），则此交易对是以eth计价，买卖lrc
        /// </para>
        /// </summary>
        public string BaseToken { get; set; }

        /// <summary>
        /// 定价货币（此交易对的 计价token），全部为大写形式
        /// <para>
        /// 例如：LRC-ETH（不同交易所的交易对字符串不同），则此交易对是以eth计价，买卖lrc
        /// </para>
        /// </summary>
        public string QuoteToken { get; set; }

        /// <summary>
        /// 费率 百分率
        /// </summary>
        public int FeeBips { get; set; }

        /// <summary>
        /// 价格精度
        /// </summary>
        public int PricePrecision { get; set; }

        /// <summary>
        /// 数量精度
        /// </summary>
        public int AmountPrecision { get; set; }

    }
}
