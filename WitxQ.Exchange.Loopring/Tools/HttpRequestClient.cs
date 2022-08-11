using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WitxQ.Exchange.Loopring.Tools
{
    /// <summary>
    /// http请求客户端
    /// </summary>
    public class HttpRequestClient
    {
        /// <summary>
        /// Rest Api 请求客户端
        /// </summary>
        private RestClient _client;

        /// <summary>
        /// 请求超时时间，15s
        /// </summary>
        private int _timeout = 15000;

        /// <summary>
        /// 构造
        /// </summary>
        public HttpRequestClient(string url)
        {
            this._client = new RestClient(url);

            this.SetBaseHeaders();
            // 设置最大连接数
            ServicePointManager.DefaultConnectionLimit = Int32.MaxValue;
        }

        /// <summary>
        /// 设置基本请求头，适用于所有request
        /// </summary>
        private void SetBaseHeaders()
        {
            this._client.AddDefaultHeader("Connection", "keep-alive");
            this._client.AddDefaultHeader("Accept", "application/json, text/plain, */*");
            this._client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            this._client.AddDefaultHeader("Accept-Language", "zh,en;q=0.9");
            this._client.Timeout = this._timeout;   // 超时 15s
            this._client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36";
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T">返回序列化对象</typeparam>
        /// <param name="queryParams">Query参数对象，会自动解析实体</param>
        /// <param name="headers">请求头信息</param>
        /// <param name="urlPath">url路径（在HttpRequestClient初始化的url为基础url无path时使用）</param>
        /// <returns></returns>
        public T Get<T>(object queryParams = null, Dictionary<string, string> headers = null ,string urlPath = "")
        {
            T t = default(T);
            var request = string.IsNullOrWhiteSpace(urlPath)? new RestRequest(Method.GET): new RestRequest(urlPath,Method.GET);
            // 添加Query参数
            if (queryParams != null)
                request.AddObject(queryParams);

            // 添加本次请求的headers
            if (headers != null && headers.Count > 0)
                request.AddHeaders(headers);

            IRestResponse<T> response = this._client.Execute<T>(request);
            t = response.Data;

            if (t == null)
                throw new Exception($"StatusCode:{response.StatusCode},StatusDescription:{response.StatusDescription},ErrorMessage:{response.ErrorMessage}", response.ErrorException);

            return t;
        }

        /// <summary>
        /// Post请求（请求ContentType：application/json）
        /// </summary>
        /// <typeparam name="T">返回序列化对象</typeparam>
        /// <param name="jsonBodyParam">请求体中的json字符串</param>
        /// <param name="queryParams">Query参数对象，会自动解析实体</param>
        /// <param name="headers">请求头信息</param>
        /// <param name="urlPath">url路径（在HttpRequestClient初始化的url为基础url无path时使用）</param>
        /// <returns></returns>
        public T PostJson<T>(string jsonBodyParam = null, object queryParams = null, Dictionary<string, string> headers = null, string urlPath = "")
        {
            T t = default(T);
            var request = string.IsNullOrWhiteSpace(urlPath) ? new RestRequest(Method.POST) : new RestRequest(urlPath, Method.POST);
            // 添加Query参数
            if (queryParams != null)
                request.AddObject(queryParams);

            // 添加本次请求的headers
            if (headers != null && headers.Count > 0)
                request.AddHeaders(headers);

            // 以application/json请求
            request.AddHeader("ContentType", "application/json");

            if (!string.IsNullOrWhiteSpace(jsonBodyParam))
                request.AddParameter("application/json;charset=utf-8", jsonBodyParam, ParameterType.RequestBody);

            IRestResponse<T> response = this._client.Execute<T>(request);
            t = response.Data;

            if (t == null)
                throw new Exception($"StatusCode:{response.StatusCode},StatusDescription:{response.StatusDescription},ErrorMessage:{response.ErrorMessage}", response.ErrorException);

            return t;
        }

        /// <summary>
        /// Post请求（请求ContentType：application/json）
        /// </summary>
        /// <typeparam name="T">返回序列化对象</typeparam>
        /// <param name="jsonBodyParam">请求体中的json字符串</param>
        /// <param name="queryParams">Query参数对象，会自动解析实体</param>
        /// <param name="headers">请求头信息</param>
        /// <param name="urlPath">url路径（在HttpRequestClient初始化的url为基础url无path时使用）</param>
        /// <returns></returns>
        public T DeleteJson<T>(string jsonBodyParam = null, object queryParams = null, Dictionary<string, string> headers = null, string urlPath = "")
        {
            T t = default(T);
            var request = string.IsNullOrWhiteSpace(urlPath) ? new RestRequest(Method.DELETE) : new RestRequest(urlPath, Method.DELETE);
            // 添加Query参数
            if (queryParams != null)
                request.AddObject(queryParams);

            // 添加本次请求的headers
            if (headers != null && headers.Count > 0)
                request.AddHeaders(headers);

            // 以application/json请求
            request.AddHeader("ContentType", "application/json");

            if (!string.IsNullOrWhiteSpace(jsonBodyParam))
                request.AddParameter("application/json;charset=utf-8", jsonBodyParam, ParameterType.RequestBody);

            IRestResponse<T> response = this._client.Execute<T>(request);
            t = response.Data;

            if (t == null)
                throw new Exception($"StatusCode:{response.StatusCode},StatusDescription:{response.StatusDescription},ErrorMessage:{response.ErrorMessage}", response.ErrorException);

            return t;
        }
    }
}
