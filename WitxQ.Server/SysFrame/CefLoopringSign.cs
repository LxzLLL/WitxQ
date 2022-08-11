using CefSharp;
using CefSharp.OffScreen;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WitxQ.Common;
using WitxQ.Interface.Loopring;
using WitxQ.Server.Test;

namespace WitxQ.Server.SysFrame
{
    /// <summary>
    /// 嵌套CEF的签名（调用js）
    /// </summary>
    public class CefLoopringSign:ISign
    {
        /// <summary>
        /// hash和sign的URL
        /// </summary>
        private string _signUrl;

        /// <summary>
        /// 记载的浏览器
        /// </summary>
        private ChromiumWebBrowser _chromeBrowser;

        /// <summary>
        /// DI中的日志
        /// </summary>
        private readonly ServerLogger _logger;

        /// <summary>
        /// DI中的配置
        /// </summary>
        private readonly ExChanges _exChanges;

        /// <summary>
        /// CEF初始化
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exChanges"></param>
        public CefLoopringSign(ServerLogger logger, ExChanges exChanges)
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;

            this._logger = logger;
            this._exChanges = exChanges;
            this._signUrl = this._exChanges.LoopringSignUrl;

            this.LoadCef();
        }

        #region 单例

        //private CefLoopringSign()
        //{
        //    //AppDomain.CurrentDomain.AssemblyResolve += Resolver;
        //}

        ///// <summary>
        ///// 获取WsManager单例
        ///// </summary>
        ///// <returns></returns>
        //public static CefLoopringSign GetInstance()
        //{
        //    return InnerInstance.instance;
        //}

        //private class InnerInstance
        //{
        //    /// <summary>
        //    /// 当一个类有静态构造函数时，它的静态成员变量不会被beforefieldinit修饰
        //    /// 就会确保在被引用的时候才会实例化，而不是程序启动的时候实例化
        //    /// </summary>
        //    static InnerInstance() { }
        //    internal static CefLoopringSign instance = new CefLoopringSign();
        //}
        #endregion

        /// <summary>
        /// 加载Cef browser
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void LoadCef()
        {
            var settings = new CefSettings();
            // Set BrowserSubProcessPath based on app bitness at runtime
            settings.BrowserSubprocessPath = PathCombine.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                   Environment.Is64BitProcess ? "x64" : "x86",
                                                   "CefSharp.BrowserSubprocess.exe");
            // Make sure you set performDependencyCheck false
            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
            this._chromeBrowser = new ChromiumWebBrowser();

