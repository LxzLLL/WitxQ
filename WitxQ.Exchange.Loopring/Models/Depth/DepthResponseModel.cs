using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Depth
{
    /// <summary>
    /// 深度的响应实体
    /// </summary>
    public class DepthResponseModel : BaseModel
    {
        /// <summary>
        /// API返回信息
        /// </summary>
        public ResultInfo resultInfo { get; set; }

        /// <summary>
        /// 市场深度信息
        /// </summary>
        public DepthModel data { get; set; }
    }
}
