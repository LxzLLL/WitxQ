using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Sys;

namespace WitxQ.Exchange.Loopring.Operation
{
    /// <summary>
    /// 操作基类
    /// </summary>
    public class BaseOperation
    {
        /// <summary>
        /// 访问路径
        /// </summary>
        public string UrlPath { get; protected set; }

        /// <summary>
        /// X-API-KEY
        /// </summary>
        public string XApiKey { get; protected set; }

        /// <summary>
        /// 访问的基础url
        /// </summary>
        public string DomainUrl { get; protected set; }

        /// <summary>
        /// 访问的完整url
        /// </summary>
        public string Url { get; protected set; }

        /// <summary>
        /// 账号信息
        /// </summary>
        public AccountModel Account { get; protected set; }

        /// <summary>
        /// BaseOperation构造
        /// </summary>
        /// <param name="urlPath">api路径</param>
        public BaseOperation(string urlPath)
        {
            this.DomainUrl = ExLoopring.API_URL;
            this.UrlPath = urlPath;
            this.Url = this.DomainUrl + this.UrlPath;
        }

    }
}
