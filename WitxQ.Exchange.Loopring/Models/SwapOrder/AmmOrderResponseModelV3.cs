using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.SwapOrder
{
    /// <summary>
    /// Amm提交订单的响应模型V3版本
    /// </summary>
    public class AmmOrderResponseModelV3 : BaseModel
    {
        /// <summary>
        /// 返回结果信息
        /// </summary>
        public ResultInfo resultInfo { get; set; }

        /// <summary>
        /// 返回的结果详情
        /// </summary>
        public SubmitOrderResponseV3Item data { get; set; }

    }

    /// <summary>
    /// Amm提交订单的响应结果模型V3版本
    /// </summary>
    public class SubmitOrderResponseV3Item
    {
        /// <summary>
        /// 提交的订单的hash
        /// </summary>
        public string orderHash { get; set; }

        /// <summary>
        /// 提交订单时系统内订单状态,取值范围 : ['NEW_ACTIVED', 'SUBMIT_ORDER_FAIL', 'UNKNOWN']
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 提交订单是否幂等, UNKNOWN状态和WAIT_FREEZE_BALANCE, 重新提交幂等为true, 其他情况为false 取值范围 : [True, False]
        /// </summary>
        public bool isIdempotent { get; set; }
    }
}
