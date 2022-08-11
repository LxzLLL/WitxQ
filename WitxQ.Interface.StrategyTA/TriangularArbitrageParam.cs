using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Model.Markets;

namespace WitxQ.Interface.StrategyTA
{
    /// <summary>
    /// 套利全局参数选项
    /// </summary>
    public class TriangularArbitrageParam
    {
        /// <summary>
        /// 交易所名称
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// 日志路径
        /// </summary>
        public string LoggerPath { get; set; }

        /// <summary>
        /// 目标token，全部大写形式,例如：ETH
        /// </summary>
        public string TargetToken { get; set; }

        /// <summary>
        /// 套利深度
        /// </summary>
        public int Deep { get; set; }

        /// <summary>
        /// 套利的环路名称，例如“A-B-C-A”
        /// </summary>
        public string TARingName { get; set; }

        /// <summary>
        /// 套利的各个交易对参数选项
        /// </summary>
        public List<TriangularArbitragePairsParam> TAPairs { get; set; }

        /// <summary>
        /// 最小价差百分之多少才开始交易（未剔除Fee），默认0.01即1%
        /// </summary>
        public decimal MinProfitRatio { get; set; } = 0.01M;

        /// <summary>
        /// 指定交易最小值，建议是三个交易对之间最大的，否则会导致某单失败（例如 0.0505Eth）
        /// <para>
        /// 值都是以目标token为单位
        /// </para>
        /// </summary>
        public decimal MinAmountTran { get; set; }

        /// <summary>
        /// 是否按最小额度进行交易，默认为true
        /// <para>
        /// true:按指定的最小额度进行交易
        /// false:按套利所有交易对 挂单 的最小额度的   进行交易
        /// </para>
        /// </summary>
        public bool IsMinAmountTran { get; set; } = true;

        /// <summary>
        /// IsMinAmountTran为false时，交易数量的百分比，默认为0.5即50%
        /// <para>
        /// 避免因为depth未及时更新导致Order重复下单
        /// </para>
        /// </summary>
        public decimal AmountTranPercentage { get; set; } = 0.5M;

        /// <summary>
        /// 是否开启交易，默认为false
        /// <para>
        /// 如果为false时，只记录日志，不进行下单交易
        /// </para>
        /// </summary>
        public bool IsTranAuto { get; set; } = false;


        /// <summary>
        /// 滑点，交易价格的百分比，默认为0，例如可以设置为0.0002，表示为万2
        /// <para>
        /// 避免因为中继对小数精度的计算误差 或 由于行情过快导致的有滑点
        /// </para>
        /// </summary>
        public decimal SlidingPoint { get; set; } = 0M;

        /// <summary>
        /// 调用order方法的时间间隔（毫秒）
        /// <para>
        /// 由于交易所有接口调用限制
        /// </para>
        /// </summary>
        public int IntervalOrderCall { get; set; } = 0;
    }

    /// <summary>
    /// 套利的交易对参数选项
    /// </summary>
    public class TriangularArbitragePairsParam
    {
        /// <summary>
        /// 套利交易对的序号（即执行顺序，从1开始）
        /// </summary>
        public int SeqNumber { get; set; }
        /// <summary>
        /// 交易对，全部为中间“-”连字符的大写形式,例如：LRC-ETH
        /// </summary>
        public PairModel Pair { get; set; }

        /// <summary>
        /// 交易方向，例如：LRC-ETH交易对，true为sell（卖LRC），false为buy（买LRC）
        /// </summary>
        public bool OrderSide { get; set; }

    }

}
