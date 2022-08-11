using Microsoft.AspNetCore.NodeServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WitxQ.Interface.Loopring;
using WitxQ.Server.Test;

namespace WitxQ.Server.SysFrame
{
    /// <summary>
    /// 使用netcore调用Nodejs
    /// </summary>
    public class NodejsLoopringSign : ISign
    {
        /// <summary>
        /// DI中的日志
        /// </summary>
        private readonly ServerLogger _logger;

        /// <summary>
        /// DI中的配置
        /// </summary>
        private readonly ExChanges _exChanges;

        /// <summary>
        /// nodeservice对象
        /// </summary>
        private readonly INodeServices _services;


        /// <summary>
        /// CEF初始化
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exChanges"></param>
        public NodejsLoopringSign(ServerLogger logger, ExChanges exChanges,INodeServices services)
        {
            this._logger = logger;
            this._exChanges = exChanges;
            this._services = services;
        }

        #region ISign

        /// <summary>
        /// 获取Hash
        /// </summary>
        /// <param name="args">参与生成hash的参数列表</param>
        /// <returns></returns>
        public string GetHash(List<Object> args)
        {
            //DateTime startTime = DateTime.Now;
            //this._logger.Info($"开始GetHash: {startTime}");
            string strHash = string.Empty;
            if (args == null || args.Count <= 0)
                return strHash;

            object objparam = args.ToArray();
            string strHasher = this._services.InvokeExportAsync<string>("./site.js", "createHash", objparam).Result;

           // DateTime endTime = DateTime.Now;
           // this._logger.Info($"结束GetHash: {endTime}，耗时：{(endTime-startTime).TotalMilliseconds} 毫秒");

            return strHasher;
        }


        /// <summary>
        /// 通过arg获取Signature
        /// <para>
        /// 集成了生成hash，不需要再调用GetHash
        /// </para>
        /// </summary>
        /// <param name="secretKey">密钥</param>
        /// <param name="args">参与生成hash的参数列表</param>
        /// <returns></returns>
        public string GetSignByArgs(string secretKey, List<Object> args)
        {
            //DateTime startTime = DateTime.Now;
            //this._logger.Info($"开始GetHash+GetSign: {startTime}");

            string hash = this.GetHash(args);
            string strSign = this.GetSign(secretKey, hash);

            //DateTime endTime = DateTime.Now;
            //this._logger.Info($"结束GetHash+GetSign: {endTime}，耗时：{(endTime - startTime).TotalMilliseconds} 毫秒");

            return strSign;
        }

        /// <summary>
        /// 通过hash获取Signature
        /// </summary>
        /// <param name="secretKey">密钥</param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public string GetSign(string secretKey, string hash)
        {
            //DateTime startTime = DateTime.Now;
            //this._logger.Info($"开始GetSign: {startTime}");

            string strSign = _services.InvokeExportAsync<string>("./site.js", "sign", secretKey, hash).Result;

            //DateTime endTime = DateTime.Now;
            //this._logger.Info($"结束GetSign: {endTime}，耗时：{(endTime - startTime).TotalMilliseconds} 毫秒");
            return strSign;
        }

        
        #endregion

        #region 测试
        public async void HashTest()
        {
            DateTime dtStart = DateTime.Now;
            string strHash = string.Empty;
            Console.WriteLine($"strHash:{strHash}");

            List<object> objs = new List<object>() { 2, 26711, 78, 0, 2, "5000000000000000", "500000000000000000000", false, 1590373869, 1593052872, 63, true, 211 };

            for (int i = 0; i < 1; i++)
            {
                //string args = $"[2,3333,88,3,2,'5013500','185000000000000000000',1,21342343534,5463463456,50,1,221]";
                strHash = this.GetHash(objs);
            }
            //Console.WriteLine($"strHash:{strHash}");

            DateTime dtEnd = DateTime.Now;
            double s = (dtEnd - dtStart).TotalMilliseconds;
            string str = $"strHash 10000次时间差：{s} ms";
            Console.WriteLine(str);
        }

        public async void SignHashTest()
        {
            string strSign = string.Empty;
            string strHash = string.Empty;
            DateTime dtStart = DateTime.Now;
            List<object> objs = new List<object>() { 2, 26711, 78, 0, 2, "5000000000000000", "500000000000000000000", false, 1590373869, 1593052872, 63, true, 211 };
            string key = "13412342";
            for (int i = 0; i < 1; i++)
            {
                //string args = $"[2,3333,88,3,2,'5013500','185000000000000000000',1,21342343534,5463463456,50,1,221]";
                strHash = this.GetHash(objs);
                strSign = this.GetSign(key, strHash);
            }
            DateTime dtEnd = DateTime.Now;
            double s = (dtEnd - dtStart).TotalMilliseconds;
            string str = $"Hash+Sign 1000次时间差：{s} ms,strHash={strHash},strSign={strSign}";
            Console.WriteLine(str);

            //Console.WriteLine($"strHash:{strHash}");
            //Console.WriteLine($"sign:{strSign}");
            //Console.WriteLine($"signRx:{sign.Rx}");
            //Console.WriteLine($"signRy:{sign.Ry}");
            //Console.WriteLine($"signS:{sign.s}");
        }
        #endregion


    }
}
