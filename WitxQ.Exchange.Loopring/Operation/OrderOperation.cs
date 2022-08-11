using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WitxQ.Common;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Models.Market;
using WitxQ.Exchange.Loopring.Models.Order;
using WitxQ.Exchange.Loopring.Models.Token;
using WitxQ.Exchange.Loopring.Tools;
using WitxQ.Interface.Spot;


/*******************************************************************************************************************
 步骤：
    1.通过/api/v2/orderId获取正确的orderId。如果您在客户端维护订单ID，可以跳过该步骤。
    2.选择合理的validSince和validUntil值。我们推荐的参数是validSince设置为当前系统时间，validUntil设置成比当前时间晚至少一个星期。
    3.选择合理的MaxFeeBips。我们建议这个值设置为63。
    4.如果您想更好地追踪订单，请选择使用clientOrderId和（或）channelId。
    5.对订单进行签名。
    6.提交订单, 确认返回的结果包含订单哈希。 
******************************************************************************************************************/
namespace WitxQ.Exchange.Loopring.Operation
{
    /// <summary>
    /// 订单相关操作类
    /// </summary>
    public class OrderOperation : BaseOperation, IOrder
    {
        /// <summary>
        /// 获取OrderId的Path
        /// </summary>
        //private readonly string _orderIdUrlPath = "/api/v2/orderId";
        private readonly string _orderIdUrlPath = "/api/v2/storageId";

        /// <summary>
        /// 提交Order的Path
        /// </summary>
        private readonly string _orderUrlPath = "/api/v2/order";

        /// <summary>
        /// 提交OrderBatch的Path
        /// </summary>
        private readonly string _orderBatchUrlPath = "/api/v2/batchOrders";

        /// <summary>
        /// 提交OrderDelBatch的Path（DELETE http方法）
        /// </summary>
        private readonly string _orderDelBatchUrlPath = "/api/v2/orders/byHash";

        /// <summary>
        /// http请求客户端
        /// </summary>
        private readonly HttpRequestClient2 _httpClient;

        /// <summary>
        /// 查询用户交易所余额 的构造
        /// </summary>
        /// <param name="account">账号信息</param>
        public OrderOperation(AccountModel account) : base("")
        {
            this.Account = account;
            this.XApiKey = this.Account.ApiKey;
            this._httpClient = new HttpRequestClient2(this.DomainUrl);  // 使用 基础Url初始化，由各个操作设置path路径
        }

        #region IOrder

