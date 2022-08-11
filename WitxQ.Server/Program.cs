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
        /// ���� hostbuilder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(configHost =>
            {
                // ��������
                // ����ִ���������Ŀ¼
                string strExecuteDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                configHost.SetBasePath(strExecuteDirectory);
                configHost.AddJsonFile("hostsettings.json", optional: true);   // �����������ļ�
                //configHost.AddEnvironmentVariables(prefix: "PREFIX_");    // �����Ļ�������
            })
            .ConfigureAppConfiguration((hostContext, configApp) =>
            {
                // AppӦ������
                configApp.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                //configApp.AddEnvironmentVariables(prefix: "PREFIX_");
                //configApp.AddCommandLine(args);
            })
            .ConfigureServices((hostContext, services) =>
            {
                // 1��ע���̨��ͨ����
                #region 1.1 ��־log4net�����������ļ���չServerLogger��־ʵ����
                // ע����־������־
                string strFileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "log4net.win.config" : "log4net.linux.config";
                //��·��Ϊ�����������·��������web�е�binĿ¼��
                string configPath = PathCombine.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "/Config/", strFileName);

                WitxQ.Common.Logger.ILogger logger = new LoggerManager().SetConfig(configPath).SetLoggerFactory(new Logger4Factory()).GetLoggerSingletonInstance();
                //���ILogger����
                services.AddSingleton<WitxQ.Common.Logger.ILogger>(logger);
                services.AddSingleton<ServerLogger>();
                #endregion

                // 1.2 ������Ϣ
                ExChanges exChanges = hostContext.Configuration.GetSection("ExChanges").Get<ExChanges>();
                services.AddSingleton<ExChanges>(exChanges);

                // 1.3 ���ISign�ӿ�
                //services.AddSingleton<ISign, CefLoopringSign>();


                #region ��ע�ͣ�����nodejs��ʽ Microsoft.AspNetCore.NodeServices(��nuget)
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

                #region ʹ��nodejs express api��ʽ
                HttpRequestClient2 httpRequest = new HttpRequestClient2(exChanges.LoopringSignUrl);
                services.AddSingleton<HttpRequestClient2>(httpRequest);

                services.AddSingleton<ISign, ApiLoopringSign>();
                #endregion




                // 1.4 ��������
                services.AddSingleton<QuantEngine>();

                // 2���������Ӧ�÷���
                services.AddHostedService<QuantWorker>();

                // 3�������������ʼ�������ڻ�ȡע������
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
