using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models
{
    /// <summary>
    /// 交易所模型信息
    /// </summary>
    public class ExchangeModel : BaseModel
    {
        /// <summary>
        /// 交易所支持的以太坊网络编号
        /// </summary>
        public int chainId { get; set; }

        /// <summary>
        /// 交易所在路印协议合约的编号
        /// </summary>
        public int exchangeId { get; set; }

        /// <summary>
        /// 交易所合约地址
        /// </summary>
        public string exchangeAddress { get; set; }

        /// <summary>
        /// 交易所收费信息
        /// </summary>
        public List<FeeInfo> onchainFees { get; set; }
    }

    /// <summary>
    /// 交易所收费信息
    /// </summary>
    public class FeeInfo:BaseModel
    {
        /// <summary>
        /// 费用类型，例如:"withdraw"
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 费用数量，单位为Wei数量的以太坊，例如："2000000000000000"
        /// </summary>
        public string fee { get; set; }

    }
}
