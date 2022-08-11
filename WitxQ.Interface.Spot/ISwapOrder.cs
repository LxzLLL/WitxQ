using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Interface.Spot
{
    /// <summary>
    /// 闪兑操作接口
    /// </summary>
    public interface ISwapOrder
    {
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
        public bool SwapOrderBuy(string ammMarket, string price, string amount, string sellToken, string buyToken, ref string orderNumber, ref string err);


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
        public bool SwapOrderSell(string ammMarket, string price, string amount, string sellToken, string buyToken, ref string orderNumber, ref string err);

    }
}
