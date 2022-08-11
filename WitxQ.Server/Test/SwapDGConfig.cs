using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Server.Test
{
    public class SwapDGConfig
    {

        public string Name { get; set; }

        /// <summary>
        /// 市场交易对，全部为中间“-”连字符的大写形式,例如：AMM-LRC-ETH
        /// <para>
        /// 运行过程中不可更新
        /// </para>
        /// </summary>
        public string PairSymbol { get; set; }

        /// <summary>
        /// 基础货币（此交易对的 交易token），全部为大写形式
        /// <para>
        /// 例如：LRC-ETH（不同交易所的交易对字符串不同），则此交易对是以eth计价，买卖lrc
        /// </para>
        /// <para>
        /// 运行过程中不可更新
        /// </para>
        /// </summary>
        public string BaseToken { get; set; }

        /// <summary>
        /// 初始基础货币（交易token）的数量
        /// <para>
        /// 运行过程中不可更新
        /// </para>
        /// </summary>
        public decimal InitialBaseTokenAmount { get; set; }

        /// <summary>
        /// 定价货币（此交易对的 计价token），全部为大写形式
        /// <para>
        /// 例如：LRC-ETH（不同交易所的交易对字符串不同），则此交易对是以eth计价，买卖lrc
        /// </para>
        /// <para>
        /// 运行过程中不可更新
        /// </para>
        /// </summary>
        public string QuoteToken { get; set; }

        /// <summary>
        /// 初始定价货币（计价token）的数量
        /// <para>
        /// 运行过程中不可更新
        /// </para>
        /// </summary>
        public decimal InitialQuoteTokenAmount { get; set; }

        /// <summary>
        /// 初始基准价格
        /// <para>
        /// 运行过程中不可更新
        /// </para>
        /// </summary>
        public decimal BenchmarkPrice { get; set; }

        /// <summary>
        /// 买步长类型，固定值：0，百分比：1，默认固定值 0
        /// </summary>
        public int StepBuyType { get; set; } = 0;

        /// <summary>
        /// 买步长
        /// </summary>
        public decimal StepBuy { get; set; }

        /// <summary>
        /// 买数量
        /// </summary>
        public decimal StepBuyAmount { get; set; }

        /// <summary>
        /// 卖步长类型，固定值：0，百分比：1，默认固定值 0
        /// </summary>
        public int StepSellType { get; set; } = 0;

        /// <summary>
        /// 卖步长
        /// </summary>
        public decimal StepSell { get; set; }

        /// <summary>
        /// 卖数量
        /// </summary>
        public decimal StepSellAmount { get; set; }

        /// <summary>
        /// 账户金额是否需要再平衡（即恢复到初始状态）
        /// <para>
        /// 设置的某一币种，达到设置的盈利数量，则重新平衡账户
        /// </para>
        /// </summary>
        public bool IsRebalance { get; set; }

        /// <summary>
        /// 再平衡条件--获得收益的token
        /// <para>
        /// BaseToken:基础token，例如lrc
        /// QuoteToken：定价Token，例如eth
        /// </para>
        /// </summary>
        public string GainToken { get; set; }

        /// <summary>
        /// 再平衡条件--获得收益token的数量
        /// <para>
        /// 如果为小于等于0，则不平衡，建议最少不能少于最小交易数量
        /// </para>
        /// </summary>
        public decimal GainTokenAmount { get; set; }

        /// <summary>
        /// 滑点限制，百分比，默认0.0001
        /// </summary>
        public decimal SlipLimit { get; set; } = 0.0001M;

        /// <summary>
        /// swap闪兑的交易费用，默认为0.0025，loopring 终身荣誉vip为0.0021
        /// </summary>
        public decimal SwapFee { get; set; } = 0.0025M;

    }
}
