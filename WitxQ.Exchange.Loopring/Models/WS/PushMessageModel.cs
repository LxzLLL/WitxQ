using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Exchange.Loopring.Models.WS.Topic;

namespace WitxQ.Exchange.Loopring.Models.WS
{
    /// <summary>
    /// WS推送的消息数据结构
    /// <para>
    /// T
    /// </para>
    /// </summary>
    public class PushMessageModel<T,D>:BaseModel 
        where T:BaseTopicModel 
        where D:class
    {
        /// <summary>
        /// 订阅主题和参数 json字符串
        /// </summary>
        public T topic { get; set; }

        /// <summary>
        /// 推送时间（毫秒）
        /// </summary>
        public long ts { get; set; }

        /// <summary>
        /// 该次推送的起始版本号
        /// </summary>
        public int startVersion { get; set; }

        /// <summary>
        /// 该次推送的终结版本号
        /// </summary>
        public int endVersion { get; set; }

        /// <summary>
        /// 推送的数据信息
        /// </summary>
        public D data { get; set; }
    }
}
