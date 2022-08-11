using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WitxQ.Interface.Loopring
{
    /// <summary>
    /// 基于loopring的中继签名
    /// </summary>
    public interface ISign
    {
        /// <summary>
        /// 获取Hash
        /// </summary>
        /// <param name="args">参与生成hash的参数</param>
        /// <returns></returns>
        //public string GetHash(object args);

        /// <summary>
        /// 通过arg获取Signature
        /// <para>
        /// 集成了生成hash，不需要再调用GetHash
        /// </para>
        /// </summary>
        /// <param name="secretKey">密钥</param>
        /// <param name="args">参与生成hash的参数</param>
        /// <returns></returns>
        //public string GetSignByArgs(string secretKey, string args);

        /// <summary>
        /// 获取Hash
        /// </summary>
        /// <param name="args">参与生成hash的参数列表</param>
        /// <returns></returns>
        public string GetHash(List<Object> args);

        /// <summary>
        /// 通过arg获取Signature
        /// <para>
        /// 集成了生成hash，不需要再调用GetHash
        /// </para>
        /// </summary>
        /// <param name="secretKey">密钥</param>
        /// <param name="args">参与生成hash的参数列表</param>
        /// <returns></returns>
        public string GetSignByArgs(string secretKey, List<Object> args);


        /// <summary>
        /// 通过hash获取Signature
        /// </summary>
        /// <param name="secretKey">密钥</param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public string GetSign(string secretKey, string hash);
    }
}
