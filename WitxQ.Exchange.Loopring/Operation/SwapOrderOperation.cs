using System;
using System.Linq;
using System.Collections.Generic;
using WitxQ.Common;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Tools;
using WitxQ.Interface.Spot;
using Newtonsoft.Json;

using WitxQ.Exchange.Loopring.Models.Market;
using WitxQ.Exchange.Loopring.Models.Order;
using WitxQ.Exchange.Loopring.Models.Token;
using WitxQ.Exchange.Loopring.Models.SwapOrder;

namespace WitxQ.Exchange.Loopring.Operation
{
    /// <summary>
    /// amm  swap  相关操作类
    /// </summary>
    public class SwapOrderOperation : BaseOperation, ISwapOrder
    {

        /// <summary>
        /// 获取amm ordderID的Path
        /// </summary>
        private readonly string _ammOrderIdUrlPath = "/api/v2/storageId";

        /// <summary>
        /// 提交Order的Path
        /// </summary>
        private readonly string _ammOrderUrlPath = "/api/v2/order";

        /// <summary>
        /// http请求客户端
        /// </summary>
        private readonly HttpRequestClient2 _httpClient;


        /// <summary>
        /// 查询用户交易所余额 的构造
        /// </summary>
        /// <param name="account">账号信息</param>
        public SwapOrderOperation(AccountModel account) : base("")
        {
            this.Account = account;
            this.XApiKey = this.Account.ApiKey;
            this._httpClient = new HttpRequestClient2(this.DomainUrl);  // 使用 基础Url初始化，由各个操作设置path路径
        }


        #region  ISwapOrder

        /// <summary>
        /// 买类型订单
        /// <para>
        /// 例如：LRC-ETH，买LRC，卖ETH，则 sellToken为ETH，buyToken为LRC
        /// </para>
        /// </summary>
        /// <param name="ammMarket">swap市场，交易对，全部为中间“-”连字符的大写形式,例如：LRC-ETH</param>
        /// <param name="price">价格</param>
        /// <param name="amount">数量，指此交易对的交易数量，也即是BaseToken的数量</param>
        /// <param name="sellToken">出售的代币符号，例如LRC</param>
        /// <param name="buyToken">买入的代币符号，例如ETH</param>
        /// <param name="orderNumber">返回的订单标识</param>
        /// <param name="err">异常信息</param>
        /// <returns></returns>
        public bool SwapOrderBuy(string ammMarket, string price, string amount, string sellToken, string buyToken, ref string orderNumber, ref string err)
        {
            return this.Order(ammMarket, price, amount, sellToken, buyToken, ref orderNumber, ref err);
        }

        /// <summary>
        /// 卖类型订单
        /// <para>
        /// 例如:LRC-ETH，卖LRC，买ETH，则 sellToken为LRC，buyToken为ETH
        /// </para>
        /// </summary>
        /// <param name="ammMarket">swap市场，交易对，全部为中间“-”连字符的大写形式,例如：LRC-ETH</param>
        /// <param name="price">价格</param>
        /// <param name="amount">数量，指此交易对的交易数量，也即是BaseToken的数量</param>
        /// <param name="sellToken">出售的代币符号，例如LRC</param>
        /// <param name="buyToken">买入的代币符号，例如ETH</param>
        /// <param name="orderNumber">返回的订单标识</param>
        /// <param name="err">异常信息</param>
        /// <returns></returns>
        public bool SwapOrderSell(string ammMarket, string price, string amount, string sellToken, string buyToken, ref string orderNumber, ref string err)
        {
            return this.Order(ammMarket, price, amount, sellToken, buyToken, ref orderNumber, ref err);
        }

        #endregion