        /// <summary>
        /// 批量订单
        /// <para>
        /// 例如：LRC-ETH，如果side为sell（卖方向），即 卖LRC，买ETH，则 sellToken为LRC，buyToken为ETH；
        /// 如果side为buy（买方向），即 买LRC，卖ETH，则 sellToken为ETH，buyToken为LRC
        /// </para>
        /// </summary>
        /// <param name="orderBatchParams">批量订单参数,
        /// string[6]个参数，
        /// 0:market(例如：LRC-ETH),
        /// 1:side(订单买卖类型buy:买入;sell:卖出，针对交易对 例如LRC-ETH，买入即为买LRC),
        /// 2:price,
        /// 3:amount,
        /// 4:sellToken(例如：LRC),
        /// 5:buyToken(例如：ETH)
        /// </param>
        /// <param name="orderNumbers">返回的订单标识列表（可能有部分为空值，因为批量订单部分成功）</param>
        /// <param name="err">异常信息</param>
        /// <returns></returns>
        public bool OrderBatch(List<List<string>> orderParams, ref List<string> orderNumbers, ref string err)
        {
            bool blnIsSuccess = false;
            if(orderParams==null || orderParams.Count<=0)
            {
                err = "IOrder--OrderBatch：参数为空！";
                return blnIsSuccess;
            }

            try
            {
                // 组装OrderBatch请求
                List<OrderRequestModel> orderRequests = new List<OrderRequestModel>();

                
                DateTime startTime = DateTime.Now;

                // 开启任务并行获取
                Task[] tasks = new Task[orderParams.Count];
                for(int i=0;i< orderParams.Count;i++)
                {
                    List<string> orderParam = orderParams[i];
                    tasks[i] = Task.Factory.StartNew(new Action<object>((x)=>
                    {
                        List<string> param = x as List<string>;
                        decimal price = ConvertHelper.StringToDecimal(param[2]);
                        decimal amount = ConvertHelper.StringToDecimal(param[3]);

                        int tokenSId = ExLoopring.TOKENS.Values.First(token => token.symbol.Equals(param[4])).tokenId;
                        int tokenBId = ExLoopring.TOKENS.Values.First(token => token.symbol.Equals(param[5])).tokenId;
                        OrderRequestModel orderRequest = this.GetOrderRequest(param[0], price, amount, tokenSId, tokenBId);
                        orderRequests.Add(orderRequest);
                    }), orderParam);
                }

                Task.WaitAll(tasks);

                DateTime endTime = DateTime.Now;

                if (orderRequests.Count!=orderParams.Count)
                {
                    err = $"IOrder--OrderBatch：组装的OrderRequest请求个数({orderRequests.Count}个)与参数个数({orderParams.Count}个)不一致！";
                    return blnIsSuccess;
                }
                OrderBatchRequestModel orderBatchRequest = new OrderBatchRequestModel();
                orderBatchRequest.orders = orderRequests;

                DateTime startTime1 = DateTime.Now;

                OrderBatchResponseModel orderBatchResponse = this.SendBatchOrder(orderBatchRequest);

                DateTime endTime1 = DateTime.Now;

                // 记录时长日志
                Task.Run(() =>
                {
                    string strLog = $"GetHash+GetSign耗时：{(endTime - startTime).TotalMilliseconds} 毫秒，开始时间：{startTime}，结束时间：{endTime};" + Environment.NewLine;
                    strLog += $"SendBatchOrder耗时：{(endTime1 - startTime1).TotalMilliseconds} 毫秒，开始时间：{startTime1}，结束时间：{endTime1};" + Environment.NewLine;
                    ExLoopring.LOGGER.Info(strLog);
                });

                if (orderBatchResponse != null)
                {
                    int responseCode = orderBatchResponse.resultInfo.code;
                    // 成功或部分成功
                    if (responseCode == (int)ReponseErrCode.Success || responseCode == (int)ReponseErrCode.OrderBatchPartFailed)
                    {
                        blnIsSuccess = responseCode == (int)ReponseErrCode.Success?true: blnIsSuccess;
                        // 写入批量hash
                        for(int i=0;i< orderBatchResponse.data.Count;i++)
                        {
                            orderNumbers.Add(orderBatchResponse.data[i].hash);
                            if(string.IsNullOrWhiteSpace(orderBatchResponse.data[i].hash))
                            {
                                err += $"IOrder--OrderBatch:(ErrCode:{orderBatchResponse.resultInfo.code},ErrMsg:{orderBatchResponse.resultInfo.message}),";
                            }
                        }
                    }
                    else
                        err = $"IOrder--OrderBatch：ErrCode:{orderBatchResponse.resultInfo.code},ErrMsg:{orderBatchResponse.resultInfo.message}";
                }
                else
                {
                    err = "IOrder--OrderBatch：批量订单发送后无返回！";
                }
            }
            catch(Exception ex)
            {
                err = $"IOrder--OrderBatch：{ex.Message}";
            }
            return blnIsSuccess;
        }

        /// <summary>
        /// 买类型订单
        /// <para>
        /// 例如：LRC-ETH，买LRC，卖ETH，则 sellToken为ETH，buyToken为LRC
        /// </para>
        /// </summary>
        /// <param name="market">市场，交易对，全部为中间“-”连字符的大写形式,例如：LRC-ETH</param>
        /// <param name="price">价格</param>
        /// <param name="amount">数量</param>
        /// <param name="sellToken">出售的代币符号，例如LRC</param>
        /// <param name="buyToken">买入的代币符号，例如ETH</param>
        /// <param name="orderNumber">返回的订单标识</param>
        /// <param name="err">异常信息</param>
        /// <returns></returns>
        public bool OrderBuy(string market, string price, string amount, string sellToken, string buyToken,ref string orderNumber, ref string err)
        {
            return this.Order(market, price, amount, sellToken, buyToken, ref orderNumber, ref err);
        }

