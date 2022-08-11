using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Exchange.Loopring.Models.WS.Topic;

namespace WitxQ.Exchange.Loopring.Models.WS
{
    /// <summary>
    /// 订阅/退订返回值
    /// </summary>
    public class WsSubscribeResultModel<T> : BaseModel where T : BaseTopicModel
    {
        /// <summary>
        /// 订阅（"sub"）或退订（unSub"）
        /// </summary>
        public string op { get; set; }

        /// <summary>
        /// 操作序列号（不是必须字段）
        /// <para>
        /// 订阅/退订时客户端可以指定一个sequence代表序列号，后台返回结果也会附带同样的序列号。
        /// </para>
        /// </summary>
        public int sequence { get; set; }

        /// <summary>
        /// 订阅主题和参数 json字符串
        /// </summary>
        public List<T> topics { get; set; }

        /// <summary>
        /// 订阅结果
        /// </summary>
        public ResultModel result { get; set; }
    }

    /// <summary>
    /// 订阅/退订返回值中的result结构
    /// </summary>
    public class ResultModel : BaseModel
    {
        /// <summary>
        /// 订阅是否成功
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 订阅失败时的错误信息（不是必须字段）
        /// </summary>
        public ResultInfo error { get; set; }
    }

}
