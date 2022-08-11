using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Market
{
    /// <summary>
    /// 交易所支持的市场配置
    /// </summary>
    public class MarketInfoModel : BaseModel
    {
        /// <summary>
        /// 市场，例如："LRC-ETH"
        /// </summary>
        public string market { get; set; }

        /// <summary>
        /// 基础货币ID（此交易对的 交易token），例如："2"（LRC）
        /// </summary>
        public int baseTokenId { get; set; }

        /// <summary>
        /// 定价货币（此交易对的 计价token），例如："0"（ETH）
        /// </summary>
        public int quoteTokenId { get; set; }

        /// <summary>
        /// 服务器端返回深度中支持的最大价格精度，例如：6
        /// </summary>
        public int precisionForPrice { get; set; }

        /// <summary>
        /// 服务器端返回深度中支持的最大归并等级，支持范围是0到该值，例如：4
        /// </summary>
        public int orderbookAggLevels { get; set; }

        /// <summary>
        /// 是否开放
        /// </summary>
        public bool enabled { get; set; }
    }
}
