using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Models.WS;
using WitxQ.Exchange.Loopring.Models.WS.Topic;

namespace WitxQ.Exchange.Loopring.Sys
{
    /// <summary>
    /// 订阅的管理类
    /// </summary>
    public class SubscribeManager
    {
        /// <summary>
        /// 全局的订阅列表（需要线程安全，可能有多用户同时操作）
        /// <para>
        /// 此订阅信息用于 断线后重连的 重新订阅
        /// </para>
        /// </summary>
        private List<SubscribeModel<TopicModel>> _subscribeTopics = new List<SubscribeModel<TopicModel>>();

        /// <summary>
        /// 全局订阅列表的锁
        /// </summary>
        private static readonly object LOCK_SUBSCRIBETOPICS = new object();

        #region SubscribeManager
        private SubscribeManager() { }

        /// <summary>
        /// 获取单例QuantEngine
        /// </summary>
        /// <returns></returns>
        public static SubscribeManager GetInstance()
        {
            return InnerInstance.instance;
        }

        private class InnerInstance
        {
            static InnerInstance() { }
            internal static SubscribeManager instance = new SubscribeManager();
        }
        #endregion


        /// <summary>
        /// 发送全部订阅，用于WSClient的Open事件回调（重新订阅）
        /// </summary>
        public void SendAllSubscribes(object sender, EventArgs e)
        {
            lock (LOCK_SUBSCRIBETOPICS)
            {
                // 在重连状态下  才重新订阅
                if (this._subscribeTopics == null || this._subscribeTopics.Count <= 0 || !ExLoopring.WSCLIENT.IsReConnecting)
                    return;

                this._subscribeTopics.ForEach(sm =>
                {
                    int aid = -1;
                    try
                    {
                        if (sm.topics != null && sm.topics.Count > 0)
                        {
                            // 发送订阅
                            ExLoopring.WSCLIENT.SendMessage(JsonConvert.SerializeObject(sm));

                            AccountModel account = ExLoopring.EXCHANGE_ACCOUNTS.Values.FirstOrDefault(exAccount => exAccount.Account.ApiKey.Equals(sm.apiKey))?.Account;
                            if (account != null)
                                aid = account.AccountId;

                            ExLoopring.LOGGER.Info($"SubscribeManager--SendAllSubscribes:{(aid > 0 ? "账户：" + aid : "")}，订阅“{sm.topics[0].topic}”主题消息已发送！");
                        }
                    }
                    catch (Exception ex)
                    {
                        ExLoopring.LOGGER.Error($"SubscribeManager--SendAllSubscribes:{(aid > 0 ? "账户：" + aid : "")}，订阅“{sm.topics[0].topic}”主题消息失败！Error message：{ex.Message},errstack:{ex.StackTrace}");
                    }

                });
            }
        }

        /// <summary>
        /// 发送指定的订阅
        /// </summary>
        public void SendSubscribe(SubscribeModel<TopicModel> subscribeModel)
        {
            if (subscribeModel == null || subscribeModel.topics == null || subscribeModel.topics.Count <= 0)
                return;

            lock(LOCK_SUBSCRIBETOPICS)
            {
                this._subscribeTopics.Add(subscribeModel);
            }
            // 发送订阅
            ExLoopring.WSCLIENT.SendMessage(JsonConvert.SerializeObject(subscribeModel));
        }


        /// <summary>
        /// 取消 账户金额信息 订阅
        /// </summary>
        /// <param name="accountTopic"></param>
        public void RemoveBalanceSubscribe(SubscribeModel<TopicModel> accountTopic)
        {
            if (accountTopic == null || accountTopic.topics == null || accountTopic.topics.Count <= 0)
                return;

            // 获取APIKEY
            string strApiKey = accountTopic.apiKey;
            if (string.IsNullOrWhiteSpace(strApiKey))
                return;

            // 从全局列表中，删除 指定账户的 金额信息订阅
            lock(LOCK_SUBSCRIBETOPICS)
            {
                // 遍历所有订阅
                this._subscribeTopics.ForEach(sm =>
                {
                    // 删除 apikey的 account的订阅
                    if (sm.apiKey.Equals(strApiKey))
                    {
                        if(sm.topics!=null && sm.topics.Count>0)
                        {
                            sm.topics.RemoveAll(topic =>
                            {
                                return topic.topic.Equals(TopicTypeEnum.account.ToString());
                            });
                        }
                    }
                });

                // 目前 不知道订阅集合中的topics的列表是否为空，需要再次遍历删除
                this._subscribeTopics.RemoveAll(subscribe =>
                {
                    return subscribe.topics == null || subscribe.topics.Count <= 0;
                });
            }

            // 发送取消订阅
            ExLoopring.WSCLIENT.SendMessage(JsonConvert.SerializeObject(accountTopic));
        } 

    }
}
