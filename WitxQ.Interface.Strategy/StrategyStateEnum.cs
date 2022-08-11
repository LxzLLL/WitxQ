using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Interface.Strategy
{
    /// <summary>
    /// 策略状态
    /// </summary>
    public enum StrategyStateEnum
    {
        /// <summary>
        /// 未执行（刚创建）
        /// </summary>
        UnExecuted,
        /// <summary>
        /// 正在运行
        /// </summary>
        Running,
        /// <summary>
        /// 暂停
        /// </summary>
        Suspend,
        /// <summary>
        /// 执行完毕（主动策略stop）
        /// </summary>
        Completed,
        /// <summary>
        /// 异常状态
        /// </summary>
        Abnormal
    }
}
