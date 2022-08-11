using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WitxQ.Common.Logger;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Models.Market;
using WitxQ.Exchange.Loopring.Models.SwapMarket;
using WitxQ.Exchange.Loopring.Models.Token;
using WitxQ.Exchange.Loopring.Models.WS;
using WitxQ.Exchange.Loopring.Models.WS.Topic;
using WitxQ.Exchange.Loopring.Operation;
using WitxQ.Exchange.Loopring.Sys;
using WitxQ.Exchange.Loopring.Tools;
using WitxQ.Interface.Loopring;

namespace WitxQ.Exchange.Loopring
{
    /// <summary>
    /// loopring的交易所，只有一个实例（全局信息）
    /// <para>
    /// 此实例下，有多个ExAccountLoopring（账户交易所实体）
    /// </para>
    /// </summary>
    public static class ExLoopring
    {
        /// <summary>
        /// 此交易所下的 深度信息及操作
        /// <para>
        /// 所有账户通用，属于全局信息
        /// </para>
        /// </summary>
        public static readonly DepthOperation OPERATION_DEPTH = new DepthOperation();

        /// <summary>
        /// websocket的客户端类
        /// </summary>
        public static readonly WSClient WSCLIENT = WSClient.GetInstance();
        /// <summary>
        /// ws连接的使用的key
        /// </summary>
        public static string WS_API_KEY = string.Empty;

        /// <summary>
        /// 此交易所下的所有账户ex实体
        /// <para>
        /// key:accountid,value:ExAccountLoopring对象
        /// </para>
        /// </summary>
        public static Dictionary<int, ExAccountLoopring> EXCHANGE_ACCOUNTS = new Dictionary<int, ExAccountLoopring>();

        /// <summary>
        /// 此交易所下的所有Token信息字典
        /// <para>
        /// key:tokenId,value:token
        /// </para>
        /// </summary>
        public static Dictionary<int,TokenModel> TOKENS = new Dictionary<int, TokenModel>();

        /// <summary>
        /// 此交易所下的所有Market信息字典
        /// <para>
        /// key:market交易对Name(loopring自有的格式，即全部大写，且中间有连字符‘-’),value:MarketInfoModel实体
        /// </para>
        /// </summary>
        public static Dictionary<string, MarketInfoModel> MARKETS = new Dictionary<string, MarketInfoModel>();


        /// <summary>
        /// 此交易所下的 市场信息及操作
        /// <para>
        /// 所有账户通用，属于全局信息
        /// </para>
        /// </summary>
        public static readonly SwapMarketOperation OPERATION_SWAP_MARKET = new SwapMarketOperation();

        /// <summary>
        /// 此交易所下的 获取Swap闪兑的价格快照
        /// <para>
        /// 所有账户通用，属于全局信息
        /// </para>
        /// </summary>
        public static readonly SwapSnapshotOperation OPERATION_SWAP_SNAPSHOT = new SwapSnapshotOperation();

        /// <summary>
        /// 此交易所下的所有AMM的Market信息字典
        /// <para>
        /// key:AMM market交易对Name(loopring自有的格式，即全部大写，且中间有连字符‘-’),value:MarketInfoModel实体
        /// </para>
        /// </summary>
        public static Dictionary<string, AmmMarketModel> SWAP_MARKETS = new Dictionary<string, AmmMarketModel>();
        //public static readonly object LOCK_SWAP_MARKETS = new object();


        /// <summary>
        /// 订阅管理
        /// </summary>
        public static SubscribeManager SUBSCRIBE_MANAGER = SubscribeManager.GetInstance();

        /// <summary>
        /// 此项目的日志类
        /// </summary>
        public static LoopringLogger LOGGER;

        /// <summary>
        /// loopring的hash和签名
        /// </summary>
        public static ISign CEF_LOOPRING_SIGN;

        //public static CefLoopringSign CEF_LOOPRING_SIGN = CefLoopringSign.GetInstance();

        /// <summary>
        /// BaseUrl，访问的url
        /// </summary>
        public static string API_URL { get; private set; }

        /// <summary>
        /// 1、设置项目日志器（必须）
        /// </summary>
        /// <param name="logger">上层调用传递的 日志接口</param>
        public static void SetLogger(ILogger logger)
        {
            ExLoopring.LOGGER = new LoopringLogger(logger);
        }


        /// <summary>
        /// 2、设置WebSocket Client
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="serverPath">ws的url</param>
        public static void SetWsClient(AccountModel account, string serverPath)
        {
            ExLoopring.WSCLIENT.SetServerPath(account,serverPath);
            // 添加 websocket 推送事件信息
            ExLoopring.WSCLIENT.MessageReceivedHandler += new WSMessageDispatcher().Dispatcher;
            // 添加 websocket open 事件信息
            ExLoopring.WSCLIENT.OpenedHandler += ExLoopring.SUBSCRIBE_MANAGER.SendAllSubscribes;
            ExLoopring.WSCLIENT.Start();
        }

        /// <summary>
        /// 3、设置API访问的url
        /// </summary>
        /// <param name="apiUrl"></param>
        public static void SetApiUrl(string apiUrl)
        {
            ExLoopring.API_URL = apiUrl;
        }

        /// <summary>
        /// 4、设置hash和sign的URL
        /// </summary>
        /// <param name="signUrl"></param>
        public static void SetCefSign(ISign cefSign)
        {
            ExLoopring.CEF_LOOPRING_SIGN = cefSign;
        }

        /// <summary>
        /// 4、设置hash和sign的URL
        /// </summary>
        /// <param name="signUrl"></param>
        //public static void SetCefSign(string signUrl)
        //{
        //    ExLoopring.CEF_LOOPRING_SIGN.SetSignUrl(signUrl);
        //    ExLoopring.CEF_LOOPRING_SIGN.LoadCef();
        //}

        /// <summary>
        /// 5、初始化ExLoopring的必要数据
        /// </summary>
        public static void Init()
        {
            // 获取loopring.io的通证信息
            List<TokenModel> tokens = new TokenOperation().GetTokensByApi();
            ExLoopring.TOKENS = tokens.ToDictionary(token => token.tokenId);

            // 获取loopring.io的订单薄市场信息
            List<MarketInfoModel> markets = new MarketOperation().GetMarketsByApi();
            ExLoopring.MARKETS = markets.ToDictionary(market => market.market);

            // 获取loopring.io的swap市场信息
            List<AmmMarketModel> ammMarkets = new SwapMarketOperation().GetAmmMarketsByApi();
            ExLoopring.SWAP_MARKETS = ammMarkets.ToDictionary(ammMarket => ammMarket.name);
        }

    }
}
