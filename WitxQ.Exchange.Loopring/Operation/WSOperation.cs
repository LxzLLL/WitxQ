using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WitxQ.Common;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Models.WS;
using WitxQ.Exchange.Loopring.Tools;

namespace WitxQ.Exchange.Loopring.Operation
{
    /// <summary>
    /// websocket连接前的REST API操作
    /// </summary>
    public class WSOperation : BaseOperation
    {
        public WSOperation(AccountModel account) : base("/v2/ws/key")
        {
            this.Account = account;
            this.XApiKey = this.Account.ApiKey;
        }

        /// <summary>
        /// 通过Api获取 ws的apikey
        /// </summary>
        public string GetWSApiKeyByApi()
        {
            HttpRequestClient2 requestClient = new HttpRequestClient2(this.Url);
            string strWSApiKey = string.Empty;
            bool blnIsSuccess = false;    // 是否获取数据成功
            do
            {
                try
                {
                    //var requestModel = new { accountId = this.Account.AccountId, publicKeyX = this.Account.PublicKeyX, publicKeyY = this.Account.PublicKeyY };
                    Dictionary<string, string> requestModel = new Dictionary<string, string>
                    {
                       { "accountId",this.Account.AccountId.ToString()},
                       { "publicKeyX",this.Account.PublicKeyX},
                       { "publicKeyY",this.Account.PublicKeyY}
                    };


                    // 签名
                    string strXApiSign = LoopringConvert.GetApiSign("GET", this.DomainUrl + this.UrlPath,
                        requestModel, this.Account.SecretKey);


                    WSApiKeyResponseModel responseModel = requestClient.Get<WSApiKeyResponseModel>(
                        new { accountId = this.Account.AccountId, publicKeyX = this.Account.PublicKeyX, publicKeyY = this.Account.PublicKeyY },
                        new Dictionary<string, string>
                        {
                            { "X-API-SIG",strXApiSign }
                        });

                    // 成功
                    if (responseModel.resultInfo.code == 0)
                    {
                        strWSApiKey = responseModel.data;
                        blnIsSuccess = true;
                        Console.WriteLine($"WSOperation--GetWSApiKeyByApi:获取WSApiKey数据成功！");
                        ExLoopring.LOGGER.Info($"WSOperation--GetWSApiKeyByApi:获取WSApiKey数据成功！");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"WSOperation--GetWSApiKeyByApi:获取WSApiKey数据异常！{Environment.NewLine}errCode={responseModel.resultInfo.code},errMsg={((ReponseErrCode)responseModel.resultInfo.code).GetDescription()}");
                        ExLoopring.LOGGER.Info($"WSOperation--GetWSApiKeyByApi:获取WSApiKey数据异常！{Environment.NewLine}errCode={responseModel.resultInfo.code},errMsg={((ReponseErrCode)responseModel.resultInfo.code).GetDescription()}");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WSOperation--GetWSApiKeyByApi:获取WSApiKey数据异常！{Environment.NewLine}ex.message:{ex.Message},{Environment.NewLine}ex.StackTrace:{ex.StackTrace}");
                    ExLoopring.LOGGER.Error($"WSOperation--GetWSApiKeyByApi:获取WSApiKey数据异常！", ex);
                }
                // 等500ms，由于交易所调用次数限制
                Thread.Sleep(500);
            } while (!blnIsSuccess);

            return strWSApiKey;
        }

    }
}
