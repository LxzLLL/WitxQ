using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WitxQ.Common;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Models.Balance;
using WitxQ.Exchange.Loopring.Models.Token;
using WitxQ.Exchange.Loopring.Models.WS;
using WitxQ.Exchange.Loopring.Models.WS.Topic;
using WitxQ.Exchange.Loopring.Tools;
using WitxQ.Interface.Spot;
using CommonModel = WitxQ.Model;

namespace WitxQ.Exchange.Loopring.Operation
{
    /// <summary>
    /// 查询用户交易所余额 操作类
    /// </summary>
    public class BalanceOperation : BaseOperation, IBalances
    {

        /// <summary>
        /// 此账号所有token的信息
        /// <para>
        /// key:tokenID，value：深度信息
        /// </para>
        /// </summary>
        private ConcurrentDictionary<int, Models.Balance.BalancesModel> _balancesModels = new ConcurrentDictionary<int, Models.Balance.BalancesModel>();

        /// <summary>
        /// 查询用户交易所余额 的构造
        /// </summary>
        /// <param name="account">账号信息</param>
        public BalanceOperation(AccountModel account):base("/api/v2/user/balances")
        {
            this.Account = account;

            // 通过Api先获取 账户信息
            this.GetBalancesByApi();
            Thread.Sleep(5000);
            // 订阅 账户变动信息（需要在）
            this.AddBalancesSubscribe();
        }


