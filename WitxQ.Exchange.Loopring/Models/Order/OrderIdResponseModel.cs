using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Order
{
    /// <summary>
    /// 订单ID响应模型
    /// </summary>
    public class OrderIdResponseModel : BaseModel
    {
        /// <summary>
        /// 返回结果信息
        /// </summary>
        public ResultInfo resultInfo { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public OrderIdResposeData data { get; set; }
    }


    /// <summary>
    /// 订单ID响应模型的Data部分
    /// </summary>
    public class OrderIdResposeData : BaseModel
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public int orderId { get; set; }

        /// <summary>
        /// 链外ID
        /// </summary>
        public int offchainId { get; set; }
    }
}
