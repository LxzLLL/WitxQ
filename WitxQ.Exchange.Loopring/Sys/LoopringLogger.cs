using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Common.Logger;

namespace WitxQ.Exchange.Loopring.Sys
{
    /// <summary>
    /// 整个工程的日志类
    /// <para>
    /// ILogger通过Autofac注入
    /// </para>
    /// </summary>
    public class LoopringLogger
    {
        private ILogger _logger;
        // 日志名称，默认RootLogger
        protected string strLoggerName = "LoopringLogger";
        public LoopringLogger(ILogger logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// 记录Debug信息
        /// </summary>
        /// <param name="message"> 需记录的信息 </param>
        public void Debug(string message)
        {
            string strDebug = $"【日志信息】：{message}";
            this._logger.Debug(strDebug, this.strLoggerName);
        }

        /// <summary>
        /// 记录Debug信息
        /// </summary>
        /// <param name="message"> 需记录的信息 </param>
        /// <param name="exception"> 异常类型 </param>
        public void Debug(string message, Exception exception)
        {
            string strDebug = $"【日志信息】：{message}";
            this._logger.Debug(strDebug, exception, this.strLoggerName);
        }

        /// <summary>
        /// 记录Fatal信息
        /// </summary>
        /// <param name="message"> 需记录的信息 </param>
        public void Fatal(string message)
        {
            string strFatal = $"【日志信息】：{message}";
            this._logger.Fatal(strFatal, this.strLoggerName);
        }

        /// <summary>
        /// 记录Fatal信息
        /// </summary>
        /// <param name="message"> 需记录的信息 </param>
        /// <param name="exception"> 异常类型 </param>
        public void Fatal(string message, Exception exception)
        {
            string strFatal = $"【日志信息】：{message}";
            this._logger.Fatal(strFatal, exception, this.strLoggerName);
        }

        /// <summary>
        /// 记录Info信息
        /// </summary>
        /// <param name="message"> 需记录的信息 </param>
        public void Info(string message)
        {
            string strInfo = $"【日志信息】：{message}";
            this._logger.Info(strInfo, this.strLoggerName);
        }

        /// <summary>
        ///  记录Warning信息
        /// </summary>
        /// <param name="message"> 需记录的信息</param>
        /// <param name="loggerName">具体的Logger名称</param>
        public void Warning(string message)
        {
            string strWarning = $"【日志信息】：{message}";
            this._logger.Warning(strWarning, this.strLoggerName);
        }

        /// <summary>
        /// 记录Error信息
        /// </summary>
        /// <param name="message"> 需记录的信息 </param>
        /// <param name="loggerName">具体的Logger名称 </param>
        public void Error(string message)
        {
            string strError = $"【日志信息】：{message}";
            this._logger.Error(strError, this.strLoggerName);
        }

        /// <summary>
        /// 记录Error信息
        /// </summary>
        /// <param name="message">需记录的信息 </param>
        /// <param name="exception"> 异常类型 </param>
        /// <param name="loggerName"> 具体的Logger名称 </param>
        public void Error(string message, Exception exception)
        {

            string strError = $"【抛出信息】：{message} \r\n【异常类型】：{exception.GetType().Name} \r\n【异常信息】：{exception.Message} \r\n【堆栈调用】：{exception.StackTrace}";
            this._logger.Error(strError, this.strLoggerName);
        }

    }
}
