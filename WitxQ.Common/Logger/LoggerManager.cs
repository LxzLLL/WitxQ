using log4net;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WitxQ.Common.Logger
{
    /// <summary>
    ///  日志管理器，用于管理具体使用哪个具象化的logger
    ///  1、使用时先用SetCurrent设置工厂对象
    ///  2、再使用ILogger获取具体的日志操作对象LoggerManager.Logger
    /// </summary>
    public class LoggerManager
    {
        /// <summary>
        /// 默认制定日志仓储
        /// </summary>
        public static ILoggerRepository loggerRepository = LogManager.CreateRepository("NETCoreRepository");

        /// <summary>
        /// 加载的配置文件路径
        /// </summary>
        public string ConfigPath { get; private set; }

        #region 私有字段
        private static ILoggerFactory _currentLogFactory;
        #endregion

        public LoggerManager()
        {
            
        }


        #region 获取单例ILogger
        /// <summary>
        /// 获取单例的ILogger
        /// </summary>
        /// <returns></returns>
        public ILogger GetLoggerSingletonInstance()
        {
            return LoggerSingleton.GetInstance();
        }

        static class LoggerSingleton
        {
            private static readonly ILogger instance = _currentLogFactory.Create();
            public static ILogger GetInstance()
            {
                return LoggerSingleton.instance;
            }
        }
        #endregion

        #region 公开方法

        
        /// <summary>
        /// 设置配置文件路径
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        public LoggerManager SetConfig(string configPath = "")
        {
            string strFileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "log4net.win.example.config" : "log4net.linux.example.config";
            //此路径为程序运行起点路径（例如web中的bin目录）
            string defaultConfigPath = PathCombine.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "/Logger/Log4Net/", strFileName);

            //如果无配置文件，则使用默认的配置文件
            this.ConfigPath = string.IsNullOrWhiteSpace(configPath) ? defaultConfigPath : configPath;

            //如果无配置文件，则使用默认的配置文件
            using (var fs = System.IO.File.OpenRead(this.ConfigPath))
            {
                log4net.Config.XmlConfigurator.Configure(LoggerManager.loggerRepository, fs);
            }

            return this;
        }


        /// <summary>
        /// 设置当前使用的日志工厂
        /// </summary>
        /// <param name="logFactory">日志工厂</param>
        public LoggerManager SetLoggerFactory(ILoggerFactory logFactory)
        {
            _currentLogFactory = logFactory;
            return this;
        }
        #endregion
    }
}