        /// <summary>
        /// 卖类型订单
        /// <para>
        /// 例如:LRC-ETH，卖LRC，买ETH，则 sellToken为LRC，buyToken为ETH
        /// </para>
        /// </summary>
        /// <param name="market">市场，交易对，全部为中间“-”连字符的大写形式,例如：LRC-ETH</param>
        /// <param name="price">价格</param>
        /// <param name="amount">数量</param>
        /// <param name="sellToken">出售的代币符号，例如LRC</param>
        /// <param name="buyToken">买入的代币符号，例如ETH</param>
        /// <param name="orderNumber">返回的订单标识</param>
        /// <param name="err">异常信息</param>
        /// <returns></returns>
        public bool OrderSell(string market, string price, string amount, string sellToken, string buyToken,ref string orderNumber, ref string err)
        {
            return this.Order(market, price, amount, sellToken, buyToken, ref orderNumber, ref err);
        }


        /// <summary>
        /// 批量取消订单
        /// </summary>
        /// <param name="orderNumbers">订单标识列表</param>
        /// <param name="delState">返回撤销订单状态</param>
        /// <param name="err">异常信息</param>
        /// <returns></returns>
        public bool OrderDelBatch(List<string> orderNumbers, ref List<bool> delState, ref string err)
        {
            bool blnIsSuccess = false;

            try
            {
                //OrderDelBatchRequestModel orderDelBatchRequest = ;
                if (orderNumbers == null || orderNumbers.Count <= 0)
                {
                    err = "IOrder--OrderBatch：订单标识列表为空！";
                    return blnIsSuccess;
                }
                
                OrderDelBatchRequestModel orderDelBatchRequest = new OrderDelBatchRequestModel();
                orderDelBatchRequest.accountId = this.Account.AccountId;
                orderDelBatchRequest.orderHash = string.Join(',', orderNumbers);

                // 签名
                string strXApiSign = LoopringConvert.GetApiSign("DELETE", this.DomainUrl + this._orderDelBatchUrlPath,
                    ConvertHelper.ConvertModelToDic<OrderDelBatchRequestModel>(orderDelBatchRequest), this.Account.SecretKey);

                OrderDelBatchResponseModel orderDelBatchResponse = this._httpClient.DeleteJson<OrderDelBatchResponseModel>(null,
                    orderDelBatchRequest, 
                    new Dictionary<string, string> 
                    { 
                        { "X-API-KEY", this.XApiKey },
                        { "X-API-SIG",strXApiSign }
                    }, 
                    this._orderDelBatchUrlPath);

                if (orderDelBatchResponse != null)
                {
                    int responseCode = orderDelBatchResponse.resultInfo.code;
                    // 成功或部分成功
                    if (responseCode == (int)ReponseErrCode.Success || responseCode == (int)ReponseErrCode.OrderBatchPartFailed)
                    {
                        blnIsSuccess = responseCode == (int)ReponseErrCode.Success ? true : blnIsSuccess;
                        // 写入批量hash
                        for (int i = 0; i < orderDelBatchResponse.data.Count; i++)
                        {
                            delState.Add(orderDelBatchResponse.data[i].result);
                        }
                    }
                    else
                        err = $"IOrder--OrderBatch：ErrCode:{orderDelBatchResponse.resultInfo.code},ErrMsg:{orderDelBatchResponse.resultInfo.message}";
                }
                else
                {
                    err = $"IOrder--OrderDelBatch：撤销订单发送后无返回！";
                }


            }
            catch(Exception ex)
            {
                err = $"IOrder--OrderDelBatch：{ex.Message}";
            }

            return blnIsSuccess;
        }

