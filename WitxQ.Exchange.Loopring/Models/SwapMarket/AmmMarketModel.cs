using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.SwapMarket
{
    /// <summary>
    /// Amm swap 的市场模型
    /// </summary>
    public class AmmMarketModel:BaseModel
    {

        /// <summary>
        /// 市场名称，例如:"AMM-LRC-USDT"
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 市场，例如:"AMM-LRC-USDT"
        /// </summary>
        public string market { get; set; }

        /// <summary>
        /// 合约地址，例如"0x97241525fe425C90eBe5A41127816dcFA5954b06"
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// 池里面的token
        /// <para>
        /// [0]:基础币
        /// [1]:计价币
        /// </para>
        /// </summary>
        public List<int> inPoolTokens { get; set; }

        /// <summary>
        /// 池id
        /// </summary>
        public int poolTokenId { get; set; }

        /// <summary>
        /// 费率 百分率
        /// </summary>
        public int feeBips { get; set; }

        /// <summary>
        /// 价格精度
        /// </summary>
        public int pricePrecision { get; set; }

        /// <summary>
        /// 数量精度
        /// </summary>
        public int amountPrecision { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool enabled { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int status { get; set; }


        #region 自定义扩展
        /// <summary>
        /// 池中的基础币ID
        /// </summary>
        public int PoolBaseTokenId
        {
            get
            {
                int id = -1;
                if (this.inPoolTokens == null || this.inPoolTokens.Count != 2)
                {
                    throw new Exception("AmmMarketModel inPoolTokens异常！");
                }
                else
                {
                    id = this.inPoolTokens[0];
                }
                return id;
            }
        }

        /// <summary>
        /// 池中的定价币ID
        /// </summary>
        public int PoolQuoteTokenId
        {
            get
            {
                int id = -1;
                if (this.inPoolTokens == null || this.inPoolTokens.Count != 2)
                {
                    throw new Exception("AmmMarketModel inPoolTokens异常！");
                }
                else
                {
                    id = this.inPoolTokens[1];
                }
                return id;
            }
        }
        #endregion 
    }
}
