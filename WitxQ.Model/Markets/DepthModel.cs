using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Model.Markets
{
    /// <summary>
    /// 订单薄，即深度
    /// </summary>
    public class DepthModel:BaseModel
    {
        /// <summary>
        /// 市场交易对，全部为中间“-”连字符的大写形式,例如：LRC-ETH
        /// </summary>
        public string PairSymbol { get; set; }

        /// <summary>
        /// 买方深度，每一条深度包含两个元素，分别是价格，挂单量[[0.002,21000]]
        /// </summary>
        public List<List<decimal>> BuyBids { get; set; }

        /// <summary>
        /// 卖方深度，每一条深度包含两个元素，分别是价格，挂单量[[0.0021,11000]]
        /// </summary>
        public List<List<decimal>> SellAsks { get; set; }
    }
}
