using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WitxQ.Common.Locks;
using WitxQ.Interface.Spot;
using WitxQ.Exchange.Loopring.Models.Depth;
using CommonModel = WitxQ.Model;
using System.Linq;
using log4net.Util;
using WitxQ.Exchange.Loopring.Models.WS;
using WitxQ.Exchange.Loopring.Models.WS.Topic;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Threading.Tasks;
using WitxQ.Common;
using WitxQ.Exchange.Loopring.Tools;

namespace WitxQ.Exchange.Loopring.Operation
{
    /// <summary>
    /// 深度 操作类
    /// </summary>
    public class DepthOperation : BaseOperation, IDepth
    {
        /// <summary>
        /// 所有深度信息
        /// <para>
        /// key:交易对，value：深度信息
        /// </para>
        /// </summary>
        private ConcurrentDictionary <string, DepthModel> _depthModels = new ConcurrentDictionary<string, DepthModel>();

        ///// <summary>
        ///// 每个深度信息的 读写锁（用于字典内的某一个交易对深度的读写）
        ///// <para>
        ///// key:交易对，value：深度信息的 读写锁
        ///// </para>
        ///// </summary>
        //private readonly Dictionary<string, UsingReaderWriterLock<object>> _lockSlims = new Dictionary<string, UsingReaderWriterLock<object>>();

        ///// <summary>
        ///// 所有深度信息的 读写锁（用于整体字典的读写，例如添加交易对时，需要更新整体字典数据，包括锁字典）
        ///// </summary>
        //private readonly UsingReaderWriterLock<object> _lockDepthSlim = new UsingReaderWriterLock<object>();


        /// <summary>
        /// 构造
        /// </summary>
        public DepthOperation():base("/api/v2/depth")
        {}

        /// <summary>
        /// 增加市场交易对（同时会添加深度回调事件）
        /// </summary>
        /// <param name="pairs">交易对列表</param>
        public void SetMarkPair(List<string> pairs)
        {
            if (pairs == null || pairs.Count <= 0)
                return;

            List<string> needPairs = new List<string>();
            pairs.ForEach(p =>
            {
                // 不存在字典中，则增加字典并添加 事件
                if(!this._depthModels.ContainsKey(p))
                {
                    // 如果添加成功，则添加 获取深度事件
                    if(this._depthModels.TryAdd(p,new DepthModel()))
                    {
                        needPairs.Add(p);
                    }
                    else
                    {
                        Console.WriteLine($"DepthOperation--SetMarkPair:添加“{p}”交易对失败！");
                        ExLoopring.LOGGER.Info($"DepthOperation--SetMarkPair:添加“{p}”交易对失败！");
                    }
                }
            });

            // 添加 交易对的深度获取事件订阅
            this.AddPairDepthSubscribe(needPairs);

        }

