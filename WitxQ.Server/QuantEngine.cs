using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using WitxQ.Common;
using WitxQ.Common.Logger;
using WitxQ.Interface.Spot;

using Microsoft.Extensions.DependencyInjection;
using WitxQ.Server.SysFrame;
using System.Threading;
using System.Threading.Tasks;
using WitxQ.Server.Test;
using System.Collections.Generic;
using WitxQ.Interface.StrategyTA;
using WitxQ.Strategy.TA.GraphRing;
using System.Linq;
using WitxQ.Strategy.TA;
using WitxQ.Model.Markets;
using WitxQ.Interface.Loopring;

//using WitxQ.Exchange.Wedex;
//using WitxQ.Exchange.Wedex.Models;
//using WitxQ.Exchange.Wedex.Operation;
//using WitxQ.Exchange.Wedex.Models.Token;
//using WitxQ.Exchange.Wedex.Models.Market;

using WitxQ.Exchange.Loopring;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Operation;
using WitxQ.Exchange.Loopring.Models.Token;
using WitxQ.Exchange.Loopring.Models.Market;
using WitxQ.Exchange.Loopring.Models.SwapMarket;

namespace WitxQ.Server
{
    /// <summary>
    /// 量化引擎
    /// </summary>
    public class QuantEngine
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
        /// DI中的通过loopring的签名
        /// </summary>
        private readonly ISign _loopringSign;

        /// <summary>
        /// 是否启动
        /// </summary>
        public bool IsStart { get; private set; } = false;


        /// <summary>
        /// 引擎构造
        /// </summary>
        /// <param name="logger">DI中的日志</param>
        public QuantEngine(ServerLogger logger, ExChanges exChanges,ISign loopringSign) 
        {
            this._logger = logger;
            this._exChanges = exChanges;
            this._loopringSign = loopringSign;
            //AppDomain.CurrentDomain.AssemblyResolve += Resolver;
        }


        #region QuantEngine
        //private QuantEngine() { }

        ///// <summary>
        ///// 获取单例QuantEngine
        ///// </summary>
        ///// <returns></returns>
        //public static QuantEngine GetInstance()
        //{
        //    return InnerInstance.instance;
        //}

        //private class InnerInstance
        //{
        //    static InnerInstance() { }
        //    internal static QuantEngine instance = new QuantEngine();
        //}
        #endregion

