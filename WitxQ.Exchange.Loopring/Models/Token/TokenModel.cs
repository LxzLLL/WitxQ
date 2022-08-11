using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Exchange.Loopring.Tools;

namespace WitxQ.Exchange.Loopring.Models.Token
{
    /// <summary>
    /// token信息（通证信息）
    /// </summary>
    public class TokenModel:BaseModel
    {
        /// <summary>
        /// 通证的种类取值范围 : ['ERC20', 'ETH']，例如"ERC20"
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 通证在路印协议注册的编号
        /// </summary>
        public int tokenId { get; set; }

        /// <summary>
        /// 通证的符号，例如:"LRC"
        /// </summary>
        public string symbol { get; set; }

        /// <summary>
        /// token名称，例如:"Loopring"
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 通证的地址，例如"0x97241525fe425C90eBe5A41127816dcFA5954b06"
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 通证合约中定义的通证小数位，例如"18"
        /// </summary>
        public int decimals { get; set; }

        /// <summary>
        /// 通证数量保留的小数位，例如"6"
        /// </summary>
        public int precision { get; set; }

        /// <summary>
        /// 单笔订单允许的最小下单数量，例如"10000000000000000"
        /// </summary>
        public string minOrderAmount { get; set; }

        /// <summary>
        /// 单笔订单允许的最大下单数量，例如"1000000000000000000"
        /// </summary>
        public string maxOrderAmount { get; set; }

        /// <summary>
        /// 订单被判定为灰尘单的数量，即小于该数量则不会继续撮合，例如"1000000000000000"
        /// </summary>
        public string dustOrderAmount { get; set; }

        /// <summary>
        /// 服务端当前是否支持该通证的充值提现等操作
        /// </summary>
        public bool enabled { get; set; }

        /// <summary>
        /// 快速取款限额，例如"20000000000000000000"
        /// </summary>
        public string fastWithdrawLimit { get; set; }


        /// <summary>
        /// 格式化后的单笔订单允许的最小下单数量
        /// </summary>
        public decimal Order_MinAmount
        {
            get
            {
                return LoopringConvert.GetLoopringNumber(this.minOrderAmount, this.tokenId);
            }
        }

        /// <summary>
        /// 格式化后的单笔订单允许的最大下单数量
        /// </summary>
        public decimal Order_MaxAmount
        {
            get
            {
                return LoopringConvert.GetLoopringNumber(this.maxOrderAmount, this.tokenId);
            }
        }

        /// <summary>
        /// 格式化后的灰尘单的数量
        /// </summary>
        public decimal Order_DustAmount
        {
            get
            {
                return LoopringConvert.GetLoopringNumber(this.dustOrderAmount, this.tokenId);
            }
        }
    }
}