        /// <summary>
        /// 订单处理
        /// </summary>
        /// <param name="ammMarket">市场，交易对，全部为中间“-”连字符的大写形式,例如：LRC-ETH</param>
        /// <param name="price">价格</param>
        /// <param name="amount">数量</param>
        /// <param name="sellToken">出售的代币符号，例如LRC</param>
        /// <param name="buyToken">买入的代币符号，例如ETH</param>
        /// <param name="orderNumber">返回的订单标识</param>
        /// <param name="err">异常信息</param>
        /// <returns></returns>
        private bool Order(string ammMarket, string price, string amount, string sellToken, string buyToken, ref string orderNumber, ref string err)
        {
            bool blnIsSuccess = false;

            decimal dprice = ConvertHelper.StringToDecimal(price);
            decimal damount = ConvertHelper.StringToDecimal(amount);
            try
            {
                int tokenSId = ExLoopring.TOKENS.Values.First(token => token.symbol.Equals(sellToken)).tokenId;
                int tokenBId = ExLoopring.TOKENS.Values.First(token => token.symbol.Equals(buyToken)).tokenId;

                string marketPair = ammMarket.Substring(4);  // 去掉ammMarket的“AMM-”，例如AMM-LRC-USDT----->LRC-USDT
                AmmOrderRequestModel ammOrderRequest = this.GetOrderRequest(marketPair, dprice, damount, tokenSId, tokenBId);
                //OrderResponseModel orderResponse = this.SendSingleOrder(orderRequest);
                AmmOrderResponseModelV3 ammOrderResponseV3 = this.SendSingleOrderV3(ammOrderRequest);

                if (ammOrderResponseV3 != null)
                {
                    if (ammOrderResponseV3.resultInfo.code == (int)ReponseErrCode.Success)
                    {
                        blnIsSuccess = true;
                        orderNumber = ammOrderResponseV3.data.orderHash;
                    }
                    else
                        err = $"IOrder--Order(sid:{sellToken},bid:{buyToken})：ErrCode:{ammOrderResponseV3.resultInfo.code},ErrMsg:{ammOrderResponseV3.resultInfo.message}";
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
        public AmmOrderIdResponseModel GetOrderId(int tokenSId)
        {
            AmmOrderIdRequestModel ammOrderIdRequest = new AmmOrderIdRequestModel();
            ammOrderIdRequest.accountId = this.Account.AccountId;
            ammOrderIdRequest.tokenSId = tokenSId;

            AmmOrderIdResponseModel ammOrderIdResponse = this._httpClient.Get<AmmOrderIdResponseModel>(ammOrderIdRequest,
                new Dictionary<string, string> { { "X-API-KEY", this.XApiKey } }, this._ammOrderIdUrlPath);

            return ammOrderIdResponse;
        }

        /// <summary>
        /// 组装订单请求实体（已包括 获取订单ID）
        /// <para>
        /// 版本2的api参数有问题，截止2020年5月26日13:25:31
        /// </para>
        /// </summary>
        /// <param name="marketPair">交易对,例如：LRC-USDT，注意此处不是amm的市场，需要</param>
        /// <param name="price">价格</param>
        /// <param name="amount">总量</param>
        /// <param name="tokenSId">要卖的TokenId</param>
        /// <param name="tokenBId">要买的TokenId</param>
        /// <returns></returns>
        public AmmOrderRequestModel GetOrderRequest(string marketPair, decimal price, decimal amount, int tokenSId, int tokenBId)
        {
            //1、获取单号
            AmmOrderIdResponseModel orderIdModel = this.GetOrderId(tokenSId);
            //long orderId = ConvertHelper.StringToLong(orderIdModel.data.orderId);
            long orderId = orderIdModel.data.orderId;

            //2、下单
            AmmOrderRequestModel ammOrderRequest = new AmmOrderRequestModel();
            //orderRequest.exchangeId = 2;    // loopring.io交易所ID
            ammOrderRequest.orderId = orderId;
            ammOrderRequest.storageId = orderId;
            ammOrderRequest.orderType = "AMM";
            ammOrderRequest.accountId = this.Account.AccountId;
            ammOrderRequest.tokenSId = tokenSId;
            ammOrderRequest.tokenBId = tokenBId;
            ammOrderRequest.amountS = this.GetLoopringAMMAmountString(marketPair, price, amount, tokenSId);
            ammOrderRequest.amountB = this.GetLoopringAMMAmountString(marketPair, price, amount, tokenBId);
            ammOrderRequest.allOrNone = false;

            //MarketInfoModel marketInfo = ExLoopring.MARKETS[marketPair];
            // 如果tokenSId的是基础货币，则表明是卖的，如果是AMM的话  一直为false
            //orderRequest.buy = tokenSId == marketInfo.baseTokenId ? false : true;
            ammOrderRequest.fillAmountBOrS = false;


            // 要减去 一小时？（为啥？）不然会提示valid since must less than (now-600) timestamp
            //orderRequest.validSince = ConvertHelper.DateTimeToUnixTime(DateTime.Now.AddHours(-10));
            ammOrderRequest.validUntil = ConvertHelper.DateTimeToUnixTime(DateTime.Now.AddMonths(2));
            ammOrderRequest.maxFeeBips = 50;
            ammOrderRequest.label = 211;

            ammOrderRequest.owner = this.Account.Address;
            ammOrderRequest.tokenS = ExLoopring.TOKENS[tokenSId].address;
            ammOrderRequest.tokenB = ExLoopring.TOKENS[tokenBId].address;
            ammOrderRequest.amountSInBN = ammOrderRequest.amountS;
            ammOrderRequest.amountBInBN = ammOrderRequest.amountB;
            ammOrderRequest.feeBips = 50;
            ammOrderRequest.poolAddress = ExLoopring.SWAP_MARKETS[$"AMM-{marketPair}"].address;
            //orderRequest.rebateBips = 0;

            //string hashArgs = $"[{orderRequest.exchangeId},{orderRequest.orderId},{orderRequest.accountId},{orderRequest.tokenSId},{orderRequest.tokenBId}," +
            //      $"'{orderRequest.amountS}','{orderRequest.amountB}',{orderRequest.allOrNone.ToString().ToLower()},{orderRequest.validSince}," +
            //       $"{orderRequest.validUntil},{orderRequest.maxFeeBips},{orderRequest.buy.ToString().ToLower()},{orderRequest.label}]";

            List<object> hashArgs = new List<object>() {
                ammOrderRequest.exchange,
                //orderRequest.orderId,
                ammOrderRequest.storageId,
                ammOrderRequest.accountId,
                ammOrderRequest.tokenSId,
                ammOrderRequest.tokenBId,
                ammOrderRequest.amountS,
                ammOrderRequest.amountB,
                //orderRequest.allOrNone,
                //orderRequest.validSince,
                ammOrderRequest.validUntil,
                ammOrderRequest.maxFeeBips,
                //orderRequest.buy,
                ammOrderRequest.fillAmountBOrS?1:0
                //orderRequest.label
                //ammOrderRequest.taker
            };

            string strSign = ExLoopring.CEF_LOOPRING_SIGN.GetSignByArgs(this.Account.SecretKey, hashArgs);
            SignatureModel signature = JsonConvert.DeserializeObject<SignatureModel>(strSign);

            // 0x + 192 bytes hex string, i.e., 0x+Rx+Ry+s
            string hexRx = ConvertHelper.BaseConvert(signature.Rx, ConvertHelper.CHS_STR10, ConvertHelper.CHS_STR16).PadLeft(64, '0');  // 在最前端用'0'补足64位
            string hexRy = ConvertHelper.BaseConvert(signature.Ry, ConvertHelper.CHS_STR10, ConvertHelper.CHS_STR16).PadLeft(64, '0');  // 在最前端用'0'补足64位
            string hexS = ConvertHelper.BaseConvert(signature.s, ConvertHelper.CHS_STR10, ConvertHelper.CHS_STR16).PadLeft(64, '0');  // 在最前端用'0'补足64位
            ammOrderRequest.eddsaSig = $"0x{hexRx + hexRy + hexS}";

            //orderRequest.signatureRx = signature.Rx;
            //orderRequest.signatureRy = signature.Ry;
            //orderRequest.signatureS = signature.s;

            //orderRequest.clientOrderId = "WitxQ";
            ammOrderRequest.channelId = "hebao::subchannel::0001";

            return ammOrderRequest;
        }

        /// <summary>
        /// 下一单，发送数据V3版本
        /// </summary>
        /// <param name="ammOrderRequest"></param>
        /// <returns></returns>
        public AmmOrderResponseModelV3 SendSingleOrderV3(AmmOrderRequestModel ammOrderRequest)
        {
            AmmOrderResponseModelV3 orderResponseV3 = this._httpClient.PostJson<AmmOrderResponseModelV3>(JsonConvert.SerializeObject(ammOrderRequest), null,
                new Dictionary<string, string> { { "X-API-KEY", this.XApiKey } }, this._ammOrderUrlPath);

            return orderResponseV3;
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
        //private string GetLoopringAmountString(string marketPair, decimal price, decimal amount, int tokenId)
        //{
        //    string strAmount = string.Empty;

        //    if (string.IsNullOrWhiteSpace(marketPair) || price <= 0 || amount <= 0 || tokenId < 0)
        //        return strAmount;

        //    // 获取此交易对的市场信息
        //    MarketInfoModel marketInfo = ExLoopring.MARKETS[marketPair];
        //    // 获取Token的实体
        //    TokenModel token = ExLoopring.TOKENS[tokenId];

        //    // 如果此token是基础货币，则数量直接通过“合约中定义的通证小数位”进行转换
        //    if (tokenId == marketInfo.baseTokenId)
        //    {
        //        strAmount = (amount * (decimal)Math.Pow(10, token.decimals)).ToString("f0");
        //    }
        //    else if (tokenId == marketInfo.quoteTokenId)
        //    {
        //        // 如果是定价货币，则amount = price*amount
        //        strAmount = (amount * price * (decimal)Math.Pow(10, token.decimals)).ToString("f0");
        //    }
        //    return strAmount;
        //}

        /// <summary>
        /// 根据交易对和tokenId，获取AMM订单使用的数量
        /// <para>
        /// 此参数的数量是“基础货币”的数量
        /// </para>
        /// </summary>
        /// <param name="marketPair">市场交易对</param>
        /// <param name="price">价格</param>
        /// <param name="amount">基础货币的数量</param>
        /// <param name="tokenId">Token Id</param>
        /// <returns></returns>
        private string GetLoopringAMMAmountString(string marketPair, decimal price, decimal amount, int tokenId)
        {
            string strAmount = string.Empty;
            string ammMarketPair = "AMM-" + marketPair;

            if (string.IsNullOrWhiteSpace(ammMarketPair) || price <= 0 || amount <= 0 || tokenId < 0)
                return strAmount;

            // 获取此交易对的市场信息
            var marketInfo = ExLoopring.SWAP_MARKETS[ammMarketPair];
            // 获取Token的实体
            TokenModel token = ExLoopring.TOKENS[tokenId];

            // 如果此token是基础货币，则数量直接通过“合约中定义的通证小数位”进行转换
            if (tokenId == marketInfo.PoolBaseTokenId)
            {
                strAmount = (amount * (decimal)Math.Pow(10, token.decimals)).ToString("f0");
            }
            else if (tokenId == marketInfo.PoolQuoteTokenId)
            {
                // 如果是定价货币，则amount = price*amount
                strAmount = (amount * price * (decimal)Math.Pow(10, token.decimals)).ToString("f0");
            }
            return strAmount;
        }

    }
}