        /// <summary>
        /// 启动引擎
        /// </summary>
        public void Start()
        {
            //this._logger.Info("adfadsfasdfadsfdasf");

            ExChange exChange = this._exChanges.AllExChanges[0];

            // 调用的日志接口
            ILogger ilogger = ServiceLocator.Instance.GetService<ILogger>();
            //ExWedex.SetLogger(ilogger);
            //ExWedex.SetWsClient(exChange.WSUrl);
            //ExWedex.SetApiUrl(exChange.HttpUrl);
            ////ExWedex.SetCefSign(this._exChanges.LoopringSignUrl);
            //ExWedex.SetCefSign(this._cefSign);
            //ExWedex.Init();

            ExLoopring.SetLogger(ilogger);
            //ExLoopring.SetWsClient(exChange.WSUrl);
            ExLoopring.SetApiUrl(exChange.HttpUrl);
            ExLoopring.SetCefSign(this._loopringSign);
            ExLoopring.Init();



            #region test order 

            //Account acc = exChange.Users[0].Account;
            //AccountModel accountTest = new AccountModel();
            //accountTest.AccountId = acc.AccountId;
            //accountTest.Address = acc.Address;
            //accountTest.ApiKey = acc.ApiKey;
            //accountTest.PublicKeyX = acc.PublicKeyX;
            //accountTest.PublicKeyY = acc.PublicKeyY;
            //accountTest.SecretKey = acc.SecretKey;

            ////OrderOperation order = new OrderOperation(accountTest);
            //SwapOrderOperation swapOrder = new SwapOrderOperation(accountTest);
            //string err = string.Empty;
            //string strOrderNum = string.Empty;
            ////bool succ = order.OrderBuy("LRC-USDT", "0.2", "50", "USDT", "LRC", ref strOrderNum, ref err);
            //bool succ = swapOrder.SwapOrderBuy("AMM-LRC-ETH", "0.000333059293", "50", "ETH", "LRC", ref strOrderNum, ref err);
            ////bool succ = swapOrder.SwapOrderSell("AMM-LRC-ETH", "0.44", "50", "LRC", "USDT", ref strOrderNum, ref err);

            ////List<bool> blns = new List<bool>();
            ////string strDelErr = string.Empty;
            ////order.OrderDelBatch(new List<string>() { strOrderNum }, ref blns, ref strDelErr);

            ////((NodejsLoopringSign)ExLoopring.CEF_LOOPRING_SIGN).SignHashTest();

            ////((ApiLoopringSign)ExLoopring.CEF_LOOPRING_SIGN).SignHashTest();
            //return;
            #endregion


            #region 抢ido模式

            Account acc = exChange.Users[0].Account;
            AccountModel accountTest = new AccountModel();
            accountTest.AccountId = acc.AccountId;
            accountTest.Address = acc.Address;
            accountTest.ApiKey = acc.ApiKey;
            accountTest.PublicKeyX = acc.PublicKeyX;
            accountTest.PublicKeyY = acc.PublicKeyY;
            accountTest.SecretKey = acc.SecretKey;


            string strTargetPair = "AMM-DPR-ETH";
            //string strTargetPair = "AMM-LRC-ETH";
            SwapOrderOperation swapOrder = new SwapOrderOperation(accountTest);
            while (true)
            {
                Thread.Sleep(300);
                try
                {
                    // 1、资金（不要）
                    Console.WriteLine($"===========开始时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}===============：");

                    // 2、swapmarket
                    Dictionary<string, AmmMarketModel> swapMarket = ExLoopring.SWAP_MARKETS;

                    if (!swapMarket.ContainsKey(strTargetPair))
                    {
                        // 获取loopring.io的swap市场信息
                        List<AmmMarketModel> ammMarkets = new SwapMarketOperation().GetAmmMarketsByApi();
                        ExLoopring.SWAP_MARKETS = ammMarkets.ToDictionary(ammMarket => ammMarket.name);
                    }
                    //Console.WriteLine($"当前为：{strTargetPair}");
                    swapMarket = ExLoopring.SWAP_MARKETS;
                    if (!swapMarket.ContainsKey(strTargetPair))
                    {
                        Console.WriteLine($"无对应amm pool：{strTargetPair}，进行下一次循环！！！");
                    }
                    else
                    {


                        Console.WriteLine($"当前为：{strTargetPair}");
                        //Console.WriteLine($"市场交易对总共：（{(swapMarket?.Count ?? 0)}），当前为：{strTargetPair}");

                        // 3、交换
                        string err = string.Empty;
                        string strOrderNum = string.Empty;
                        bool succ = swapOrder.SwapOrderBuy(strTargetPair, "0.0000477", "84000", "ETH", "DPR", ref strOrderNum, ref err);
                        //bool succ = swapOrder.SwapOrderBuy(strTargetPair, "0.0003", "200", "ETH", "LRC", ref strOrderNum, ref err);

                        if (succ)
                        {
                            Console.WriteLine("交易成功！！");
                        }
                        else
                        {
                            Console.WriteLine($"交易失败！！原因：{err}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"出现异常：{ex}");
                }
                Console.WriteLine($"===========结束时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}===============：");
            }

            return;



            #endregion


            #region  Wedex.io
            //// Wedex.io的处理
            //List<User> users = exChange.Users;
            //users.ForEach(user =>
            //{
            //    AccountModel account = new AccountModel();
            //    account.AccountId = user.Account.AccountId;
            //    account.Address = user.Account.Address;
            //    account.ApiKey = user.Account.ApiKey;
            //    account.PublicKeyX = user.Account.PublicKeyX;
            //    account.PublicKeyY = user.Account.PublicKeyY;
            //    account.SecretKey = user.Account.SecretKey;

            //    ExAccountWedex exAccount = new ExAccountWedex(account);

            //    // 添加账户信息到全局ExLoopring
            //    if (!ExWedex.EXCHANGE_ACCOUNTS.ContainsKey(account.AccountId))
            //        ExWedex.EXCHANGE_ACCOUNTS.Add(account.AccountId, exAccount);

            //    // 获取账户余额信息
            //    IBalances ibalance = exAccount.Balance;
            //    foreach (var b in ibalance.GetBalances())
            //    {
            //        Console.WriteLine($"{b.TokenSymbol},AvailableAmount:{b.AvailableAmount},LockedAmount:{b.LockedAmount}");
            //    }

            //    WitxQ.Server.Test.TAStrategy strategy = user.Strategies.FirstOrDefault(s => s.Name.Equals("TriangularArbitrage"));
            //    TAStrategyConfig strategyConfig = strategy.Config;

            //    // 添加策略
            //    // 1、先获取环路
            //    List<string> pairs = ExWedex.MARKETS.Values.Select<MarketInfoModel, string>(market => market.market).ToList();
            //    IUndirectedGraph undirectedGraph = new UDGraphRing();
            //    var allRings = undirectedGraph.GetUDGraphRings(pairs, strategyConfig.TargetToken, strategyConfig.Deep);

            //    Dictionary<string, List<Tuple<int, string, bool>>> rings = new Dictionary<string, List<Tuple<int, string, bool>>>();
            //    foreach (var rv in allRings.Values)
            //    {
            //        foreach (string ringName in rv.Keys)
            //        {
            //            if (ringName.Contains("ETH") && ringName.Contains("DKEY") && ringName.Contains("USDT"))
            //            {
            //                rings.Add(ringName, rv[ringName]);
            //            }
            //        }
            //    }

            //    foreach (var ring in rings)
            //    {
            //        TriangularArbitrageParam param = new TriangularArbitrageParam();
            //        param.ExchangeName = exChange.Name;
            //        param.LoggerPath = "Logs/TA";
            //        param.TargetToken = strategyConfig.TargetToken;
            //        param.Deep = strategyConfig.Deep;
            //        param.TARingName = ring.Key;
            //        param.MinProfitRatio = strategyConfig.MinProfitRatio;
            //        param.MinAmountTran = strategyConfig.MinAmountTran;
            //        param.IsMinAmountTran = strategyConfig.IsMinAmountTran;
            //        param.AmountTranPercentage = strategyConfig.AmountTranPercentage;
            //        param.SlidingPoint = strategyConfig.SlidingPoint;
            //        param.IsTranAuto = strategyConfig.IsTranAuto;

            //        List<TriangularArbitragePairsParam> pairsParams = new List<TriangularArbitragePairsParam>();
            //        List<string> ringPairs = new List<string>();   // 这个环路下的交易对列表
            //        ring.Value.ForEach(t =>
            //        {
            //            TriangularArbitragePairsParam tapp = new TriangularArbitragePairsParam();
            //            tapp.SeqNumber = t.Item1;

            //            MarketInfoModel marketInfo = ExWedex.MARKETS[t.Item2];
            //            PairModel pairModel = new PairModel();
            //            pairModel.PairSymbol = marketInfo.market;
            //            TokenModel quoteToken = ExWedex.TOKENS[marketInfo.quoteTokenId];
            //            TokenModel baseToken = ExWedex.TOKENS[marketInfo.baseTokenId];
            //            pairModel.QuoteToken = quoteToken.symbol;
            //            pairModel.BaseToken = baseToken.symbol;
            //            pairModel.MinAmountQuoteToken = quoteToken.Order_MinAmount;
            //            pairModel.MinAmountBaseToken = baseToken.Order_MinAmount;
            //            pairModel.Fee = exChange.Fee;
            //            pairModel.PrecisionForPrice = marketInfo.precisionForPrice;

            //            tapp.Pair = pairModel;
            //            tapp.OrderSide = t.Item3;

            //            pairsParams.Add(tapp);

            //            ringPairs.Add(marketInfo.market);
            //        });
            //        param.TAPairs = pairsParams;

            //        // 添加深度信息
            //        ExWedex.OPERATION_DEPTH.SetMarkPair(ringPairs);

            //        ITriangularArbitrage triangular = new TriangularArbitrageFactory().Create(param);
            //        triangular.SetTATask(new OrderOperation(account), ExWedex.OPERATION_DEPTH, ibalance);
            //        triangular.Run();
            //    }
            //});
            #endregion


            #region  loopring.io
            // loopring的处理
            List<User> users = exChange.Users;
            users.ForEach(user =>
            {
                AccountModel account = new AccountModel();
                account.AccountId = user.Account.AccountId;
                account.Address = user.Account.Address;
                account.ApiKey = user.Account.ApiKey;
                account.PublicKeyX = user.Account.PublicKeyX;
                account.PublicKeyY = user.Account.PublicKeyY;
                account.SecretKey = user.Account.SecretKey;

                ExAccountLoopring exAccount = new ExAccountLoopring(account);

                // 添加账户信息到全局ExLoopring
                if (!ExLoopring.EXCHANGE_ACCOUNTS.ContainsKey(account.AccountId))
                    ExLoopring.EXCHANGE_ACCOUNTS.Add(account.AccountId, exAccount);
                
                // 设置账号WS订阅
                ExLoopring.SetWsClient(account,exChange.WSUrl);

                // 获取账户余额信息
                IBalances ibalance = exAccount.Balance;
                foreach (var b in ibalance.GetBalances())
                {
                    //Console.WriteLine($"{b.TokenSymbol},AvailableAmount:{b.AvailableAmount},LockedAmount:{b.LockedAmount}");
                }
                // 市场信息接口
                IMarket imarket = new MarketInfoOperation();


                #region 1 添加三角套利策略

                //WitxQ.Server.Test.TAStrategy strategy = user.TAStrategies.FirstOrDefault(s => s.Name.Equals("TriangularArbitrage"));
                //TAStrategyConfig strategyConfig = strategy.Config;

                //// 1.1 先获取环路
                //List<string> pairs = ExLoopring.MARKETS.Values.Select<MarketInfoModel, string>(market => market.market).ToList();
                //IUndirectedGraph undirectedGraph = new UDGraphRing();
                //var allRings = undirectedGraph.GetUDGraphRings(pairs, strategyConfig.TargetToken, strategyConfig.Deep);

                //Dictionary<string, List<Tuple<int, string, bool>>> rings = new Dictionary<string, List<Tuple<int, string, bool>>>();
                //foreach (var rv in allRings.Values)
                //{
                //    foreach (string ringName in rv.Keys)
                //    {
                //        if (ringName.Contains("ETH") && ringName.Contains("LRC") && ringName.Contains("USDT"))
                //        {
                //            rings.Add(ringName, rv[ringName]);
                //        }
                //    }
                //}

                //foreach (var ring in rings)
                //{
                //    TriangularArbitrageParam param = new TriangularArbitrageParam();
                //    param.ExchangeName = exChange.Name;
                //    param.LoggerPath = "Logs/TA";
                //    param.TargetToken = strategyConfig.TargetToken;
                //    param.Deep = strategyConfig.Deep;
                //    param.TARingName = ring.Key;
                //    param.MinProfitRatio = strategyConfig.MinProfitRatio;
                //    param.MinAmountTran = strategyConfig.MinAmountTran;
                //    param.IsMinAmountTran = strategyConfig.IsMinAmountTran;
                //    param.AmountTranPercentage = strategyConfig.AmountTranPercentage;
                //    param.SlidingPoint = strategyConfig.SlidingPoint;
                //    param.IsTranAuto = strategyConfig.IsTranAuto;
                //    param.IntervalOrderCall = strategyConfig.IntervalOrderCall;

                //    List<TriangularArbitragePairsParam> pairsParams = new List<TriangularArbitragePairsParam>();
                //    List<string> ringPairs = new List<string>();   // 这个环路下的交易对列表
                //    ring.Value.ForEach(t =>
                //    {
                //        TriangularArbitragePairsParam tapp = new TriangularArbitragePairsParam();
                //        tapp.SeqNumber = t.Item1;

                //        MarketInfoModel marketInfo = ExLoopring.MARKETS[t.Item2];
                //        PairModel pairModel = new PairModel();
                //        pairModel.PairSymbol = marketInfo.market;
                //        TokenModel quoteToken = ExLoopring.TOKENS[marketInfo.quoteTokenId];
                //        TokenModel baseToken = ExLoopring.TOKENS[marketInfo.baseTokenId];
                //        pairModel.QuoteToken = quoteToken.symbol;
                //        pairModel.BaseToken = baseToken.symbol;
                //        pairModel.MinAmountQuoteToken = quoteToken.Order_MinAmount;
                //        pairModel.MinAmountBaseToken = baseToken.Order_MinAmount;
                //        //pairModel.Fee = exChange.Fee;

                //        tapp.Pair = pairModel;
                //        tapp.OrderSide = t.Item3;

                //        pairsParams.Add(tapp);

                //        ringPairs.Add(marketInfo.market);
                //    });
                //    param.TAPairs = pairsParams;

                //    // 添加深度信息
                //    ExLoopring.OPERATION_DEPTH.SetMarkPair(ringPairs);

                //    ITriangularArbitrage triangular = new TriangularArbitrageFactory().Create(param);
                //    triangular.SetTATask(new OrderOperation(account), ExLoopring.OPERATION_DEPTH, ibalance);
                //    triangular.Run();
                //}

                #endregion

                #region 2 添加做市商策略（已注释）

                // 2 添加做市商策略
                //WitxQ.Server.Test.MMStrategy mmstrategy = user.MMStrategies.FirstOrDefault(s => s.Name.Equals("MarketMerchant"));
                //MMStrategyConfig mmstrategyConfig = mmstrategy.Config;

                //IMarketMerchant mm = new MarketMerchantFactory().Create(config=>
                //{
                //    config.LoggerPath = "Logs/MM";
                //    config.MarketPair = mmstrategyConfig.MarketPair;
                //    config.MakerDirection = mmstrategyConfig.MakerDirection;

                //    List<Tuple<int, decimal>> bids = null;
                //    if(mmstrategyConfig.MarkerBids!=null && mmstrategyConfig.MarkerBids.Count>0)
                //    {
                //        bids = new List<Tuple<int, decimal>>();
                //        mmstrategyConfig.MarkerBids.ForEach(m =>
                //        {
                //            Tuple<int, decimal> tuple = new Tuple<int, decimal>(m.Level, m.Amount);
                //            bids.Add(tuple);
                //        });
                //    }
                //    config.MarkerBids = bids;

                //    List<Tuple<int, decimal>> asks = null;
                //    if (mmstrategyConfig.MarkerAsks != null && mmstrategyConfig.MarkerAsks.Count > 0)
                //    {
                //        asks = new List<Tuple<int, decimal>>();
                //        mmstrategyConfig.MarkerAsks.ForEach(m =>
                //        {
                //            Tuple<int, decimal> tuple = new Tuple<int, decimal>(m.Level, m.Amount);
                //            asks.Add(tuple);
                //        });
                //    }
                //    config.MarkerAsks = asks;
                //});
                //// 添加深度信息
                //ExLoopring.OPERATION_DEPTH.SetMarkPair(new List<string>() { mmstrategyConfig.MarketPair });
                //mm.SetMMTask(new OrderOperation(account), ExLoopring.OPERATION_DEPTH, ibalance, imarket);
                //mm.Run();

                #endregion


                #region 3 添加Swap与订单薄套利策略

                //List<SwapStrategyConfig> swapStrategyConfigs = user.SWAPStrategies;
                //foreach (var swapStrategyConfig in swapStrategyConfigs)
                //{
                //    ISwap swap = new SwapFactory().Create(config =>
                //    {
                //        config.LoggerPath = "Logs/Swap";
                //        config.SwapMarketPair = swapStrategyConfig.SwapMarketPair;
                //        config.MarketPair = swapStrategyConfig.MarketPair;
                //        config.TargetToken = swapStrategyConfig.TargetToken;
                //        config.TradeAmount = swapStrategyConfig.TradeAmount;
                //        config.SlipLimit = swapStrategyConfig.SlipLimit;
                //        config.SwapFee = swapStrategyConfig.SwapFee;
                //        config.OrderBookFee = swapStrategyConfig.OrderBookFee;
                //        config.MinProfitRatio = swapStrategyConfig.MinProfitRatio;
                //        config.IsTranAuto = swapStrategyConfig.IsTranAuto;
                //        config.IntervalOrderCall = swapStrategyConfig.IntervalOrderCall;
                //    });
                //    // 设置订单薄指定的市场深度信息回调 
                //    ExLoopring.OPERATION_DEPTH.SetMarkPair(new List<string>() { swapStrategyConfig.MarketPair });
                //    ExLoopring.OPERATION_SWAP_SNAPSHOT.SetAmmPoolNames(new List<string>() { swapStrategyConfig.SwapMarketPair });
                //    //swap.SetSwapTask(new OrderOperation(account), ExLoopring.OPERATION_DEPTH, ibalance, imarket, new SwapSnapshotOperation(account), new SwapOrderOperation(account));
                //    swap.SetSwapTask(new OrderOperation(account), ExLoopring.OPERATION_DEPTH, ibalance, imarket, ExLoopring.OPERATION_SWAP_SNAPSHOT, new SwapOrderOperation(account));
                //    swap.Run();
                //}


                #endregion

                #region 4 添加Swap动态网格策略
                //List<SwapDGConfig> swapDGConfigs = user.SwapDGStrategies;
                //foreach (var swapDGConfig in swapDGConfigs)
                //{
                //    SwapDynamicGridConfig config = new SwapDynamicGridConfig();
                //    config.ExchangeName = "Loopring";
                //    config.LoggerPath = "Logs/SwapDG";
                //    config.PairSymbol = swapDGConfig.PairSymbol;
                //    config.BaseToken = swapDGConfig.BaseToken;
                //    config.InitialBaseTokenAmount = swapDGConfig.InitialBaseTokenAmount;
                //    config.QuoteToken = swapDGConfig.QuoteToken;
                //    config.InitialQuoteTokenAmount = swapDGConfig.InitialQuoteTokenAmount;
                //    config.BenchmarkPrice = swapDGConfig.BenchmarkPrice;
                //    config.StepBuyType = swapDGConfig.StepBuyType;
                //    config.StepBuy = swapDGConfig.StepBuy;
                //    config.StepBuyAmount = swapDGConfig.StepBuyAmount;
                //    config.StepSellType = swapDGConfig.StepSellType;
                //    config.StepSell = swapDGConfig.StepSell;
                //    config.StepSellAmount = swapDGConfig.StepSellAmount;
                //    config.IsRebalance = swapDGConfig.IsRebalance;
                //    config.GainToken = swapDGConfig.GainToken;
                //    config.GainTokenAmount = swapDGConfig.GainTokenAmount;
                //    config.SlipLimit = swapDGConfig.SlipLimit;
                //    config.SwapFee = swapDGConfig.SwapFee;

                //    ISwapDynamicGrid swapDG = new SwapDynamicGridFactory().Create(config);

                //    // 设置订单薄指定的市场深度信息回调 
                //    //ExLoopring.OPERATION_DEPTH.SetMarkPair(new List<string>() { swapStrategyConfig.MarketPair });
                //    swapDG.SetDGTask(new SwapOrderOperation(account), new SwapSnapshotOperation(account), ibalance);
                //    swapDG.Run();
                //}
                #endregion

            });
            #endregion

            this.IsStart = true;

            Console.WriteLine("Hello World!");
            //Console.ReadKey();
        }

        /// <summary>
        /// 停止引擎
        /// </summary>
        public void Stop()
        {
            // TODO Engine.Stop() 需要将开启的线程都先关闭掉，才结束
            if (!this.IsStart)
                return;
            try
            {
                // 1、释放cef资源
                CefSharp.Cef.Shutdown();
            }
            catch (Exception ex)
            {
                this._logger.Error($"Engine stop failed.errmsg:{ex.Message},errStack:{ex.StackTrace}");
            }

            this.IsStart = false;
            //SysLogger.Info("Engine stopped.");

        }


        /// <summary>
        /// Will attempt to load missing assembly from either x86 or x64 subdir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        //private Assembly Resolver(object sender, ResolveEventArgs args)
        //{
        //    if (args.Name.StartsWith("CefSharp"))
        //    {
        //        string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
        //        string archSpecificPath = PathCombine.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
        //                                               Environment.Is64BitProcess ? "x64" : "x86",
        //                                               assemblyName);

        //        return File.Exists(archSpecificPath)
        //                   ? Assembly.LoadFile(archSpecificPath)
        //                   : null;
        //    }

        //    return null;
        //}

    }
}
