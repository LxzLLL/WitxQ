using System;

namespace WitxQ.Interface.Strategy
{
    /// <summary>
    /// 策略接口
    /// </summary>
    public interface IStrategy
    {
        /// <summary>
        /// 运行此策略
        /// </summary>
        /// <returns></returns>
        public bool Run();

        /// <summary>
        /// 停止此策略
        /// </summary>
        /// <returns></returns>
        public bool Stop();

        /// <summary>
        /// 暂停此策略
        /// </summary>
        /// <returns></returns>
        public bool Suspend();

        /// <summary>
        /// 获取此策略的状态
        /// </summary>
        /// <returns></returns>
        public StrategyStateEnum GetStrategyState();

        /// <summary>
        /// 获取此策略的异常
        /// </summary>
        /// <returns></returns>
        //public StrategyException GetStrategyException();
    }
}