        /// <summary>
        /// 获取 调度器 分发的 账号金额推送数据
        /// <para>
        /// 此只是更新，需要事先请求获取账号金额
        /// </para>
        /// </summary>
        /// <param name="pushMessageModel">推送的 账号金额息数据</param>
        public void GetBalanceDispatcher(PushMessageModel<AccountTopicModel, Models.Balance.BalancesModel> pushMessageModel)
        {
            if (pushMessageModel == null )
                return;

            // 如果不是这个账户的，则不处理
            Models.Balance.BalancesModel msgBalances = pushMessageModel.data;
            if (this.Account.AccountId != msgBalances.accountId)
                return;

            int tokenId = msgBalances.tokenId;
            try
            {
                // 有 此tokenid的余额信息，则替换，无的话进行添加
                if (this._balancesModels.ContainsKey(tokenId))
                {
                    BalancesModel prevBalance = this._balancesModels[tokenId];
                    if (!this._balancesModels.TryUpdate(tokenId, msgBalances, prevBalance))
                    {
                        Console.WriteLine($"BalanceOperation--GetBalanceDispatcher:更新账户：{this.Account.AccountId}，的“{tokenId}”的字典余额信息出错！");
                        ExLoopring.LOGGER.Info($"BalanceOperation--GetBalanceDispatcher:更新账户：{this.Account.AccountId}，的“{tokenId}”的字典余额信息出错！");
                    }
                }
                else
                {
                    // 不存在，则进行添加
                    if (!this._balancesModels.TryAdd(tokenId, msgBalances))
                    {
                        Console.WriteLine($"BalanceOperation--GetBalanceDispatcher:添加账户：{this.Account.AccountId}，的“{tokenId}”的字典余额信息出错！");
                        ExLoopring.LOGGER.Info($"BalanceOperation--GetBalanceDispatcher:添加账户：{this.Account.AccountId}，的“{tokenId}”的字典余额信息出错！");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BalanceOperation--GetBalanceDispatcher:处理账户：{this.Account.AccountId}，的token：{tokenId}余额信息异常！ex.StackTrace:{ex.StackTrace}");
                ExLoopring.LOGGER.Error($"BalanceOperation--GetBalanceDispatcher:处理账户：{this.Account.AccountId}，的token：{tokenId}余额信息异常！",ex);
            }
        }



        #region IBalances

        /// <summary>
        /// 通过token获取账户信息
        /// </summary>
        /// <param name="token">token，大写形式，例如ETH</param>
        /// <returns></returns>
        public CommonModel.Markets.BalancesModel GetBalancesByPair(string token)
        {
            CommonModel.Markets.BalancesModel commBalance = new CommonModel.Markets.BalancesModel();

            TokenModel tokenModel = ExLoopring.TOKENS.Values.FirstOrDefault(tm => tm.symbol.Equals(token));
            if(this._balancesModels.ContainsKey(tokenModel.tokenId))
            {
                Models.Balance.BalancesModel balance = this._balancesModels[tokenModel.tokenId];
                commBalance.TokenSymbol = tokenModel.symbol;
                commBalance.AvailableAmount = balance.AvailableAmountFormat;
                commBalance.LockedAmount = balance.FrozenAmountFormat;
            }

            return commBalance;
        }

        /// <summary>
        /// 获取账号 所有token的余额信息
        /// </summary>
        /// <returns></returns>
        public List<CommonModel.Markets.BalancesModel> GetBalances()
        {
            List<CommonModel.Markets.BalancesModel> commBalances = new List<CommonModel.Markets.BalancesModel>();
            List<BalancesModel> balances = this._balancesModels.Values.ToList();
            balances.ForEach(balance =>
            {
                CommonModel.Markets.BalancesModel commBalance = new CommonModel.Markets.BalancesModel();
                commBalance.TokenSymbol = balance.TokenSymbol;
                commBalance.AvailableAmount = balance.AvailableAmountFormat;
                commBalance.LockedAmount = balance.FrozenAmountFormat;
                commBalances.Add(commBalance);
            });
            return commBalances;
        }

        #endregion

        /// <summary>
        /// 通过Api获取账号全部资金数据
        /// </summary>
        private void GetBalancesByApi()
        {
            HttpRequestClient2 requestClient = new HttpRequestClient2(this.Url);
            bool blnIsSuccess = false;    // 是否获取数据成功
            do
            {
                try
                {
                    BalancesResponseModel responseModel = requestClient.Get<BalancesResponseModel>(new { accountId = this.Account.AccountId },
                        new Dictionary<string, string>() { { "X-API-KEY", this.Account.ApiKey } });

                    // 成功
                    if(responseModel.resultInfo.code==0)
                    {
                        List<Models.Balance.BalancesModel> balances = responseModel.data;
                        balances.ForEach(b =>
                        {
                            this._balancesModels.AddOrUpdate(b.tokenId, b, (key, oldValue) => b);
                        });

                        blnIsSuccess = true;
                        Console.WriteLine($"BalanceOperation--GetBalancesByApi:获取账户：{this.Account.AccountId}，金额数据成功！");
                        ExLoopring.LOGGER.Info($"BalanceOperation--GetBalancesByApi:获取账户：{this.Account.AccountId}，金额数据成功！");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"BalanceOperation--GetBalancesByApi:获取账户：{this.Account.AccountId}，金额数据异常！errCode={responseModel.resultInfo.code},errMsg={((ReponseErrCode)responseModel.resultInfo.code).GetDescription()}");
                        ExLoopring.LOGGER.Info($"BalanceOperation--GetBalancesByApi:获取账户：{this.Account.AccountId}，金额数据异常！errCode={responseModel.resultInfo.code},errMsg={((ReponseErrCode)responseModel.resultInfo.code).GetDescription()}");
                    }

                }
                catch(Exception ex)
                {
                    Console.WriteLine($"BalanceOperation--GetBalancesByApi:获取账户：{this.Account.AccountId}，金额数据异常！ex.StackTrace:{ex.StackTrace}");
                    ExLoopring.LOGGER.Error($"BalanceOperation--GetBalancesByApi:获取账户：{this.Account.AccountId}，金额数据异常！",ex);
                }
                // 等200ms，由于交易所调用次数限制
                Thread.Sleep(200);
                //Task.Delay(200).Wait();

            } while (!blnIsSuccess);
        }

        /// <summary>
        /// 添加 账号的金额变更 订阅
        /// </summary>
        private void AddBalancesSubscribe()
        {
            // 订阅主题列表
            //List<AccountTopicModel> baseTopics = new List<AccountTopicModel>();
            //AccountTopicModel accountTopic = new AccountTopicModel();
            //accountTopic.topic = TopicTypeEnum.account.ToString();
            //baseTopics.Add(accountTopic);

            //// 组装订阅主题 并 发送信息
            //SubscribeModel<AccountTopicModel> subscribeModel = new SubscribeModel<AccountTopicModel>
            //{
            //    op = SubscribeTypeEnum.sub.ToString(),
            //    unsubscribeAll = false,
            //    apiKey=this.Account.ApiKey,
            //    topics = baseTopics
            //};
            //ExLoopring.WSCLIENT.SendMessage(JsonConvert.SerializeObject(subscribeModel));

            List<TopicModel> baseTopics = new List<TopicModel>();
            TopicModel accountTopic = new TopicModel();
            accountTopic.topic = TopicTypeEnum.account.ToString();
            baseTopics.Add(accountTopic);

            // 组装订阅主题 并 发送信息
            SubscribeModel<TopicModel> subscribeModel = new SubscribeModel<TopicModel>
            {
                op = SubscribeTypeEnum.sub.ToString(),
                unsubscribeAll = false,
                apiKey = this.Account.ApiKey,
                topics = baseTopics
            };

            ExLoopring.SUBSCRIBE_MANAGER.SendSubscribe(subscribeModel);

            Console.WriteLine($"BalanceOperation--AddBalancesSubscribe:账户：{this.Account.AccountId}，订阅account主题消息已发送！");
            ExLoopring.LOGGER.Info($"BalanceOperation--AddBalancesSubscribe:账户：{this.Account.AccountId}，订阅account主题消息已发送！");
        }

        
    }
}