        #endregion

        /// <summary>
        /// 订单处理
        /// </summary>
        /// <param name="market">市场，交易对，全部为中间“-”连字符的大写形式,例如：LRC-ETH</param>
        /// <param name="price">价格</param>
        /// <param name="amount">数量</param>
        /// <param name="sellToken">出售的代币符号，例如LRC</param>
        /// <param name="buyToken">买入的代币符号，例如ETH</param>
        /// <param name="orderNumber">返回的订单标识</param>
        /// <param name="err">异常信息</param>
        /// <returns></returns>
        private bool Order(string market, string price, string amount, string sellToken, string buyToken, ref string orderNumber, ref string err)
        {
            bool blnIsSuccess = false;

            decimal dprice = ConvertHelper.StringToDecimal(price);
            decimal damount = ConvertHelper.StringToDecimal(amount);
            try
            {
                int tokenSId = ExLoopring.TOKENS.Values.First(token => token.symbol.Equals(sellToken)).tokenId;
                int tokenBId = ExLoopring.TOKENS.Values.First(token => token.symbol.Equals(buyToken)).tokenId;

                OrderRequestModel orderRequest = this.GetOrderRequest(market, dprice, damount, tokenSId, tokenBId);
                //OrderResponseModel orderResponse = this.SendSingleOrder(orderRequest);
                OrderResponseModelV3 orderResponseV3 = this.SendSingleOrderV3(orderRequest);

                if (orderResponseV3 != null)
                {
                    if (orderResponseV3.resultInfo.code == (int)ReponseErrCode.Success)
                    {
                        blnIsSuccess = true;
                        orderNumber = orderResponseV3.data.orderHash;
                    }
                    else
                        err = $"IOrder--Order(sid:{sellToken},bid:{buyToken})：ErrCode:{orderResponseV3.resultInfo.code},ErrMsg:{orderResponseV3.resultInfo.message}";
                }
                else
                {
                    err = $"IOrder--Order(sid:{sellToken},bid:{buyToken})：订单发送后无返回！";
                }
            }
            catch (Exception ex)
            {
                err = $"IOrder--Order(sid:{sellToken},bid:{buyToken})：{ex.Message}";
            }

            return blnIsSuccess;
        }


        /// <summary>
        /// 获取订单ID，根据提供的tokenSId
        /// </summary>
        /// <returns></returns>
        public OrderIdResponseModel GetOrderId(int tokenSId)
        {
            OrderIdRequestModel orderIdRequest = new OrderIdRequestModel();
            orderIdRequest.accountId = this.Account.AccountId;
            orderIdRequest.tokenSId = tokenSId;

            OrderIdResponseModel orderIdResponse = this._httpClient.Get<OrderIdResponseModel>(orderIdRequest, 
                new Dictionary<string, string> { { "X-API-KEY", this.XApiKey } }, this._orderIdUrlPath);

            return orderIdResponse;
        }

