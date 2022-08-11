using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Exchange.Loopring.Models.Order;

namespace WitxQ.Exchange.Loopring.Models.SwapOrder
{
    /// <summary>
    /// AMM的OrderRequestModel
    /// </summary>
    public class AmmOrderRequestModel : OrderRequestModel
    {
        /// <summary>
        /// 池的地址
        /// </summary>
        public string poolAddress { get; set; }
    }
}
