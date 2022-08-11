using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Sys
{
    /// <summary>
    /// 全局常量
    /// </summary>
    public class SysConst
    {
        /// <summary>
        /// 接入webSocket的URL
        /// </summary>
        //public const string WEBSOCKET_URI = "wss://ws.loopring.io/v2/ws";

        /// <summary>
        /// BaseUrl，访问的url
        /// </summary>
        //public const string API_BASEURL = "https://api.loopring.io";

        /// <summary>
        /// loopring的exchange交易所地址
        /// </summary>
        public const string EXCHANGE_ADDRESS = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4";
        /// <summary>
        /// 由用户指定的P2P订单使用的taker，当前使用0x0000000000000000000000
        /// </summary>
        public const string EXCHANGE_TAKER = "0x0000000000000000000000";

    }
}
