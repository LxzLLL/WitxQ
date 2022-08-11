using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Server.Test
{
    public class User
    {
        /// <summary>
        /// 用户的账户
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// 三角套利策略
        /// </summary>
        public List<TAStrategy> TAStrategies { get; set; }

        /// <summary>
        /// 做市商策略
        /// </summary>
        public List<MMStrategy> MMStrategies { get; set; }

        /// <summary>
        /// Swap策略配置
        /// </summary>
        public List<SwapStrategyConfig> SWAPStrategies { get; set; }

        /// <summary>
        /// SwapDG动态网格策略配置
        /// </summary>
        public List<SwapDGConfig> SwapDGStrategies { get; set; }
    }
}
