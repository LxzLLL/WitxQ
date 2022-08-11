using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Market
{
    /// <summary>
    /// 交易所支持的市场的响应实体
    /// </summary>
    public class MarketInfoResponseModel : BaseModel
    {
        /// <summary>
        /// API返回信息
        /// </summary>
        public ResultInfo resultInfo { get; set; }

        /// <summary>
        /// 通证信息
        /// </summary>
        public List<MarketInfoModel> data { get; set; }
    }
}
