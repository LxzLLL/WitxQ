using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WitxQ.Exchange.Loopring.Tools;

namespace WitxQ.Exchange.Loopring.Models.Balance
{
    /// <summary>
    /// 以太坊地址Token的余额
    /// </summary>
    public class BalancesModel:BaseModel
    {
        /// <summary>
        /// 账号ID
        /// </summary>
        public int accountId { get; set; }

        /// <summary>
        /// Token ID
        /// <para>
        /// 0:ETH
        /// 2:LRC
        /// </para>
        /// </summary>
        public int tokenId { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public string totalAmount { get; set; }

        /// <summary>
        /// 冻结数量
        /// </summary>
        public string amountLocked { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PendingBalance pending { get; set; }


        /// <summary>
        /// Token的代号
        /// </summary>
        public string TokenSymbol
        {
            get
            {
                return ExLoopring.TOKENS[this.tokenId]?.symbol;
            }
        }

        /// <summary>
        /// 格式化后的总数量
        /// </summary>
        public decimal TotalAmountFormat
        {
            get
            {
                return LoopringConvert.GetLoopringNumber(this.totalAmount, this.tokenId);
            }
        }

        /// <summary>
        /// 格式化后的冻结数量
        /// </summary>
        public decimal FrozenAmountFormat
        {
            get
            {
                return LoopringConvert.GetLoopringNumber(this.amountLocked, this.tokenId);
            }
        }

        /// <summary>
        /// 格式化后的可用数量
        /// </summary>
        public decimal AvailableAmountFormat
        {
            get
            {
                return TotalAmountFormat - FrozenAmountFormat;
            }
        }
    }


    public class PendingBalance
    {
        public string withdraw { get; set; }

        public string deposit { get; set; }
    }
}