        /// <summary>
        /// 组装订单请求实体（已包括 获取订单ID）
        /// <para>
        /// 版本2的api参数有问题，截止2020年5月26日13:25:31
        /// </para>
        /// </summary>
        /// <param name="marketPair">交易对</param>
        /// <param name="price">价格</param>
        /// <param name="amount">总量</param>
        /// <param name="tokenSId">要卖的TokenId</param>
        /// <param name="tokenBId">要买的TokenId</param>
        /// <returns></returns>
        public OrderRequestModel GetOrderRequest(string marketPair, decimal price, decimal amount, int tokenSId, int tokenBId)
        {
            //1、获取单号
            OrderIdResponseModel orderIdModel = this.GetOrderId(tokenSId);
            //long orderId = ConvertHelper.StringToLong(orderIdModel.data.orderId);
            long orderId = orderIdModel.data.orderId;

            //2、下单
            OrderRequestModel orderRequest = new OrderRequestModel();
            //orderRequest.exchangeId = 2;    // loopring.io交易所ID
            orderRequest.orderId = orderId;
            orderRequest.storageId = orderId;
            orderRequest.orderType = "LIMIT_ORDER";
            orderRequest.accountId = this.Account.AccountId;
            orderRequest.tokenSId = tokenSId;
            orderRequest.tokenBId = tokenBId;
            orderRequest.amountS = this.GetLoopringAmountString(marketPair, price, amount, tokenSId);
            orderRequest.amountB = this.GetLoopringAmountString(marketPair, price, amount, tokenBId);
            orderRequest.allOrNone = false;

            MarketInfoModel marketInfo = ExLoopring.MARKETS[marketPair];
            // 如果tokenSId的是基础货币，则表明是卖的
            //orderRequest.buy = tokenSId == marketInfo.baseTokenId ? false : true;
            orderRequest.fillAmountBOrS = tokenSId == marketInfo.baseTokenId ? false : true;


            // 要减去 一小时？（为啥？）不然会提示valid since must less than (now-600) timestamp
            //orderRequest.validSince = ConvertHelper.DateTimeToUnixTime(DateTime.Now.AddHours(-10));
            orderRequest.validUntil = ConvertHelper.DateTimeToUnixTime(DateTime.Now.AddMonths(2));
            orderRequest.maxFeeBips = 50;
            orderRequest.label = 211;

            orderRequest.owner = this.Account.Address;
            orderRequest.tokenS = ExLoopring.TOKENS[tokenSId].address;
            orderRequest.tokenB = ExLoopring.TOKENS[tokenBId].address;
            orderRequest.amountSInBN = orderRequest.amountS;
            orderRequest.amountBInBN = orderRequest.amountB;
            orderRequest.feeBips = 50;
            //orderRequest.rebateBips = 0;

            //string hashArgs = $"[{orderRequest.exchangeId},{orderRequest.orderId},{orderRequest.accountId},{orderRequest.tokenSId},{orderRequest.tokenBId}," +
            //      $"'{orderRequest.amountS}','{orderRequest.amountB}',{orderRequest.allOrNone.ToString().ToLower()},{orderRequest.validSince}," +
            //       $"{orderRequest.validUntil},{orderRequest.maxFeeBips},{orderRequest.buy.ToString().ToLower()},{orderRequest.label}]";

            List<object> hashArgs = new List<object>() {
                orderRequest.exchange,
                //orderRequest.orderId,
                orderRequest.storageId,
                orderRequest.accountId,
                orderRequest.tokenSId,
                orderRequest.tokenBId,
                orderRequest.amountS,
                orderRequest.amountB,
                //orderRequest.allOrNone,
                //orderRequest.validSince,
                orderRequest.validUntil,
                orderRequest.maxFeeBips,
                //orderRequest.buy,
                orderRequest.fillAmountBOrS?1:0
                //orderRequest.label
                //orderRequest.taker
            };

            string strSign = ExLoopring.CEF_LOOPRING_SIGN.GetSignByArgs(this.Account.SecretKey, hashArgs);
            SignatureModel signature = JsonConvert.DeserializeObject<SignatureModel>(strSign);

            // 0x + 192 bytes hex string, i.e., 0x+Rx+Ry+s
            string hexRx = ConvertHelper.BaseConvert(signature.Rx, ConvertHelper.CHS_STR10, ConvertHelper.CHS_STR16).PadLeft(64, '0');  // 在最前端用'0'补足64位
            string hexRy = ConvertHelper.BaseConvert(signature.Ry, ConvertHelper.CHS_STR10, ConvertHelper.CHS_STR16).PadLeft(64, '0');  // 在最前端用'0'补足64位
            string hexS = ConvertHelper.BaseConvert(signature.s, ConvertHelper.CHS_STR10, ConvertHelper.CHS_STR16).PadLeft(64, '0');  // 在最前端用'0'补足64位
            orderRequest.eddsaSig = $"0x{hexRx + hexRy + hexS}";

            //orderRequest.signatureRx = signature.Rx;
            //orderRequest.signatureRy = signature.Ry;
            //orderRequest.signatureS = signature.s;

            //orderRequest.clientOrderId = "WitxQ";
            orderRequest.channelId = "hebao::subchannel::0001";

            return orderRequest;
        }

