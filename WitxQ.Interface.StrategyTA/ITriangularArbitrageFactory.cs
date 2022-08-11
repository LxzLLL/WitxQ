using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Interface.StrategyTA
{
    /// <summary>
    /// 创建三角套利的工厂接口
    /// </summary>
    public interface ITriangularArbitrageFactory
    {
        /// <summary>
        /// 创建三角套利
        /// </summary>
        /// <param name="taParams">创建三角套利的参数</param>
        /// <returns></returns>
        public ITriangularArbitrage Create(TriangularArbitrageParam taParams);
    }
}
