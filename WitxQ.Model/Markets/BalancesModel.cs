using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Model.Markets
{
    /// <summary>
    /// 账户信息
    /// </summary>
    public class BalancesModel : BaseModel
    {
        /// <summary>
        /// token标识，大写形式，例如ETH
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 可用数量
        /// </summary>
        public decimal Available { get; set; }

        /// <summary>
        /// 冻结数量
        /// </summary>
        public decimal Locked { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public decimal Total 
        { 
            get
            {
                return this.Available + this.Locked;
            }
        }
    }
}
