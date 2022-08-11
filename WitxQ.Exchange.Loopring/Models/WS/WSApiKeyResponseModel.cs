using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.WS
{
    /// <summary>
    /// Websocket所需的REST Api的返回
    /// </summary>
    public class WSApiKeyResponseModel : BaseModel
    {
        /// <summary>
        /// API返回信息
        /// </summary>
        public ResultInfo resultInfo { get; set; }

        /// <summary>
        /// 通证信息
        /// </summary>
        public string data { get; set; }
    }
}
