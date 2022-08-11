using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Token
{
    /// <summary>
    /// 交易所支持的通证信息的响应实体
    /// </summary>
    public class TokenResponseModel : BaseModel
    {
        /// <summary>
        /// API返回信息
        /// </summary>
        public ResultInfo resultInfo { get; set; }

        /// <summary>
        /// 通证信息
        /// </summary>
        public List<TokenModel> data { get; set; }
    }
}
