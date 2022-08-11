using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.WS.Topic
{
    /// <summary>
    /// 订阅主题基类
    /// </summary>
    public class BaseTopicModel:BaseModel
    {
        /// <summary>
        /// 主题名称
        /// </summary>
        public string topic { get; set; }
    }
}
