using System;
using System.Collections.Generic;
using System.Text;


/***************************************************************************************************************
 1、本系统默认market格式为 Token符号的大写形式，且中间有“-”连字符，例如：LRC-ETH，LRC-USDT，ETH-USDT，
   在涉及本系统的计算均使用此格式，例如在策略中使用的便是ETH、LRC、USDT等大写字符格式的Token，对接到其他交易所中，需要将此Token
   格式转换为交易所能使用的格式
 2、Token的格式为Token符号的大写形式，使用方式类似1
 3、如此设计方便对接各种市场，例如股市、汇市、贵金属等
***************************************************************************************************************/
namespace WitxQ.Model.Markets
{
    /// <summary>
    /// 市场上的交易对信息
    /// </summary>
    public class PairModel:BaseModel
    {
        /// <summary>
        /// 市场交易对，全部为中间“-”连字符的大写形式,例如：LRC-ETH
        /// </summary>
        public string PairSymbol { get; set; }

        /// <summary>
        /// 定价货币（此交易对的 计价token），全部为大写形式
        /// <para>
        /// 例如：LRC-ETH（不同交易所的交易对字符串不同），则此交易对是以eth计价，买卖lrc
        /// </para>
        /// </summary>
        public string QuoteToken { get; set; }

        /// <summary>
        /// 基础货币（此交易对的 交易token），全部为大写形式
        /// <para>
        /// 例如：LRC-ETH（不同交易所的交易对字符串不同），则此交易对是以eth计价，买卖lrc
        /// </para>
        /// </summary>
        public string BaseToken { get; set; }

        /// <summary>
        /// 定价货币（计价token）的最小下单量
        /// </summary>
        public decimal MinAmountQuoteToken { get; set; }

        /// <summary>
        /// 基础货币（交易token）的最小下单量
        /// </summary>
        public decimal MinAmountBaseToken { get; set; }

        /// <summary>
        /// 交易费
        /// </summary>
        //public decimal Fee { get; set; }

        /// <summary>
        /// 市场价格折叠级别，默认都是0，（每隔一级少个小数位）
        /// </summary>
        //public int PriceFoldLevel { get; set; }

        /// <summary>
        /// 服务器端返回深度中支持的最大价格精度，例如：6
        /// </summary>
        public int PrecisionForPrice { get; set; }

    }
}
