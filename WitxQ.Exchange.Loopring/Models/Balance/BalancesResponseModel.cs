using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Balance
{
    /// <summary>
    /// 账户信息的响应实体
    /// </summary>
    public class BalancesResponseModel : BaseModel
    {
        /// <summary>
        /// API返回信息
        /// </summary>
        public ResultInfo resultInfo { get; set; }

        /// <summary>
        /// 账户数据
        /// </summary>
        public List<BalancesModel> data { get; set; }
    }
}
