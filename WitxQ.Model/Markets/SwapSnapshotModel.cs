using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Model.Markets
{
    /// <summary>
    /// 闪兑交易池的快照信息
    /// </summary>
    public class SwapSnapshotModel : BaseModel
    {

        /// <summary>
        /// 此快照的市场信息
        /// </summary>
        public SwapMarketPairModel MarketPair { get; set; } = new SwapMarketPairModel();

        /// <summary>
        /// 池的权益代币（LP）数量
        /// </summary>
        public string PoolTokenAmount { get; set; }

        /// <summary>
        /// 池中的基础币数量
        /// </summary>
        public string PoolBaseTokenAmount { get; set; }

        /// <summary>
        /// 池中的定价币数量
        /// </summary>
        public string PoolQuoteTokenAmount { get; set; }

        /// <summary>
        /// 基础币对应定价币的价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 逆向价格，定价币对应基础币的价格
        /// </summary>
        public decimal ReversePrice { get; set; }

        
    }
}
