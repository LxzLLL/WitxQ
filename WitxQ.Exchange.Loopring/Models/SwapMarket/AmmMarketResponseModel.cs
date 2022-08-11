using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.SwapMarket
{
    /// <summary>
    /// 交易所支持的市场信息的响应实体
    /// </summary>
    public class AmmMarketResponseModel : BaseModel
    {
        /// <summary>
        /// API返回信息
        /// </summary>
        public ResultInfo resultInfo { get; set; }

        /// <summary>
        /// 市场信息
        /// </summary>
        public List<AmmMarketModel> data { get; set; }
    }
}