            // 浏览器初始化后的事件
            this._chromeBrowser.BrowserInitialized += ChromeBrowser_BrowserInitialized;
            // 页面主Frame加载完成后的事件
            this._chromeBrowser.FrameLoadEnd += BrowserFrameLoadEnd;
            

        }

        #region 参考代码 注释
        ///// <summary>
        ///// 创建order的Hash
        ///// </summary>
        ///// <param name="rodm"></param>
        ///// <returns></returns>
        //public async Task<string> GetHash(OrderRequestModel orderRequest)
        //{
        //    string strHash = string.Empty;
        //    string args = $"[{orderRequest.exchangeId},{orderRequest.orderId},{orderRequest.accountId},{orderRequest.tokenSId},{orderRequest.tokenBId}," +
        //        $"'{orderRequest.amountS}','{orderRequest.amountB}',{orderRequest.allOrNone.ToString().ToLower()},{orderRequest.validSince}," +
        //        $"{orderRequest.validUntil},{orderRequest.maxFeeBips},{orderRequest.buy.ToString().ToLower()},{orderRequest.label}]";

        //    JavascriptResponse jsResponse = await this._chromeBrowser.EvaluateScriptAsync($"createHash({args})");
        //    var a = jsResponse.Result;
        //    strHash = a.ToString();

        //    return strHash;
        //}

        ///// <summary>
        ///// 创建order的Signature
        ///// </summary>
        ///// <param name="secretKey"></param>
        ///// <param name="orderHash"></param>
        ///// <returns></returns>
        //public async Task<SignatureModel> GetSign(string secretKey, string orderHash)
        //{
        //    SignatureModel sign = new SignatureModel();

        //    JavascriptResponse jsResponse = await this._chromeBrowser.EvaluateScriptAsync($"sign('{secretKey}', '{orderHash}')");
        //    var a = jsResponse.Result;
        //    sign = JsonConvert.DeserializeObject<SignatureModel>(a.ToString());
        //    //foreach (var item in (IDictionary<string, object>)obj)
        //    //{
        //    //    if (item.Key == "Rx")
        //    //        sign.Rx = item.Value.ToString();
        //    //    else if (item.Key == "Ry")
        //    //        sign.Ry = item.Value.ToString();
        //    //    else
        //    //        sign.s = item.Value.ToString();
        //    //}

        //    return sign;
        //}
        #endregion

        /// <summary>
        /// 创建order的Hash
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task<string> GetHashAsync(string args)
        {
            JavascriptResponse jsResponse = await this._chromeBrowser.EvaluateScriptAsync($"createHash({args})");
            var a = jsResponse.Result;
            string strHash = a.ToString();

            return strHash;
        }


        /// <summary>
        /// 创建order的Signature
        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="orderHash"></param>
        /// <returns></returns>
        private async Task<string> GetSignAsync(string secretKey, string orderHash)
        {
            JavascriptResponse jsResponse = await this._chromeBrowser.EvaluateScriptAsync($"sign('{secretKey}', '{orderHash}')");
            var a = jsResponse.Result;
            string strSign = a.ToString();
            return strSign;
        }

        #region 浏览器相关
        /// <summary>
        /// 浏览器初始化后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChromeBrowser_BrowserInitialized(object sender, EventArgs e)
        {
            this._logger.Info($"Cef ChromeBrowser BrowserInitialized！The Url Is {this._signUrl}");
            ChromiumWebBrowser browser = sender as ChromiumWebBrowser;
            browser.Load(this._signUrl);
        }

        /// <summary>
        /// 页面主Frame加载完成后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowserFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            this._logger.Info($"Cef Browser FrameLoadEnd！");
            ChromiumWebBrowser browser = sender as ChromiumWebBrowser;
            if (e.Frame.IsMain)
            {
                this._logger.Info($"Cef Browser Main Frame Loaded！");
                //HashTest();
                //SignHashTest();
            }
            else
            {
                Console.WriteLine("Cef Browser Frame Is Not Loaded End!");
            }
        }
        #endregion

        /// <summary>
        /// Will attempt to load missing assembly from either x86 or x64 subdir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = PathCombine.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }

        #region ISign
        /// <summary>
        /// 获取Hash
        /// </summary>
        /// <param name="args">参与生成hash的参数</param>
        /// <returns></returns>
        //public string GetHash(string args)
        //{
        //    string hash = this.GetHashAsync(args).Result;
        //    return hash;
        //}

        /// <summary>
        /// 获取Hash
        /// </summary>
        /// <param name="args">参与生成hash的参数列表</param>
        /// <returns></returns>
        public string GetHash(List<Object> args)
        {
            string strHash = string.Empty;
            if (args == null || args.Count <= 0)
                return strHash;

            string argsStr = "[";
            args.ForEach(o =>
            {
                // int  long  string bool
                if(o is System.String)
                {
                    argsStr += "'"+o + "',";
                }
                else if(o is int || o is long)
                {
                    argsStr += o + ",";
                }
                else if(o is bool)
                {
                    argsStr += o.ToString().ToLower() + ",";
                }
            });
            argsStr = argsStr.TrimEnd(',') + "]";

            strHash = this.GetHashAsync(argsStr).Result;
            return strHash;
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
            string hash = this.GetHash(args);
            string strSign = this.GetSign(secretKey, hash);
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
            string strSign = this.GetSignAsync(secretKey, hash).Result;
            return strSign;
        }

        /// <summary>
        /// 通过arg获取Signature
        /// <para>
        /// 集成了生成hash，不需要再调用GetHash
        /// </para>
        /// </summary>
        /// <param name="secretKey">密钥</param>
        /// <param name="args">参与生成hash的参数</param>
        /// <returns></returns>
        //public string GetSignByArgs(string secretKey, string args)
        //{
        //    string hash = this.GetHash(args);
        //    string strSign = this.GetSign(secretKey, hash);
        //    return strSign;
        //}
        #endregion

        #region 测试
        private async void HashTest()
        {
            DateTime dtStart = DateTime.Now;
            string strHash = string.Empty;
            Console.WriteLine($"strHash:{strHash}");
            for (int i = 0; i < 1; i++)
            {
                string args = $"[2,3333,88,3,2,'5013500','185000000000000000000',1,21342343534,5463463456,50,1,221]";
                JavascriptResponse jsResponse = await this._chromeBrowser.EvaluateScriptAsync($"createHash({args})");
                var a = jsResponse.Result;
                strHash = a.ToString();
            }
            //Console.WriteLine($"strHash:{strHash}");

            DateTime dtEnd = DateTime.Now;
            double s = (dtEnd - dtStart).TotalMilliseconds;
            string str = $"strHash 10000次时间差：{s} ms";
            Console.WriteLine(str);
        }

        private async void SignHashTest()
        {

            DateTime dtStart = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                string args = $"[2,3333,88,3,2,'5013500','185000000000000000000',1,21342343534,5463463456,50,1,221]";
                JavascriptResponse jsResponse = await this._chromeBrowser.EvaluateScriptAsync($"createHash({args}).toString(10)");
                var a = jsResponse.Result;
                string strHash = a.ToString();

                JavascriptResponse jsResponse1 = await this._chromeBrowser.EvaluateScriptAsync($"sign('132131313', '{strHash}')");
                var a1 = jsResponse1.Result;
                string strSign = a1.ToString();
            }
            DateTime dtEnd = DateTime.Now;
            double s = (dtEnd - dtStart).TotalMilliseconds;
            string str = $"Hash+Sign 1000次时间差：{s} ms";
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
