using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WitxQ.Common;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Models.Token;
using WitxQ.Exchange.Loopring.Tools;

namespace WitxQ.Exchange.Loopring.Operation
{
    /// <summary>
    /// 获取交易所支持的通证信息
    /// </summary>
    public class TokenOperation : BaseOperation
    {

        /// <summary>
        /// 
        /// </summary>
        public TokenOperation():base("/api/v2/exchange/tokens")
        {

        }

        /// <summary>
        /// 通过Api获取 交易所支持的通证信息
        /// </summary>
        public List<TokenModel> GetTokensByApi()
        {
            HttpRequestClient2 requestClient = new HttpRequestClient2(this.Url);
            List<TokenModel> tokens = new List<TokenModel>();
            bool blnIsSuccess = false;    // 是否获取数据成功
            do
            {
                try
                {
                    TokenResponseModel responseModel = requestClient.Get<TokenResponseModel>();
                    // 成功
                    if (responseModel.resultInfo.code == 0)
                    {
                        tokens = responseModel.data;
                        blnIsSuccess = true;
                        Console.WriteLine($"TokenOperation--GetTokensByApi:获取交易所支持的通证信息数据成功！");
                        ExLoopring.LOGGER.Info($"TokenOperation--GetTokensByApi:获取交易所支持的通证信息数据成功！");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"TokenOperation--GetTokensByApi:获取交易所支持的通证信息数据异常！{Environment.NewLine}errCode={responseModel.resultInfo.code},errMsg={((ReponseErrCode)responseModel.resultInfo.code).GetDescription()}");
                        ExLoopring.LOGGER.Info($"TokenOperation--GetTokensByApi:获取交易所支持的通证信息数据异常！{Environment.NewLine}errCode={responseModel.resultInfo.code},errMsg={((ReponseErrCode)responseModel.resultInfo.code).GetDescription()}");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TokenOperation--GetTokensByApi:获取交易所支持的通证信息数据异常！{Environment.NewLine}ex.message:{ex.Message},{Environment.NewLine}ex.StackTrace:{ex.StackTrace}");
                    ExLoopring.LOGGER.Error($"TokenOperation--GetTokensByApi:获取交易所支持的通证信息数据异常！", ex);
                }
                // 等200ms，由于交易所调用次数限制
                Thread.Sleep(200);
            } while (!blnIsSuccess);

            return tokens;
        }

    }
}
