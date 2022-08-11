using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.WS.Topic
{
    /// <summary>
    /// 用户订单更新 主题
    /// </summary>
    public class OrderTopicModel:BaseTopicModel
    {
        /// <summary>
        /// 交易对
        /// </summary>
        public string market { get; set; }
    }
}
