using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Depth
{
    /// <summary>
    /// 市场深度信息
    /// </summary>
    public class DepthModel:BaseModel
    {
        /// <summary>
        /// 连续版本号。前端据此判断是否丢失数据。（参考websocket的使用）
        /// </summary>
        public int version { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long timestamp { get; set; }

        /// <summary>
        /// 买单深度
        /// <para>
        /// 每一条深度包含四个元素，分别是价格，数量（基础通证的数量），成交额（ 计价通证的数量），和聚合的订单数目，例如："[["0.0391","88412777200000007405568","3465780866","2"],["0.0387","115378777300000005586944","4476696558","3"]]"
        /// </para>
        /// </summary>
        public List<List<string>> bids { get; set; } = new List<List<string>>();

        /// <summary>
        /// 卖单深度
        /// <para>
        /// 每一条深度包含四个元素，分别是价格，数量（基础通证的数量），成交额（ 计价通证的数量），和聚合的订单数目，例如："[["0.0419","573300000000000000000","24021270","1"],["0.0420","9799537899999999295488","411580591","1"]]"
        /// </para>
        /// </summary>
        public List<List<string>> asks { get; set; } = new List<List<string>>();
    }
}
