using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.WS.Topic
{
    /// <summary>
    /// AmmPool的主题模型
    /// </summary>
    public class AmmPoolTopicModel : BaseTopicModel
    {
        /// <summary>
        /// pool的地址
        /// </summary>
        public string poolAddress { get; set; }
        /// <summary>
        /// 快照
        /// </summary>
        public bool snapshot { get; set; }
    }
}