        /// <summary>
        /// 增加 交易对的 深度获取事件的订阅
        /// </summary>
        /// <param name="pair">交易对</param>
        private void AddPairDepthSubscribe(List<string> pairs)
        {
            if (pairs == null || pairs.Count <= 0)
                return;

            // 订阅主题列表
            //List<OrderBookTopicModel> baseTopics = new List<OrderBookTopicModel>();
            //pairs.ForEach(p =>
            //{
            //    OrderBookTopicModel baseTopic = new OrderBookTopicModel();
            //    baseTopic.topic = TopicTypeEnum.orderbook.ToString();
            //    baseTopic.market = p;
            //    baseTopic.level = p=="LRC-ETH"?2:0;   // TODO AddPairDepthEventHandler  需要根据交易对的配置进行赋值
            //    baseTopic.count = 5;
            //    baseTopic.snapshot = true;
            //    baseTopics.Add(baseTopic);
            //});

            //// 组装订阅主题 并 发送信息
            //SubscribeModel<OrderBookTopicModel> subscribeModel = new SubscribeModel<OrderBookTopicModel>
            //{
            //    op = SubscribeTypeEnum.sub.ToString(),
            //    unsubscribeAll = false,
            //    topics = baseTopics
            //};
            //ExLoopring.WSCLIENT.SendMessage(JsonConvert.SerializeObject(subscribeModel));

            List<TopicModel> baseTopics = new List<TopicModel>();
            pairs.ForEach(p =>
            {
                TopicModel baseTopic = new TopicModel();
                baseTopic.topic = TopicTypeEnum.orderbook.ToString();
                baseTopic.market = p;
                baseTopic.level = p == "LRC-ETH" ? 2 : 0;   // TODO AddPairDepthEventHandler  需要根据交易对的配置进行赋值
                baseTopic.count = 5;
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

            string strArrPair = string.Join(',', pairs.ToArray());
            Console.WriteLine($"DepthOperation--AddPairDepthSubscribe:已订阅（{strArrPair}）的orderbook主题！");
            ExLoopring.LOGGER.Info($"DepthOperation--AddPairDepthSubscribe:已订阅（{strArrPair}）的orderbook主题！");
        }

        /// <summary>
        /// 移除 交易对的 深度获取事件的订阅
        /// </summary>
        /// <param name="pair">交易对</param>
        private void RemovePairDepthSubscribe(string pair)
        {

        }


        /// <summary>
        /// 获取 调度器 分发的 深度推送数据
        /// </summary>
        /// <param name="pushMessageModel">推送的 深度信息数据</param>
        public void GetDepthDispatcher(PushMessageModel<OrderBookTopicModel, DepthModel> pushMessageModel)
        {
            if (pushMessageModel == null)
                return;

            string marketPair = pushMessageModel.topic.market;
            try
            {
                // 有 此交易对深度，无的话什么都不做
                if (this._depthModels.ContainsKey(marketPair))
                {
                    // 判断每个档位的价格和数量是否一致，如果不一致则更新且调用事件，否则只更新
                    DepthModel prevDepth = this._depthModels[marketPair];
                    DepthModel currDepth = pushMessageModel.data;
                    bool blnIsChange = false;    // depth是否有变动，默认“没有变动”

                    // 当前深度是否正常
                    if (currDepth != null && currDepth.asks != null && currDepth.asks.Count > 0
                        && currDepth.bids != null && currDepth.bids.Count > 0)
                    {
                        blnIsChange = this.IsDepthChange(currDepth, prevDepth);

                        // 如果 深度改变 且 深度更新成功，则调用“深度获取”事件
                        if (blnIsChange)
                        {
                            DepthModel changedDepth = new DepthModel();
                            changedDepth.timestamp = pushMessageModel.ts;
                            changedDepth.asks = currDepth.asks;
                            changedDepth.bids = currDepth.bids;

                            // 更新深度信息
                            if (this._depthModels.TryUpdate(marketPair, changedDepth, prevDepth))
                            {
                                CommonModel.Markets.DepthModel depth = new CommonModel.Markets.DepthModel();
                                depth.PairSymbol = marketPair;
                                //depth.BuyBids = changedDepth.bids.Select<List<string>, List<decimal>>(ls => ls.Select<string, decimal>(s => ConvertHelper.StringToDecimal(s)).ToList()).ToList();
                                //depth.SellAsks = changedDepth.asks.Select<List<string>, List<decimal>>(ls => ls.Select<string, decimal>(s => ConvertHelper.StringToDecimal(s)).ToList()).ToList();
                                depth.BuyBids = LoopringConvert.GetOrderBookConvert(changedDepth.bids,marketPair);
                                depth.SellAsks = LoopringConvert.GetOrderBookConvert(changedDepth.asks, marketPair);

                                // 调用事件
                                Task.Run(() => this.GetDepthEvent?.Invoke(this, depth));
                            }
                        } // end for  if(深度是否改变)

                    } // end for  if(当前深度是否正常)

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"DepthOperation--GetDepthDispatcher:获取“{marketPair}”交易对深度异常！ex.StackTrace:{ex.StackTrace}");
                ExLoopring.LOGGER.Error($"DepthOperation--GetDepthDispatcher:获取“{marketPair}”交易对深度异常！",ex);
            }
        }



        #region IDepth

        /// <summary>
        /// 获取交易深度事件
        /// </summary>
        public event EventHandler<CommonModel.Markets.DepthModel> GetDepthEvent;

        /// <summary>
        /// 通过交易对获取深度信息
        /// </summary>
        /// <param name="pair">交易对，全部为中间“-”连字符的大写形式</param>
        /// <returns></returns>
        public CommonModel.Markets.DepthModel GetDepthByPair(string pair)
        {
            CommonModel.Markets.DepthModel depthCommonModel = new CommonModel.Markets.DepthModel();
            try
            {
                DepthModel dm = this._depthModels[pair];
                depthCommonModel.PairSymbol = pair;
                // 从 List<List<string>>转为 List<List<decimal>>
                //depthCommonModel.BuyBids = this._depthModels[pair].bids.Select<List<string>,List<decimal>>(ls=>ls.Select<string,decimal>(s=> ConvertHelper.StringToDecimal(s)).ToList()).ToList();
                //depthCommonModel.SellAsks = this._depthModels[pair].asks.Select<List<string>, List<decimal>>(ls => ls.Select<string, decimal>(s => ConvertHelper.StringToDecimal(s)).ToList()).ToList();

                depthCommonModel.BuyBids = LoopringConvert.GetOrderBookConvert(dm.bids, pair);
                depthCommonModel.SellAsks = LoopringConvert.GetOrderBookConvert(dm.asks, pair);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return depthCommonModel;
        }
        #endregion


        /// <summary>
        /// 判断 深度（前deep个买卖单）是否改变
        /// </summary>
        /// <param name="currDepth">当前深度信息</param>
        /// <param name="prevDepth">上一个深度信息</param>
        /// <param name="deep">第前几个买卖单有改变（默认为1，只看第一个买卖单是否有改变）</param>
        /// <returns></returns>
        private bool IsDepthChange( DepthModel currDepth, DepthModel prevDepth,int deep=1)
        {
            bool blnIschange = false;

            // 上次Depth为空
            if (prevDepth.bids == null || prevDepth.bids.Count <= 0 || prevDepth.asks == null || prevDepth.asks.Count <= 0)
                return true;

            // 检查前几个买卖单有改变
            int deepChange = currDepth.bids.Count <= deep ? currDepth.bids.Count : deep;

            // 判断 买深度是否改变
            for (int i=0;i< deepChange; i++)
            {
                // 价格或数量不相等，表明有改变
                if (currDepth.bids[i][0] != prevDepth.bids[i][0] || currDepth.bids[i][1] != prevDepth.bids[i][1])
                {
                    blnIschange = true;
                    break;
                }   
            }

            // 判断 卖深度是否改变
            if (!blnIschange)
            {
                for (int i = 0; i < deepChange; i++)
                {
                    // 价格或数量不相等，表明有改变
                    if (currDepth.asks[i][0] != prevDepth.asks[i][0] || currDepth.asks[i][1] != prevDepth.asks[i][1])
                    {
                        blnIschange = true;
                        break;
                    }
                }
            }

            return blnIschange;
        }
    }
}
