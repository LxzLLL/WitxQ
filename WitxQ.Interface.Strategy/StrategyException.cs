using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Interface.Strategy
{
    /// <summary>
    /// 策略异常
    /// </summary>
    public abstract class StrategyException:Exception
    {
        /// <summary>
        /// 策略类型的字符串
        /// </summary>
        public string StrategyType { get; set; }

        /// <summary>
        /// 策略的错误编码
        /// </summary>
        public string ErrCode { get; set; }

        /// <summary>
        /// 策略异常
        /// </summary>
        /// <param name="strategyType">策略类型</param>
        /// <param name="errCode">策略的错误编码</param>
        /// <param name="message">策略的错误消息</param>
        public StrategyException(string strategyType,string errCode, string message):base(message)
        {
            this.StrategyType = strategyType;
            this.ErrCode = errCode;
        }

        /// <summary>
        /// 策略异常
        /// </summary>
        /// <param name="strategyType">策略类型</param>
        /// <param name="errCode">策略的错误编码</param>
        /// <param name="message">策略的错误消息</param>
        /// <param name="innerException">异常</param>
        public StrategyException(string strategyType, string errCode, string message, Exception? innerException) : base(message,innerException)
        {
            this.StrategyType = strategyType;
            this.ErrCode = errCode;
        }
    }
}
