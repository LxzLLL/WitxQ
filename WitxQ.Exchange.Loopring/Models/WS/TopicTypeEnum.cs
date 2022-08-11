using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.WS
{
    /// <summary>
    /// 主题类型
    /// </summary>
    public enum TopicTypeEnum
    {
        /// <summary>
        /// 账号金额更新
        /// </summary>
        account,
        /// <summary>
        /// 用户订单更新
        /// </summary>
        order,
        /// <summary>
        /// 订单簿更新
        /// </summary>
        orderbook,
        /// <summary>
        /// 最新成交更新
        /// </summary>
        trade,
        /// <summary>
        /// Ticker更新
        /// </summary>
        ticker,
        /// <summary>
        /// Candlestick更新
        /// </summary>
        candlestick,
        /// <summary>
        /// AmmPol更新
        /// </summary>
        ammpool
    }
}
