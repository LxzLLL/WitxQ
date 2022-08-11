using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.WS.Topic
{
    /// <summary>
    /// 订单簿更新 主题
    /// </summary>
    public class OrderBookTopicModel : BaseTopicModel
    {
        /// <summary>
        /// 交易对
        /// </summary>
        public string market { get; set; }

        /// <summary>
        /// 深度聚合级别
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 买卖深度条目数量，值不可以超过50。仅在snapshot = true时生效（不是必须字段）
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 默认为false。 如果该值为true，并且当深度条目有任何一条变化，那么指定数量的深度条目会被全量推送给客户端。（不是必须字段）
        /// </summary>
        public bool snapshot { get; set; }
    }
}
