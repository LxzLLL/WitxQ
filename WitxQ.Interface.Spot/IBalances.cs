using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Model.Markets;

namespace WitxQ.Interface.Spot
{
    /// <summary>
    /// 账户信息
    /// </summary>
    public interface IBalances
    {
        /// <summary>
        /// 获取账号的 token余额信息
        /// </summary>
        /// <param name="token">token，大写形式，例如ETH</param>
        /// <returns></returns>
        public BalancesModel GetBalancesByPair(string token);

        /// <summary>
        /// 获取账号 所有token的余额信息
        /// </summary>
        /// <returns></returns>
        public List<BalancesModel> GetBalances();
    }
}
