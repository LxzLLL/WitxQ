using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Interface.Loopring;
using WitxQ.Server.Test;
using WitxQ.Server.Tools;

namespace WitxQ.Server.SysFrame
{
    /// <summary>
    /// 使用Nodejs express搭建的api获取sign
    /// </summary>
    public class ApiLoopringSign : ISign
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
        /// http请求类
        /// </summary>
        private readonly HttpRequestClient2 _client;

        /// <summary>
        /// CEF初始化
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exChanges"></param>
        public ApiLoopringSign(ServerLogger logger, ExChanges exChanges, HttpRequestClient2 httpRequestClient)
        {
            this._logger = logger;
            this._exChanges = exChanges;
            this._client = httpRequestClient;
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

            string argsStr = "[";
            args.ForEach(o =>
            {
                // int  long  string bool
                if (o is System.String)
                {
                    argsStr += "'" + o + "',";
                }
                else if (o is int || o is long)
                {
                    argsStr += o + ",";
                }
                else if (o is bool)
                {
                    argsStr += o.ToString().ToLower() + ",";
                }
            });
            argsStr = argsStr.TrimEnd(',') + "]";
            string strHasher = this._client.Get<string>(new { pars = argsStr },null,"/createHash");

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

            string strSign = string.Empty;
            if (string.IsNullOrWhiteSpace(secretKey) || args == null || args.Count <= 0)
                return strSign;

            //DateTime startTime = DateTime.Now;
            //this._logger.Info($"开始GetHash+GetSign: {startTime}");

            #region  方式一
            //string hash = this.GetHash(args);
            //string strSign = this.GetSign(secretKey, hash);
            #endregion


            #region  方式二

            string argsStr = "[";
            args.ForEach(o =>
            {
                // int  long  string bool
                if (o is System.String)
                {
                    argsStr += "'" + o + "',";
                }
                else if (o is int || o is long)
                {
                    argsStr += o + ",";
                }
                else if (o is bool)
                {
                    argsStr += o.ToString().ToLower() + ",";
                }
            });
            argsStr = argsStr.TrimEnd(',') + "]";

            strSign = this._client.Get<string>(new { key = secretKey, pars = argsStr }, null, "/createHashAndsign");
            #endregion

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

            string strSign = this._client.Get<string>(new { key = secretKey, msg = hash }, null, "/sign");

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
            for (int i = 0; i < 1000; i++)
            {
                //strHash = this.GetHash(objs);
                //strSign = this.GetSign(key, strHash);
                strSign = this.GetSignByArgs(key, objs);
                
            }
            DateTime dtEnd = DateTime.Now;
            double s = (dtEnd - dtStart).TotalMilliseconds;
            string str = $"Hash+Sign 1000次时间差：{s} ms,strHash={strHash},strSign={strSign}";
            Console.WriteLine(str);
            this._logger.Info(str);
            //Console.WriteLine($"strHash:{strHash}");
            //Console.WriteLine($"sign:{strSign}");
            //Console.WriteLine($"signRx:{sign.Rx}");
            //Console.WriteLine($"signRy:{sign.Ry}");
            //Console.WriteLine($"signS:{sign.s}");
        }
        #endregion

    }
}
