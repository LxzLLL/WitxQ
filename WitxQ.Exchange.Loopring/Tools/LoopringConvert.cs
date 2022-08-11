using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WitxQ.Common;
using WitxQ.Common.CryptographyHelper;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Models.Market;
using WitxQ.Exchange.Loopring.Models.Token;

namespace WitxQ.Exchange.Loopring.Tools
{
    /// <summary>
    /// loopring的转换规则
    /// </summary>
    public class LoopringConvert
    {
        /// <summary>
        /// 从字符串获取loopring算法的小数
        /// <para>
        /// 小数位的保留使用TokenModel中的建议值，保留小数位后的数据直接截断（非四舍五入）
        /// </para>
        /// </summary>
        /// <param name="strNumber">数量的字符串</param>
        /// <param name="tokenid">token编号</param>
        /// <returns></returns>
        public static decimal GetLoopringNumber(string strNumber, int tokenid = -1)
        {
            if (string.IsNullOrWhiteSpace(strNumber) || tokenid < 0)
                return 0;

            TokenModel token = ExLoopring.TOKENS[tokenid];
            if (token == null || string.IsNullOrWhiteSpace(token.name))
                return 0;

            int strNumberLength = strNumber.Length;    // 字符串长度
            int subLength = token.decimals - token.precision;  // 要截取的长度

            // 如果数据字符串长度，小于需要截取的位数，表明此数据不足以用保留的小数位表示，即为0
            if (strNumberLength <= subLength)
                return 0;

            // 截取要保留位数后的字符串
            string str = strNumber.Substring(0, strNumberLength - subLength);

            return ConvertHelper.StringToDecimal(str, 0) / (decimal)Math.Pow(10, token.precision);
        }

        /// <summary>
        /// 订单薄的深度数据转换
        /// </summary>
        /// <param name="depthData">深度数据列表，bids或asks</param>
        /// <param name="marketPair">市场交易对</param>
        /// <returns></returns>
        public static List<List<decimal>> GetOrderBookConvert(List<List<string>> depthData,string marketPair)
        {
            List<List<decimal>> depthDataResult = new List<List<decimal>>();
            if(depthData!=null && depthData.Count>0 && !string.IsNullOrWhiteSpace(marketPair))
            {
                // 获取市场信息实体
                MarketInfoModel marketInfo = ExLoopring.MARKETS[marketPair];
                int baseTokenId = marketInfo.baseTokenId;  // 基础货币ID（此交易对的 交易token）
                int quoteTokenId = marketInfo.quoteTokenId;  // 定价货币（此交易对的 计价token）

                for(int i=0;i< depthData.Count;i++)
                {
                    //List<decimal> data = new List<decimal>(4);
                    decimal[] data = new decimal[4];

                    List<string> dataTemp = depthData[i];

                    data[0] = ConvertHelper.StringToDecimal(dataTemp[0]);  // 价格
                    data[1] = LoopringConvert.GetLoopringNumber(dataTemp[1], baseTokenId);  // 数量（基础通证的数量）
                    data[2] = LoopringConvert.GetLoopringNumber(dataTemp[2], quoteTokenId);  // 成交额（ 计价通证的数量）
                    data[3] = ConvertHelper.StringToDecimal(dataTemp[3]);  // 聚合的订单数目

                    depthDataResult.Add(data.ToList());
                }
            }

            return depthDataResult;
        }


        /******************************************
        签名生成算法
            1、初始化空字符串signatureBase；
            2、将API请求的HTTP方法字符串追加到signatureBase；
            3、将“＆”字符附加到signatureBase；
            4、将百分号编码后（percent-encoded）后的完整URL路径（不包括“?”和查询参数）追加到signatureBase；
            5、将“＆”字符附加到signatureBase；
            6、初始化空字符串parameterString；
            7、对于GET / DELETE 请求：
            8、将请求里的参数按键的字典顺序升序排序，得到排过序后的键/值对；
            9、将百分号编码后后的键附加到parameterString；
            10、将“=”字符附加到parameterString；
            11、将百分号编码后后的值附加到parameterString；
            12、如果有更多的键/值对，请在parameterString后面附加“＆”字符，并重复上述操作；
            13、对于POST / PUT 请求；
            14、将发送请求的Body JSON字符串附加到parameterString；
            15、将百分号编码后后的parameterString附加到signatureBase；
            16、计算signatureBase的SHA-256哈希值hash；
            17、对hash用账号的私钥privateKey做签名，得到三个值：Rx,Ry, 和S；
            18、将Rx,Ry, 和S通过逗号分隔拼接成最终签名字符串：${Rx},${Ry},${S}。
        ****************************************/


        /// <summary>
        /// 获取通用的API请求签名，X-API-SIG
        /// </summary>
        /// <param name="method">请求的方法名称，全部大写（例如：GET，POST，PUT，DELETE）</param>
        /// <param name="url">请求的url（URL中请一定包含HTTPS协议头，确保协议头和接入URL全部小写，例如：https://api.loopring.io/api/v2/apiKey）</param>
        /// <param name="dictParams">URL包含的Query参数</param>
        /// <param name="privateKey">用户的私钥</param>
        /// <returns></returns>
        public static string GetApiSign(string method,string url,IDictionary<string,string> dictParams,string privateKey)
        {
            string strSign = string.Empty;

            StringBuilder signatureBase = new StringBuilder() ;  // 原始的签名字符串

            if (string.IsNullOrWhiteSpace(method) || string.IsNullOrWhiteSpace(url) || dictParams == null)
                return strSign.ToString();

            // 1 附加API的方法
            signatureBase.Append(method + "&");

            // 2 附加url（百分号编码的url encoded）
            signatureBase.Append(WebUtility.UrlEncode(url) + "&");

            // 3 附加Query参数
            string parameterString = string.Empty;

            // 3.1 把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(dictParams);

            // 注意使用ASCII排序
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams
                .OrderBy(x => x.Key, new OrdinalComparer())
                .ToDictionary(x => x.Key, y => y.Value)
                .GetEnumerator();

            // 3.2 把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    // 将百分号编码后后的键和值 附加到parameterString
                    query.Append(WebUtility.UrlEncode(key)).Append("=").Append(WebUtility.UrlEncode(value)).Append("&");
                }
            }
            parameterString = query.ToString().Substring(0, query.Length - 1);

            // 3.3 将百分号编码后后的parameterString附加到signatureBase
            signatureBase.Append(WebUtility.UrlEncode(parameterString));

            // 4 计算signatureBase的SHA-256哈希值hash；
            string strHash = ShaHelper.Sha256EncryptToString(signatureBase.ToString());

            // 5 获取签名对象
            string strJsonSign = ExLoopring.CEF_LOOPRING_SIGN.GetSign(privateKey, LoopringConvert.AddHexPrefix(strHash));
            SignatureModel signature = JsonConvert.DeserializeObject<SignatureModel>(strJsonSign);

            strSign = $"{signature.Rx},{signature.Ry},{signature.s}";
            return strSign;
        }

        /// <summary>
        /// 添加16进制头
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string AddHexPrefix(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.StartsWith("0x") ? input : "0x" + input;
            }
            else
                return input;
        }
    }

    /// <summary>
    /// ASCII值排序
    /// </summary>
    public class OrdinalComparer : System.Collections.Generic.IComparer<String>
    {
        public int Compare(String x, String y)
        {
            return string.CompareOrdinal(x, y);
        }
    }
}