        #region 废弃 以前老版本  
        /// <summary>
        /// 组装订单请求实体（已包括 获取订单ID）----以前老版本  废弃
        /// <para>
        /// 版本2的api参数有问题，截止2020年5月26日13:25:31
        /// </para>
        /// </summary>
        /// <param name="marketPair">交易对</param>
        /// <param name="price">价格</param>
        /// <param name="amount">总量</param>
        /// <param name="tokenSId">要卖的TokenId</param>
        /// <param name="tokenBId">要买的TokenId</param>
        /// <returns></returns>
        public OrderRequestModelAbandoned GetOrderRequestAbandoned(string marketPair, decimal price, decimal amount, int tokenSId, int tokenBId)
        {
            //1、获取单号
            OrderIdResponseModel orderIdModel = this.GetOrderId(tokenSId);
            //long orderId = ConvertHelper.StringToLong(orderIdModel.data.orderId);
            long orderId = orderIdModel.data.orderId;

            //2、下单
            OrderRequestModelAbandoned orderRequest = new OrderRequestModelAbandoned();
            //orderRequest.exchangeId = 2;    // loopring.io交易所ID
            orderRequest.orderId = orderId;
            orderRequest.accountId = this.Account.AccountId;
            orderRequest.tokenSId = tokenSId;
            orderRequest.tokenBId = tokenBId;
            orderRequest.amountS = this.GetLoopringAmountString(marketPair, price, amount, tokenSId);
            orderRequest.amountB = this.GetLoopringAmountString(marketPair, price, amount, tokenBId);
            orderRequest.allOrNone = false;

            MarketInfoModel marketInfo = ExLoopring.MARKETS[marketPair];
            // 如果tokenSId的是基础货币，则表明是卖的
            orderRequest.buy = tokenSId == marketInfo.baseTokenId ? false : true;
            //if (tranPair.ToLower() == "LRC-ETH".ToLower())
            //    rodm.buy = tokenSId == 0 ? true : false;
            //else if (tranPair.ToLower() == "ETH-USDT".ToLower())
            //    rodm.buy = tokenSId == 0 ? false : true;
            //else if (tranPair.ToLower() == "LRC-USDT".ToLower())
            //    rodm.buy = tokenSId == 2 ? false : true;

            // 要减去 一小时？（为啥？）不然会提示valid since must less than (now-600) timestamp
            orderRequest.validSince = ConvertHelper.DateTimeToUnixTime(DateTime.Now.AddHours(-10));
            orderRequest.validUntil = ConvertHelper.DateTimeToUnixTime(DateTime.Now.AddMonths(2));
            orderRequest.maxFeeBips = 50;
            orderRequest.label = 211;

            //orderRequest.owner = this.Account.Address;
            //orderRequest.tokenS = ExLoopring.TOKENS[tokenSId].address;
            //orderRequest.tokenB = ExLoopring.TOKENS[tokenBId].address;
            //orderRequest.amountSInBN = orderRequest.amountS;
            //orderRequest.amountBInBN = orderRequest.amountB;
            //orderRequest.feeBips = 50;
            //orderRequest.rebateBips = 0;

            //string hashArgs = $"[{orderRequest.exchangeId},{orderRequest.orderId},{orderRequest.accountId},{orderRequest.tokenSId},{orderRequest.tokenBId}," +
            //      $"'{orderRequest.amountS}','{orderRequest.amountB}',{orderRequest.allOrNone.ToString().ToLower()},{orderRequest.validSince}," +
            //       $"{orderRequest.validUntil},{orderRequest.maxFeeBips},{orderRequest.buy.ToString().ToLower()},{orderRequest.label}]";

            List<object> hashArgs = new List<object>() {
                orderRequest.exchangeId ,
                orderRequest.orderId,
                orderRequest.accountId,
                orderRequest.tokenSId,
                orderRequest.tokenBId,
                orderRequest.amountS,
                orderRequest.amountB,
                orderRequest.allOrNone,
                orderRequest.validSince,
                orderRequest.validUntil,
                orderRequest.maxFeeBips,
                orderRequest.buy,
                orderRequest.label
            };

            string strSign = ExLoopring.CEF_LOOPRING_SIGN.GetSignByArgs(this.Account.SecretKey, hashArgs);
            SignatureModel signature = JsonConvert.DeserializeObject<SignatureModel>(strSign);

            orderRequest.signatureRx = signature.Rx;
            orderRequest.signatureRy = signature.Ry;
            orderRequest.signatureS = signature.s;

            //orderRequest.clientOrderId = "WitxQ";
            orderRequest.channelId = "hebao::subchannel::0001";

            return orderRequest;
        }

