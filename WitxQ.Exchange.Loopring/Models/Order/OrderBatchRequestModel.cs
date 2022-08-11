using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Order
{
    /// <summary>
    /// 批量提交订单的请求模型
    /// </summary>
    public class OrderBatchRequestModel : BaseModel
    {
        /// <summary>
        /// 订单列表，最多10个
        /// </summary>
        public List<OrderRequestModel> orders { get; set; }
    }
}
