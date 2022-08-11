using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WitxQ.Common;
using WitxQ.Common.Logger;
using WitxQ.Server.SysFrame;
using Microsoft.Extensions.Logging;
using WitxQ.Server.Test;
using WitxQ.Interface.Loopring;
using Microsoft.AspNetCore.NodeServices;
using WitxQ.Server.Tools;

namespace WitxQ.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// 创建 hostbuilder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(configHost =>
            {
                // 主机配置
                // 程序执行入口所在目录
                string strExecuteDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                configHost.SetBasePath(strExecuteDirectory);
                configHost.AddJsonFile("hostsettings.json", optional: true);   // 主机的配置文件
                //configHost.AddEnvironmentVariables(prefix: "PREFIX_");    // 主机的环境变量
            })
            .ConfigureAppConfiguration((hostContext, configApp) =>
            {
                // App应用配置
                configApp.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                //configApp.AddEnvironmentVariables(prefix: "PREFIX_");
                //configApp.AddCommandLine(args);
            })
            .ConfigureServices((hostContext, services) =>
            {
                // 1、注册后台普通服务
                #region 1.1 日志log4net，根据配置文件扩展ServerLogger日志实用类
                // 注入日志配置日志
                string strFileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "log4net.win.config" : "log4net.linux.config";
                //此路径为程序运行起点路径（例如web中的bin目录）
                string configPath = PathCombine.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "/Config/", strFileName);

                WitxQ.Common.Logger.ILogger logger = new LoggerManager().SetConfig(configPath).SetLoggerFactory(new Logger4Factory()).GetLoggerSingletonInstance();
                //添加ILogger服务
                services.AddSingleton<WitxQ.Common.Logger.ILogger>(logger);
                services.AddSingleton<ServerLogger>();
                #endregion

                // 1.2 配置信息
                ExChanges exChanges = hostContext.Configuration.GetSection("ExChanges").Get<ExChanges>();
                services.AddSingleton<ExChanges>(exChanges);

                // 1.3 添加ISign接口
                //services.AddSingleton<ISign, CefLoopringSign>();


                #region （注释）调用nodejs方式 Microsoft.AspNetCore.NodeServices(需nuget)
                //services.AddNodeServices();
                //var option = new NodeServicesOptions(services.BuildServiceProvider())
                //{
                //    LaunchWithDebugging = false,
                //    ProjectPath = Path.Combine("./Script/")
                //};
                //var nodeservice = NodeServicesFactory.CreateNodeServices(option);
                //services.AddSingleton<INodeServices>(nodeservice);
                //services.AddSingleton<ISign, NodejsLoopringSign>();
                #endregion

                #region 使用nodejs express api方式
                HttpRequestClient2 httpRequest = new HttpRequestClient2(exChanges.LoopringSignUrl);
                services.AddSingleton<HttpRequestClient2>(httpRequest);

                services.AddSingleton<ISign, ApiLoopringSign>();
                #endregion




                // 1.4 量化引擎
                services.AddSingleton<QuantEngine>();

                // 2、添加主机应用服务
                services.AddHostedService<QuantWorker>();

                // 3、服务加载器初始化，用于获取注入的组件
                ServiceLocator.Instance = services.BuildServiceProvider();
            })
            .ConfigureLogging((hostContext, configLogging) =>
            {
                configLogging.AddConsole();
                configLogging.AddDebug();
            })
            .UseWindowsService();
            //.UseConsoleLifetime()
    }
}
