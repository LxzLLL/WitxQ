using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WitxQ.Exchange.Loopring.Models.Balance;
using WitxQ.Exchange.Loopring.Models.Depth;
using WitxQ.Exchange.Loopring.Models.SwapSnapshot;
using WitxQ.Exchange.Loopring.Models.WS;
using WitxQ.Exchange.Loopring.Models.WS.Topic;

namespace WitxQ.Exchange.Loopring.Sys
{
    /// <summary>
    /// websocket推送消息 调度器
    /// </summary>
    public class WSMessageDispatcher
    {

        /// <summary>
        /// 调度
        /// </summary>
        /// <param name="msg">回传消息</param>
        public void Dispatcher(string msg)
        {
            //Console.WriteLine("WSMessageDispatcher--Dispatcher--Received:" + msg);
            //ExLoopring.LOGGER.Info("WSMessageDispatcher--Dispatcher--Received:" + msg);

            var dynamicObject = JsonConvert.DeserializeObject<JObject>(msg);
            try
            {
                // 是订阅时的回传结果
                if (dynamicObject["result"] != null)
                {
                    WsSubscribeResultModel<BaseTopicModel> resultInfo = JsonConvert.DeserializeObject<WsSubscribeResultModel<BaseTopicModel>>(msg);
                }
                else
                {
                    // 订阅后的推送
                    if (dynamicObject["topic"] != null)
                    {
                        string strTopicType = dynamicObject["topic"]["topic"].ToString();
                        TopicTypeEnum topicType = (TopicTypeEnum)Enum.Parse(typeof(TopicTypeEnum), strTopicType, true);
                        switch (topicType)
                        {
                            // 订单薄数据
                            case TopicTypeEnum.orderbook:
                                PushMessageModel<OrderBookTopicModel, DepthModel> pushMessageDepthModel =
                                JsonConvert.DeserializeObject<PushMessageModel<OrderBookTopicModel, DepthModel>>(msg);

                                ExLoopring.OPERATION_DEPTH.GetDepthDispatcher(pushMessageDepthModel);
                                break;
                            // 账户的余额变动推送数据
                            case TopicTypeEnum.account:
                                PushMessageModel<AccountTopicModel, BalancesModel> pushMessageBalancesModel =
                                JsonConvert.DeserializeObject<PushMessageModel<AccountTopicModel, BalancesModel>>(msg);

                                // 需要找到是哪个账户的，才能进行 分发调度
                                if (ExLoopring.EXCHANGE_ACCOUNTS.ContainsKey(pushMessageBalancesModel.data.accountId))
                                    ExLoopring.EXCHANGE_ACCOUNTS[pushMessageBalancesModel.data.accountId].Balance.GetBalanceDispatcher(pushMessageBalancesModel);
                                break;
                            case TopicTypeEnum.ammpool:
                                // ArrayList[0]:  [,] tokenAmount ；  ArrayList[1]:string  poolTokenAmount
                                PushMessageModel<AmmPoolTopicModel, ArrayList> pushMessageAmmPoolModel =
                                JsonConvert.DeserializeObject<PushMessageModel<AmmPoolTopicModel, ArrayList>>(msg);

                                ExLoopring.OPERATION_SWAP_SNAPSHOT.GetDepthDispatcher(pushMessageAmmPoolModel);
                                break;
                            default: break;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"WSMessageDispatcher--Dispatcher:json解析topic异常，msg：{msg}");
                        ExLoopring.LOGGER.Info($"WSMessageDispatcher--Dispatcher:json解析topic异常，msg：{msg}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WSMessageDispatcher--Dispatcher:json解析异常，msg：{msg}，error:{ex.Message}");
                ExLoopring.LOGGER.Error($"WSMessageDispatcher--Dispatcher:json解析异常，msg：{msg}",ex);
            }

        }

    }
}
