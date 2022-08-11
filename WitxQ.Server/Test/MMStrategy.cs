using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Server.Test
{
    public class MMStrategy
    {
        public string Name { get; set; }

        public MMStrategyConfig Config { get; set; }
    }

    /// <summary>
    /// 市商策略的配置
    /// </summary>
    public class MMStrategyConfig
    {
        /// <summary>
        /// 市场交易对
        /// </summary>
        public string MarketPair { get; set; }

        /// <summary>
        /// 交易方向，0买，1卖，2双向
        /// </summary>
        public int MakerDirection { get; set; }

        /// <summary>
        /// 挂买单的配置
        /// </summary>
        public List<MakerInfo> MarkerBids { get; set; }

        /// <summary>
        /// 挂卖单的配置
        /// </summary>
        public List<MakerInfo> MarkerAsks { get; set; }
    }


    public class MakerInfo
    {
        /// <summary>
        /// 挂单档位
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 挂单数量
        /// </summary>
        public decimal Amount { get; set; }
    }
}
