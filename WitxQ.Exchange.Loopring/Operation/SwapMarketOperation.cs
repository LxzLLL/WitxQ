using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WitxQ.Common;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Models.SwapMarket;
using WitxQ.Exchange.Loopring.Tools;
using WitxQ.Interface.Spot;
using WitxQ.Model.Markets;

namespace WitxQ.Exchange.Loopring.Operation
{
    /// <summary>
    /// 获取交易所支持的Swap市场
    /// </summary>
    public class SwapMarketOperation : BaseOperation, ISwapMarket
    {

        /// <summary>
        /// 获取swap的market的Path
        /// </summary>
        //private readonly string _ammMarketPath = "/api/v2/amm/markets";

        /// <summary>
        /// amm的市场信息
        /// </summary>
        //private List<AmmMarketModel> _ammMarketModels;

        /// <summary>
        /// http请求客户端
        /// </summary>
        //private HttpRequestClient2 _httpClient;

        public SwapMarketOperation() : base("/api/v2/amm/markets")
        {
            
        }

        #region ISwapMarket  返回通用格式的swapmarket
        /// <summary>
        /// 通过交易对字符串，获取交易对信息
        /// </summary>
        /// <param name="swapMarketPairName">交易对，全部为中间“-”连字符的大写形式，例如AMM-LRC-USDT</param>
        /// <returns></returns>
        public SwapMarketPairModel GetSwapMarketPairModel(string swapMarketPairName)
        {
            if (ExLoopring.SWAP_MARKETS == null || ExLoopring.SWAP_MARKETS.Count <= 0)
                return null;

            AmmMarketModel model = ExLoopring.SWAP_MARKETS[swapMarketPairName];

            SwapMarketPairModel pairModel = new SwapMarketPairModel();
            pairModel.Name = model.name;
            pairModel.Market = model.market;
            pairModel.Address = model.address;
            pairModel.BaseToken = ExLoopring.TOKENS[model.PoolBaseTokenId].name;
            pairModel.QuoteToken = ExLoopring.TOKENS[model.PoolQuoteTokenId].name;
            pairModel.FeeBips = model.feeBips;
            pairModel.PricePrecision = model.pricePrecision;
            pairModel.AmountPrecision = model.amountPrecision;

            return pairModel;
        }
        #endregion 



        /// <summary>
        /// 获取amm的市场信息
        /// </summary>
        /// <returns></returns>
        public List<AmmMarketModel> GetAmmMarketsByApi()
        {
            bool blnIsSuccess = false;    // 是否获取数据成功
            HttpRequestClient2 requestClient = new HttpRequestClient2(this.Url);

            List<AmmMarketModel> ammMarkets = new List<AmmMarketModel>();
            do
            {
                try
                {
                    AmmMarketResponseModel ammMarketResponse = requestClient.Get<AmmMarketResponseModel>();

                    // 成功
                    if (ammMarketResponse.resultInfo.code == 0)
                    {
                        ammMarkets = ammMarketResponse.data;
                        blnIsSuccess = true;
                        Console.WriteLine($"SwapMarketOperation--GetAmmMarketsByApi:获取交易所支持的amm市场信息数据成功！");
                        ExLoopring.LOGGER.Info($"SwapMarketOperation--GetAmmMarketsByApi:获取交易所支持的amm市场信息数据成功！");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"SwapMarketOperation--GetAmmMarketsByApi:获取交易所支持的amm市场信息数据异常！{Environment.NewLine}errCode={ammMarketResponse.resultInfo.code},errMsg={((ReponseErrCode)ammMarketResponse.resultInfo.code).GetDescription()}");
                        ExLoopring.LOGGER.Info($"SwapMarketOperation--GetAmmMarketsByApi:获取交易所支持的amm市场信息数据异常！{Environment.NewLine}errCode={ammMarketResponse.resultInfo.code},errMsg={((ReponseErrCode)ammMarketResponse.resultInfo.code).GetDescription()}");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SwapMarketOperation--GetAmmMarketsByApi:获取交易所支持的amm市场信息数据异常！{Environment.NewLine}ex.message:{ex.Message},{Environment.NewLine}ex.StackTrace:{ex.StackTrace}");
                    ExLoopring.LOGGER.Error($"SwapMarketOperation--GetAmmMarketsByApi:获取交易所支持的amm市场信息数据异常！", ex);
                }
                // 等200ms，由于交易所调用次数限制
                Thread.Sleep(300);
                //Task.Delay(200).Wait();
            } while (!blnIsSuccess);

            return ammMarkets;
        }
    }
}
