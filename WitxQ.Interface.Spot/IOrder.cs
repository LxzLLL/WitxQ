using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Interface.Spot
{
    /// <summary>
    /// 订单接口
    /// </summary>
    public interface IOrder
    {
        /// <summary>
        /// 买类型订单
        /// <para>
        /// 例如：LRC-ETH，买LRC，卖ETH，则 sellToken为ETH，buyToken为LRC
        /// </para>
        /// </summary>
        /// <param name="market">市场，交易对，全部为中间“-”连字符的大写形式,例如：LRC-ETH</param>
        /// <param name="price">价格</param>
        /// <param name="amount">数量，指此交易对的交易数量，也即是BaseToken的数量</param>
        /// <param name="sellToken">出售的代币符号，例如LRC</param>
        /// <param name="buyToken">买入的代币符号，例如ETH</param>
        /// <param name="orderNumber">返回的订单标识</param>
        /// <param name="err">异常信息</param>
        /// <returns></returns>
        public bool OrderBuy(string market,string price, string amount,string sellToken,string buyToken, ref string orderNumber, ref string err);


        /// <summary>
        /// 卖类型订单
        /// <para>
        /// 例如:LRC-ETH，卖LRC，买ETH，则 sellToken为LRC，buyToken为ETH
        /// </para>
        /// </summary>
        /// <param name="market">市场，交易对，全部为中间“-”连字符的大写形式,例如：LRC-ETH</param>
        /// <param name="price">价格</param>
        /// <param name="amount">数量，指此交易对的交易数量，也即是BaseToken的数量</param>
        /// <param name="sellToken">出售的代币符号，例如LRC</param>
        /// <param name="buyToken">买入的代币符号，例如ETH</param>
        /// <param name="orderNumber">返回的订单标识</param>
        /// <param name="err">异常信息</param>
        /// <returns></returns>
        public bool OrderSell(string market, string price, string amount, string sellToken, string buyToken, ref string orderNumber, ref string err);


        /// <summary>
        /// 批量订单
        /// <para>
        /// 例如：LRC-ETH，如果side为sell（卖方向），即 卖LRC，买ETH，则 sellToken为LRC，buyToken为ETH；
        /// 如果side为buy（买方向），即 买LRC，卖ETH，则 sellToken为ETH，buyToken为LRC
        /// </para>
        /// </summary>
        /// <param name="orderBatchParams">批量订单参数,
        /// string[6]个参数，
        /// 0:market(例如：LRC-ETH),
        /// 1:side(订单买卖类型buy:买入;sell:卖出，针对交易对 例如LRC-ETH，买入即为买LRC),
        /// 2:price,
        /// 3:amount,指此交易对的交易数量，也即是BaseToken的数量
        /// 4:sellToken(例如：LRC),
        /// 5:buyToken(例如：ETH)
        /// </param>
        /// <param name="orderNumbers">返回的订单标识列表（可能有部分为空值，因为批量订单部分成功）</param>
        /// <param name="err">异常信息</param>
        /// <returns></returns>
        public bool OrderBatch(List<List<string>> orderParams,ref List<string> orderNumbers, ref string err);

        /// <summary>
        /// 批量取消订单
        /// </summary>
        /// <param name="orderNumbers">订单标识列表</param>
        /// <param name="delState">返回撤销订单状态</param>
        /// <param name="err">异常信息</param>
        /// <returns></returns>
        public bool OrderDelBatch(List<string> orderNumbers, ref List<bool> delState, ref string err);

    }
}
