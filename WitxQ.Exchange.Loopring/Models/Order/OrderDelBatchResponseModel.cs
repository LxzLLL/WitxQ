using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Order
{
    /// <summary>
    /// 批量撤销订单的响应模型
    /// </summary>
    public class OrderDelBatchResponseModel : BaseModel
    {
        /// <summary>
        /// 返回结果信息
        /// </summary>
        public ResultInfo resultInfo { get; set; }

        /// <summary>
        /// 批量撤销订单响应实体的 结果列表信息
        /// </summary>
        public List<OrderDelBatchInfo> data { get; set; }
    }

    /// <summary>
    /// 批量撤销订单响应实体的 结果列表信息
    /// </summary>
    public class OrderDelBatchInfo : BaseModel
    {
        /// <summary>
        /// 要取消订单的HASH或者ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 订单取消的结果
        /// </summary>
        public bool result { get; set; }

        /// <summary>
        /// 订单取消失败的错误
        /// </summary>
        public ResultInfo error { get; set; }
    }
}
