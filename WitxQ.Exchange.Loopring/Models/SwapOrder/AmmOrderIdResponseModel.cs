using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.SwapOrder
{
    /// <summary>
    /// Amm订单ID响应模型
    /// </summary>
    public class AmmOrderIdResponseModel : BaseModel
    {
        /// <summary>
        /// 返回结果信息
        /// </summary>
        public ResultInfo resultInfo { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public AmmOrderIdDataModel data { get; set; }
    }

    public class AmmOrderIdDataModel : BaseModel
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int orderId { get; set; }

        /// <summary>
        /// 链下ID（暂不知何用）
        /// </summary>
        public int offchainId { get; set; }
    }
}
