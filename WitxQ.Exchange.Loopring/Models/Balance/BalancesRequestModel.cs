using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Balance
{
    /// <summary>
    /// 账户的请求实体
    /// </summary>
    public class BalancesRequestModel : BaseModel
    {
        /// <summary>
        /// 账号ID
        /// </summary>
        public int accountId { get; set; }

        /// <summary>
        /// 资产列表（不是必须的），例如："0,1"
        /// </summary>
        public string tokens { get; set; }
    }
}
