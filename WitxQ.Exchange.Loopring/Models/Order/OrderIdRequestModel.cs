using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Order
{
    /// <summary>
    /// 订单ID请求模型
    /// </summary>
    public class OrderIdRequestModel : BaseModel
    {
        /// <summary>
        /// 账号ID
        /// </summary>
        public int accountId { get; set; }

        /// <summary>
        /// 需要卖出的Token ID
        /// </summary>
        public int tokenSId { get; set; }
    }
}
