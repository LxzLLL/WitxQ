using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.WS.Topic
{
    /// <summary>
    /// 主题模型（适用于所有）
    /// </summary>
    public class TopicModel : BaseTopicModel
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

        /// <summary>
        /// 时间间隔，candlestick主题的参数
        /// <para>
        /// 1min	1分钟
        /// 5min	5分钟
        /// 15min	15分钟
        /// 30min	30分钟
        /// 1hr	1小时
        /// 2hr	2小时
        /// 4hr	4小时
        /// 12hr	12小时
        /// 1d	1天
        /// 1w	1周
        /// </para>
        /// </summary>
        public string interval { get; set; }

        /// <summary>
        /// ammpool的池地址
        /// </summary>
        public string poolAddress { get; set; }

    }
}
