using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Order
{
    /// <summary>
    /// 批量撤销订单的请求模型
    /// </summary>
    public class OrderDelBatchRequestModel : BaseModel
    {
        /// <summary>
        /// 账号ID
        /// </summary>
        public int accountId { get; set; }

        /// <summary>
        /// 要取消的订单HASH,多个HASH以","分隔
        /// </summary>
        public string orderHash { get; set; }
    }
}
