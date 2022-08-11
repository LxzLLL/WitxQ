using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Order
{
    /// <summary>
    /// 批量提交订单的响应模型
    /// </summary>
    public class OrderBatchResponseModel : BaseModel
    {
        /// <summary>
        /// 返回结果信息
        /// </summary>
        public ResultInfo resultInfo { get; set; }

        /// <summary>
        /// 批量订单响应实体的 结果列表信息
        /// </summary>
        public List<OrderBatchInfo> data { get; set; }
    }

    /// <summary>
    /// 批量订单响应实体的 结果列表信息
    /// </summary>
    public class OrderBatchInfo : BaseModel
    {
        /// <summary>
        /// 订单Hash
        /// </summary>
        public string hash { get; set; }

        /// <summary>
        /// 对应hash的结果信息
        /// </summary>
        public ResultInfo error { get; set; }
    }
}
