using System;
using System.Collections.Generic;
using System.Threading;
using WitxQ.Common;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Models.Market;
using WitxQ.Exchange.Loopring.Tools;

namespace WitxQ.Exchange.Loopring.Operation
{
    /// <summary>
    /// 获取交易所支持的市场
    /// </summary>
    public class MarketOperation : BaseOperation
    {

        public MarketOperation() : base("/api/v2/exchange/markets")
        {

        }


        /// <summary>
        /// 通过Api获取 交易所支持的市场信息
        /// </summary>
        public List<MarketInfoModel> GetMarketsByApi()
        {
            HttpRequestClient2 requestClient = new HttpRequestClient2(this.Url);
            List<MarketInfoModel> tokens = new List<MarketInfoModel>();
            bool blnIsSuccess = false;    // 是否获取数据成功
            do
            {
                try
                {
                    MarketInfoResponseModel responseModel = requestClient.Get<MarketInfoResponseModel>();
                    // 成功
                    if (responseModel.resultInfo.code == 0)
                    {
                        tokens = responseModel.data;
                        blnIsSuccess = true;
                        Console.WriteLine($"MarketOperation--GetMarketsByApi:获取交易所支持的市场信息数据成功！");
                        ExLoopring.LOGGER.Info($"MarketOperation--GetMarketsByApi:获取交易所支持的市场信息数据成功！");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"MarketOperation--GetMarketsByApi:获取交易所支持的市场信息数据异常！{Environment.NewLine}errCode={responseModel.resultInfo.code},errMsg={((ReponseErrCode)responseModel.resultInfo.code).GetDescription()}");
                        ExLoopring.LOGGER.Info($"MarketOperation--GetMarketsByApi:获取交易所支持的市场信息数据异常！{Environment.NewLine}errCode={responseModel.resultInfo.code},errMsg={((ReponseErrCode)responseModel.resultInfo.code).GetDescription()}");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"MarketOperation--GetMarketsByApi:获取交易所支持的市场信息数据异常！{Environment.NewLine}ex.message:{ex.Message},{Environment.NewLine}ex.StackTrace:{ex.StackTrace}");
                    ExLoopring.LOGGER.Error($"MarketOperation--GetMarketsByApi:获取交易所支持的市场信息数据异常！", ex);
                }
                // 等200ms，由于交易所调用次数限制
                Thread.Sleep(200);
                //Task.Delay(200).Wait();
            } while (!blnIsSuccess);

            return tokens;
        }

    }
}
