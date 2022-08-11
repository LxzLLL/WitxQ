using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using WitxQ.Common;
using WitxQ.Common.Locks;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Models.SwapMarket;
using WitxQ.Exchange.Loopring.Models.SwapSnapshot;
using WitxQ.Exchange.Loopring.Models.WS;
using WitxQ.Exchange.Loopring.Models.WS.Topic;
using WitxQ.Exchange.Loopring.Tools;
using WitxQ.Interface.Spot;
using WitxQ.Model.Markets;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace WitxQ.Exchange.Loopring.Operation
{
    /// <summary>
    /// 获取Swap闪兑的价格快照
    /// <para>
    /// 内部已做定时（每隔300ms）获取
    /// </para>
    /// </summary>
    public class SwapSnapshotOperation : BaseOperation, ISwapSnapshot
    {

        /// <summary>
        /// 获取swap的snapshot的Path
        /// </summary>
        private readonly string _ammSnapshotPath = "/api/v2/amm/snapshot";

        /// <summary>
        /// 获取swap的snapshots的Path
        /// </summary>
        private readonly string _ammSnapshotsPath = "/api/v2/amm/snapshots";

        /// <summary>
        /// 所有闪兑交易对的快照信息字典
        /// key:poolAddress，value：AmmSnapshotModel
        /// </summary>
        //List<AmmSnapshotModel> _ammSnapshots = new List<AmmSnapshotModel>();
        Dictionary<string, AmmSnapshotModel> _ammSnapshots = new Dictionary<string, AmmSnapshotModel>();
        /// <summary>
        /// 快照列表的读写锁
        /// </summary>
        private readonly UsingReaderWriterLock<object> _lockSnapshots = new UsingReaderWriterLock<object>();

        /// <summary>
        /// http请求客户端
        /// </summary>
        private readonly HttpRequestClient2 _httpClient;

        /// <summary>
        /// 查询用户交易所余额 的构造
        /// </summary>
        /// <param name="account">账号信息</param>
        //public SwapSnapshotOperation(AccountModel account) : base("")
        //{
        //    this.Account = account;
        //    this.XApiKey = this.Account.ApiKey;
        //    this._httpClient = new HttpRequestClient2(this.DomainUrl);  // 使用 基础Url初始化，由各个操作设置path路径

        //    this.Init();
        //}

        /// <summary>
        /// 查询用户交易所余额 的构造
        /// </summary>
        /// <param name="account">账号信息</param>
        public SwapSnapshotOperation() : base("")
        {
            this._httpClient = new HttpRequestClient2(this.DomainUrl);  // 使用 基础Url初始化，由各个操作设置path路径
            //this.Init();
        }

        /// <summary>
        /// 需要初始化的内容
        /// </summary>
        private void Init()
        {
            this.GetAmmSnapshotsByApi();
        }

        #region ISwapSnapshot

        /// <summary>
        /// 获取Amm闪兑交易深度事件
        /// </summary>
        public event EventHandler<List<SwapSnapshotModel>> GetSwapDepthEvent;


        /// <summary>
        /// 直接通过Api 通过swap交易对名称获取snapshot
        /// <para>
        /// 每次调用都会请求一次Api
        /// </para>
        /// </summary>
        /// <param name="swapMarketPairName">交易对，全部为中间“-”连字符的大写形式，例如AMM-LRC-USDT</param>
        /// <returns></returns>
        private SwapSnapshotModel GetSnapshotByPairWithApi(string swapMarketPairName)
        {
            SwapSnapshotModel swapSnapshot = null;
            SwapMarketPairModel swapMarket = ExLoopring.OPERATION_SWAP_MARKET.GetSwapMarketPairModel(swapMarketPairName);

            AmmSnapshotModel ammSnapshot = this.GetAmmSnapshotByApi(swapMarket.Address);
            if(ammSnapshot!=null)
            {
                swapSnapshot = new SwapSnapshotModel();
                swapSnapshot.MarketPair = swapMarket;
                swapSnapshot.PoolTokenAmount = ammSnapshot.PoolTokenAmount;
                swapSnapshot.PoolBaseTokenAmount = ammSnapshot.PoolBaseTokenAmount;
                swapSnapshot.PoolQuoteTokenAmount = ammSnapshot.PoolQuoteTokenAmount;
                swapSnapshot.Price = ammSnapshot.Price;
                swapSnapshot.ReversePrice = ammSnapshot.ReversePrice;
            }
            return swapSnapshot;
        }

        /// <summary>
        /// 通过swap交易对名称获取snapshot
        /// </summary>
        /// <param name="swapMarketPairName">交易对，全部为中间“-”连字符的大写形式，例如AMM-LRC-USDT</param>
        /// <returns></returns>
        public SwapSnapshotModel GetSnapshotByPair(string swapMarketPairName)
        {
            SwapSnapshotModel swapSnapshot = null;
            SwapMarketPairModel swapMarket = ExLoopring.OPERATION_SWAP_MARKET.GetSwapMarketPairModel(swapMarketPairName);

            using (this._lockSnapshots.Read())
            {
                AmmSnapshotModel ammSnapshot = this._ammSnapshots[swapMarket.Address];
                if (ammSnapshot != null)
                {
                    swapSnapshot = new SwapSnapshotModel();
                    swapSnapshot.MarketPair = swapMarket;
                    swapSnapshot.PoolTokenAmount = ammSnapshot.PoolTokenAmount;
                    swapSnapshot.PoolBaseTokenAmount = ammSnapshot.PoolBaseTokenAmount;
                    swapSnapshot.PoolQuoteTokenAmount = ammSnapshot.PoolQuoteTokenAmount;
                    swapSnapshot.Price = ammSnapshot.Price;
                    swapSnapshot.ReversePrice = ammSnapshot.ReversePrice;
                }
            }
            return swapSnapshot;
        }

        /// <summary>
        /// 获取swap的snapshots
        /// </summary>
        /// <param name="pairNames">交易对名称列表，如果为null，则获取全部数据，默认为null</param>
        /// <returns></returns>
        public List<SwapSnapshotModel> GetSnapshotsByPairs(List<string> pairNames = null)
        {
            return this.GetSnapshotsByPairs(this._ammSnapshots.Values.ToList(), pairNames);
        }

        #endregion



        /// <summary>
        /// 获取Amm的Snapshot数据
        /// </summary>
        /// <returns></returns>
        private AmmSnapshotModel GetAmmSnapshotByApi(string poolAddress)
        {
            AmmSnapshotModel ammSnapshot = null;
            try
            {
                AmmSnapshotResponseModel ammSnapshotResponse = this._httpClient.Get<AmmSnapshotResponseModel>(new { poolAddress = poolAddress }, null, this._ammSnapshotPath);

                // 成功
                if (ammSnapshotResponse.resultInfo.code == 0)
                {
                    ammSnapshot = ammSnapshotResponse.data;
                    Console.WriteLine($"SwapSnapshotOperation--GetAmmSnapshotByApi:获取交易所支持的amm的Snapshot数据成功！");
                    ExLoopring.LOGGER.Info($"SwapSnapshotOperation--GetAmmSnapshotByApi:获取交易所支持的amm的Snapshot数据成功！");
                }
                else
                {
                    Console.WriteLine($"SwapSnapshotOperation--GetAmmSnapshotByApi:获取交易所支持的amm的Snapshot数据异常！{Environment.NewLine}errCode={ammSnapshotResponse.resultInfo.code},errMsg={((ReponseErrCode)ammSnapshotResponse.resultInfo.code).GetDescription()}");
                    ExLoopring.LOGGER.Info($"SwapSnapshotOperation--GetAmmSnapshotByApi:获取交易所支持的amm的Snapshot数据异常！{Environment.NewLine}errCode={ammSnapshotResponse.resultInfo.code},errMsg={((ReponseErrCode)ammSnapshotResponse.resultInfo.code).GetDescription()}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"SwapSnapshotOperation--GetAmmSnapshotByApi:获取交易所支持的amm的Snapshot数据异常！{Environment.NewLine}ex.message:{ex.Message},{Environment.NewLine}ex.StackTrace:{ex.StackTrace}");
                ExLoopring.LOGGER.Error($"SwapSnapshotOperation--GetAmmSnapshotByApi:获取交易所支持的amm的Snapshot数据异常！", ex);
            }
            return ammSnapshot;
        }


        /// <summary>
        /// 定时获取Amm的所有Snapshot数据
        /// </summary>
        /// <returns></returns>
        private void GetAmmSnapshotsByApi()
        {
            List<AmmSnapshotModel> ammSnapshots = null;
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        AmmSnapshotsResponseModel ammSnapshotsResponse = this._httpClient.Get<AmmSnapshotsResponseModel>(urlPath: this._ammSnapshotsPath);

                        // 成功
                        if (ammSnapshotsResponse.resultInfo.code == 0)
                        {
                            ammSnapshots = ammSnapshotsResponse.data;
                            Console.WriteLine($"SwapSnapshotOperation--GetAmmSnapshotsByApi:获取交易所支持的amm的Snapshot数据成功！");
                            ExLoopring.LOGGER.Info($"SwapSnapshotOperation--GetAmmSnapshotsByApi:获取交易所支持的amm的Snapshot数据成功！");

                            // 调用事件
                            if (this.GetSwapDepthEvent != null)
                            {
                                this.GetSwapDepthEvent.Invoke(this, this.GetSnapshotsByPairs(ammSnapshots));
                            }

                        }
                        else
                        {
                            Console.WriteLine($"SwapSnapshotOperation--GetAmmSnapshotsByApi:获取交易所支持的amm的Snapshot数据异常！{Environment.NewLine}errCode={ammSnapshotsResponse.resultInfo.code},errMsg={((ReponseErrCode)ammSnapshotsResponse.resultInfo.code).GetDescription()}");
                            ExLoopring.LOGGER.Info($"SwapSnapshotOperation--GetAmmSnapshotsByApi:获取交易所支持的amm的Snapshot数据异常！{Environment.NewLine}errCode={ammSnapshotsResponse.resultInfo.code},errMsg={((ReponseErrCode)ammSnapshotsResponse.resultInfo.code).GetDescription()}");
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"SwapSnapshotOperation--GetAmmSnapshotsByApi:获取交易所支持的amm的Snapshot数据异常！{Environment.NewLine}ex.message:{ex.Message},{Environment.NewLine}ex.StackTrace:{ex.StackTrace}");
                        ExLoopring.LOGGER.Error($"SwapSnapshotOperation--GetAmmSnapshotsByApi:获取交易所支持的amm的Snapshot数据异常！", ex);
                    }

                    using (this._lockSnapshots.Write())
                    {
                        this._ammSnapshots = ammSnapshots.ToDictionary(amm=>amm.poolAddress);
                    }

                    await Task.Delay(350);
                }
            });
            
        }


        /// <summary>
        /// 获取swap的snapshots
        /// </summary>
        /// <param name="ammSnapshots">loopring格式的amm闪兑交易数据</param>
        /// <param name="pairNames">交易对名称列表，如果为null，则获取全部数据，默认为null</param>
        /// <returns></returns>
        private List<SwapSnapshotModel> GetSnapshotsByPairs(List<AmmSnapshotModel> ammSnapshots, List<string> pairNames = null)
        {
            List<SwapSnapshotModel> swapSnapshots = new List<SwapSnapshotModel>();
            //SwapMarketPairModel swapMarket = ExLoopring.OPERATION_SWAP_MARKET.GetSwapMarketPairModel(swapMarketPairName);

            using (this._lockSnapshots.Read())
            {
                //List<AmmSnapshotModel> ammSnapshots = this._ammSnapshots;
                if (ammSnapshots != null && ammSnapshots.Count > 0)
                {
                    List<AmmSnapshotModel> ammSnapshotsFilter = new List<AmmSnapshotModel>();
                    if (pairNames != null && pairNames.Count > 0)
                    {
                        ammSnapshotsFilter = ammSnapshots.FindAll(asm =>
                        {
                            return pairNames.Contains(asm.poolName);
                        });
                    }
                    else
                    {
                        ammSnapshotsFilter = ammSnapshots;
                    }

                    foreach (var model in ammSnapshotsFilter)
                    {
                        // 排除价格异常的数据
                        if (model.Price <= 0 || model.ReversePrice <= 0)
                            continue;
                        SwapSnapshotModel swapSnapshot = new SwapSnapshotModel();
                        swapSnapshot.MarketPair = ExLoopring.OPERATION_SWAP_MARKET.GetSwapMarketPairModel(model.poolName);
                        swapSnapshot.PoolTokenAmount = model.PoolTokenAmount;
                        swapSnapshot.PoolBaseTokenAmount = model.PoolBaseTokenAmount;
                        swapSnapshot.PoolQuoteTokenAmount = model.PoolQuoteTokenAmount;
                        swapSnapshot.Price = model.Price;
                        swapSnapshot.ReversePrice = model.ReversePrice;

                        swapSnapshots.Add(swapSnapshot);
                    }
                }
            }
            return swapSnapshots;
        }



        #region 订阅

        /// <summary>
        /// 增加ammpool的订阅pool名称
        /// </summary>
        /// <param name="pairs">交易对列表</param>
        public void SetAmmPoolNames(List<string> poolNames)
        {
            if (poolNames == null || poolNames.Count <= 0)
                return;

            List<string> addrs = new List<string>();
            poolNames.ForEach(name =>
            {
                addrs.Add(ExLoopring.SWAP_MARKETS[name].address);
            });

            // 添加 交易对的深度获取事件订阅
            this.AddAmmPoolSnapshotSubscribe(addrs);

        }

        /// <summary>
        /// 增加 交易对的 深度获取事件的订阅
        /// </summary>
        /// <param name="pair">交易对</param>
        private void AddAmmPoolSnapshotSubscribe(List<string> addrs)
        {
            if (addrs == null || addrs.Count <= 0)
                return;

            List<TopicModel> baseTopics = new List<TopicModel>();
            addrs.ForEach(p =>
            {
                TopicModel baseTopic = new TopicModel();
                baseTopic.topic = TopicTypeEnum.ammpool.ToString();
                baseTopic.poolAddress = p;
                baseTopic.snapshot = true;
                baseTopics.Add(baseTopic);
            });

            // 组装订阅主题 并 发送信息
            SubscribeModel<TopicModel> subscribeModel = new SubscribeModel<TopicModel>
            {
                op = SubscribeTypeEnum.sub.ToString(),
                unsubscribeAll = false,
                topics = baseTopics
            };

            ExLoopring.SUBSCRIBE_MANAGER.SendSubscribe(subscribeModel);

            string strArrPair = string.Join(',', addrs.ToArray());
            Console.WriteLine($"SwapSnapshotOperation--AddPairDepthSubscribe:已订阅（{strArrPair}）的orderbook主题！");
            ExLoopring.LOGGER.Info($"SwapSnapshotOperation--AddPairDepthSubscribe:已订阅（{strArrPair}）的orderbook主题！");
        }

        /// <summary>
        /// 移除 交易对的 深度获取事件的订阅
        /// </summary>
        /// <param name="pair">交易对</param>
        private void RemovePairDepthSubscribe(string pair)
        {

        }


        /// <summary>
        /// 获取 调度器 分发的 amm深度推送数据
        /// </summary>
        /// <param name="pushMessageModel">推送的 深度信息数据</param>
        public void GetDepthDispatcher(PushMessageModel<AmmPoolTopicModel, ArrayList> pushMessageModel)
        {
            if (pushMessageModel == null)
                return;

            string poolAddr = pushMessageModel.topic.poolAddress;
            AmmSnapshotModel ammModel = new AmmSnapshotModel();
            AmmMarketModel ammMarket = ExLoopring.SWAP_MARKETS.Values.First(amm => amm.address == poolAddr);
            try
            {
                ammModel.poolName = ammMarket.name;
                ammModel.poolAddress = ammMarket.address;
                ammModel.poolTokenId = ammMarket.poolTokenId.ToString();
                ammModel.tokenIds = ammMarket.inPoolTokens;
                ammModel.PoolTokenAmount = pushMessageModel.data[1].ToString(); 
                ammModel.tokenAmounts = ((JArray)pushMessageModel.data[0]).ToObject<List<string>>();

                using (this._lockSnapshots.Write())
                {
                    // 有 此交易对深度，无的话什么都不做
                    if (this._ammSnapshots.ContainsKey(poolAddr))
                    {
                        this._ammSnapshots[poolAddr] = ammModel;
                    }
                    else
                    {
                        this._ammSnapshots.Add(poolAddr, ammModel);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SwapSnapshotOperation--GetDepthDispatcher:获取“{ammMarket.name}”的amm交易对深度异常！ex.StackTrace:{ex.StackTrace}");
                ExLoopring.LOGGER.Error($"SwapSnapshotOperation--GetDepthDispatcher:获取“{ammMarket.name}”的amm交易对深度异常！", ex);
            }
        }
        #endregion

    }
}
