using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using WitxQ.Server.SysFrame;

namespace WitxQ.Server
{
    public class QuantWorker: BackgroundService
    {

        /// <summary>
        /// DI中的日志
        /// </summary>
        private readonly ServerLogger _logger;

        /// <summary>
        /// DI中的QuantEngine
        /// </summary>
        private readonly QuantEngine _quantEngine;

        public QuantWorker(ServerLogger logger, QuantEngine quantEngine)
        {
            this._logger = logger;
            this._quantEngine = quantEngine;
        }

        //重写BackgroundService.StartAsync方法，在开始服务的时候，执行一些处理逻辑，这里我们仅输出一条日志
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            this._logger.Info($"Worker starting at: {DateTimeOffset.Now}");

            await base.StartAsync(cancellationToken);
        }


        /// <summary>
        /// 异步执行
        /// </summary>
        /// <param name="stoppingToken">取消任务的令牌</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                //使用await关键字，异步等待RunTask，这样调用ExecuteAsync方法的线程会立即返回，不会卡在这里被阻塞
                await Task.Run(() =>
                {
                    //如果服务被停止，那么下面的IsCancellationRequested会返回true，我们就应该结束循环
                    //while (!stoppingToken.IsCancellationRequested)
                    //{
                    //    this._logger.Info($"RunTaskThree running at: {DateTimeOffset.Now}");
                    //    Thread.Sleep(1000);
                    //}

                    if (!this._quantEngine.IsStart)
                        this._quantEngine.Start();
                }, stoppingToken);
                
            }
            catch (Exception ex)
            {
                this._logger.Error($"Worker executing error",ex);
            }
            finally
            {
                //Worker Service服务停止后，如果有需要收尾的逻辑，可以写在这里
                this._logger.Info($"Worker executed completed");
            }
            //this._logger.Info($"Worker executed completed");

        }

        //重写BackgroundService.StopAsync方法，在结束服务的时候，执行一些处理逻辑，这里我们仅输出一条日志
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            this._logger.Info($"Worker stopping at: {DateTimeOffset.Now}");
            await base.StopAsync(cancellationToken);
        }
    }
}
