using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Exchange.Loopring.Models.WS.Topic;

namespace WitxQ.Exchange.Loopring.Models.WS
{
    /// <summary>
    /// 订阅/退订实体
    /// </summary>
    public class SubscribeModel<T> : BaseModel where T : BaseTopicModel
    {
        /// <summary>
        /// 订阅（"sub"）或退订（unSub"）
        /// </summary>
        public string op { get; set; }

        /// <summary>
        /// 操作序列号
        /// <para>
        /// 订阅/退订时客户端可以指定一个sequence代表序列号，后台返回结果也会附带同样的序列号。
        /// </para>
        /// </summary>
        public int sequence { get; set; }

        /// <summary>
        /// apiKey
        /// <para>
        /// 在一次订阅/退订中，如果topics中任何一个主题需要ApiKey，那么本次操作就必须包含ApiKey
        /// </para>
        /// </summary>
        public string apiKey { get; set; }

        /// <summary>
        /// 是否全部退订
        /// <para>
        /// 1、如果unsubscribeAll是true，订阅前会先退订之前订阅的所有主题。
        /// 2、如果unsubscribeAll是true，所有主题都会被退订；如果在某个主题内将unsubscribeAll设置为true，那么该主题的所有配置都会被退订。
        /// </para>
        /// </summary>
        public bool unsubscribeAll { get; set; }

        /// <summary>
        /// 订阅主题和参数 json字符串
        /// </summary>
        public List<T> topics { get; set; }

    }
}
