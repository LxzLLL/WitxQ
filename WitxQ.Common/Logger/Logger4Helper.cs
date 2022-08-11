using System;

//[assembly: log4net.Config.XmlConfigurator(ConfigFile = @"log4net.config", Watch = true)]
namespace WitxQ.Common.Logger
{
    internal class Logger4Helper : ILogger
    {
        /// <summary>
        /// 记录Debug信息
        /// </summary>
        /// <param name="message"> 需记录的信息 </param>
        /// <param name="loggerName">具体的Logger名称 </param>
        public void Debug(string message, string loggerName)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(LoggerManager.loggerRepository.Name, loggerName);
            log.Debug(message);
        }

        /// <summary>
        /// 记录Debug信息
        /// </summary>
        /// <param name="message"> 需记录的信息 </param>
        /// <param name="exception"> 异常类型 </param>
        /// <param name="loggerName">具体的Logger名称</param>
        public void Debug(string message, Exception exception, string loggerName)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(LoggerManager.loggerRepository.Name, loggerName);
            log.Debug(message, exception);
        }

        /// <summary>
        /// 记录Fatal信息
        /// </summary>
        /// <param name="message"> 需记录的信息 </param>
        /// <param name="loggerName"> 具体的Logger名称 </param>
        public void Fatal(string message, string loggerName)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(LoggerManager.loggerRepository.Name, loggerName);
            log.Fatal(message);
        }

        /// <summary>
        /// 记录Fatal信息
        /// </summary>
        /// <param name="message"> 需记录的信息 </param>
        /// <param name="exception"> 异常类型 </param>
        public void Fatal(string message, Exception exception, string loggerName)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(LoggerManager.loggerRepository.Name, loggerName);
            log.Fatal(message, exception);
        }

        /// <summary>
        /// 记录Info信息
        /// </summary>
        /// <param name="message"> 需记录的信息 </param>
        /// <param name="loggerName"> 具体的Logger名称 </param>
        public void Info(string message, string loggerName)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(LoggerManager.loggerRepository.Name, loggerName);
            log.Info(message);
        }

        /// <summary>
        ///  记录Warning信息
        /// </summary>
        /// <param name="message"> 需记录的信息</param>
        /// <param name="loggerName">具体的Logger名称</param>
        public void Warning(string message, string loggerName)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(LoggerManager.loggerRepository.Name, loggerName);
            log.Warn(message);
        }

        /// <summary>
        /// 记录Error信息
        /// </summary>
        /// <param name="message"> 需记录的信息 </param>
        /// <param name="loggerName">具体的Logger名称 </param>
        public void Error(string message, string loggerName)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(LoggerManager.loggerRepository.Name, loggerName);
            log.Error(message);
        }

        /// <summary>
        /// 记录Error信息
        /// </summary>
        /// <param name="message">需记录的信息 </param>
        /// <param name="exception"> 异常类型 </param>
        /// <param name="loggerName"> 具体的Logger名称 </param>
        public void Error(string message, Exception exception, string loggerName)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(LoggerManager.loggerRepository.Name, loggerName);
            log.Error(message, exception);
        }

    }
}