        /// <summary>
        /// 下一单，发送数据
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        public OrderResponseModel SendSingleOrderAbandoned(OrderRequestModel orderRequest)
        {
            OrderResponseModel orderResponse = this._httpClient.PostJson<OrderResponseModel>(JsonConvert.SerializeObject(orderRequest), null,
                new Dictionary<string, string> { { "X-API-KEY", this.XApiKey } }, this._orderUrlPath);

            return orderResponse;

        }


        #endregion

        /// <summary>
        /// 下一单，发送数据V3版本
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        public OrderResponseModelV3 SendSingleOrderV3(OrderRequestModel orderRequest)
        {
            OrderResponseModelV3 orderResponseV3 = this._httpClient.PostJson<OrderResponseModelV3>(JsonConvert.SerializeObject(orderRequest), null,
                new Dictionary<string, string> { { "X-API-KEY", this.XApiKey } }, this._orderUrlPath);

            return orderResponseV3;
        }

        /// <summary>
        /// 发送批量订单
        /// </summary>
        /// <param name="orderBatchRequest"></param>
        /// <returns></returns>
        public OrderBatchResponseModel SendBatchOrder(OrderBatchRequestModel orderBatchRequest)
        {
            OrderBatchResponseModel orderBatchResponse = this._httpClient.PostJson<OrderBatchResponseModel>(JsonConvert.SerializeObject(orderBatchRequest), null,
                new Dictionary<string, string> { { "X-API-KEY", this.XApiKey } }, this._orderBatchUrlPath);

            return orderBatchResponse;
        }

        /// <summary>
        /// 根据交易对和tokenId，获取订单使用的数量
        /// <para>
        /// 此参数的数量是“基础货币”的数量
        /// </para>
        /// </summary>
        /// <param name="marketPair">市场交易对</param>
        /// <param name="price">价格</param>
        /// <param name="amount">基础货币的数量</param>
        /// <param name="tokenId">Token Id</param>
        /// <returns></returns>
        private string GetLoopringAmountString(string marketPair, decimal price, decimal amount, int tokenId)
        {
            string strAmount = string.Empty;

            if (string.IsNullOrWhiteSpace(marketPair) || price <= 0 || amount <= 0 || tokenId < 0)
                return strAmount;

            // 获取此交易对的市场信息
            MarketInfoModel marketInfo = ExLoopring.MARKETS[marketPair];
            // 获取Token的实体
            TokenModel token = ExLoopring.TOKENS[tokenId];

            // 如果此token是基础货币，则数量直接通过“合约中定义的通证小数位”进行转换
            if (tokenId==marketInfo.baseTokenId)
            {
                strAmount = (amount * (decimal)Math.Pow(10, token.decimals)).ToString("f0");
            }
            else if(tokenId==marketInfo.quoteTokenId)
            {
                // 如果是定价货币，则amount = price*amount
                strAmount = (amount * price * (decimal)Math.Pow(10, token.decimals)).ToString("f0");
            }
            return strAmount;
        }
    }
}
