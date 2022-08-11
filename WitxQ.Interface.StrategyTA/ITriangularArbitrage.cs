using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WitxQ.Interface.Spot;
using WitxQ.Interface.Strategy;

namespace WitxQ.Interface.StrategyTA
{
    /// <summary>
    /// 三角套利接口
    /// </summary>
    public interface ITriangularArbitrage:IStrategy
    {
        /// <summary>
        /// 设置此三角套利策略
        /// </summary>
        /// <param name="order">Order接口</param>
        /// <param name="depth">深度接口</param>
        /// <param name="balances">账户接口</param>
        public void SetTATask(IOrder order,IDepth depth, IBalances balances);

    }
}
