using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Exchange.Loopring.Tools;

namespace WitxQ.Exchange.Loopring.Models.SwapSnapshot
{
    /// <summary>
    /// Amm快照实体
    /// </summary>
    public class AmmSnapshotModel : BaseModel
    {
        /// <summary>
        /// 池名称，例如:"AMM-LRC-USDT"
        /// </summary>
        public string poolName { get; set; }

        /// <summary>
        /// 池的合约地址，例如"0x97241525fe425C90eBe5A41127816dcFA5954b06"
        /// </summary>
        public string poolAddress { get; set; }

        /// <summary>
        /// 池的ID
        /// </summary>
        public string poolTokenId { get; set; }

        /// <summary>
        /// 池的权益代币数量（LP数量）
        /// </summary>
        public string PoolTokenAmount { get; set; }

        /// <summary>
        /// 池里的token
        /// <para>
        /// [0]:基础币
        /// [1]:计价币
        /// </para>
        /// </summary>
        public List<int> tokenIds { get; set; }

        /// <summary>
        /// 池里的token数量
        /// <para>
        /// [0]:基础币
        /// [1]:计价币
        /// </para>
        /// </summary>
        public List<string> tokenAmounts { get; set; }


        #region 自定义扩展
        /// <summary>
        /// 池中的基础币ID
        /// </summary>
        public int PoolBaseTokenId
        {
            get
            {
                int id = -1;
                if (this.tokenIds == null || this.tokenIds.Count != 2)
                {
                    throw new Exception("AmmSnapshotModel tokenIds异常！");
                }
                else
                {
                    id = this.tokenIds[0];
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
                if (this.tokenIds == null || this.tokenIds.Count != 2)
                {
                    throw new Exception("AmmSnapshotModel tokenIds异常！");
                }
                else
                {
                    id = this.tokenIds[1];
                }
                return id;
            }
        }


        /// <summary>
        /// 池中的基础币数量
        /// </summary>
        public string PoolBaseTokenAmount
        {
            get
            {
                string amount = string.Empty;
                if(this.tokenAmounts == null || this.tokenAmounts.Count != 2)
                {
                    throw new Exception("AmmSnapshotModel tokenAmounts异常！");
                }
                else
                {
                    amount = this.tokenAmounts[0];
                }
                
                return amount;
            }
        }

        /// <summary>
        /// 池中的定价币数量
        /// </summary>
        public string PoolQuoteTokenAmount
        {
            get
            {
                string amount = string.Empty;
                if (this.tokenAmounts == null || this.tokenAmounts.Count != 2)
                {
                    throw new Exception("AmmSnapshotModel tokenAmounts异常！");
                }
                else
                {
                    amount = this.tokenAmounts[1];
                }

                return amount;
            }
        }

        /// <summary>
        /// 基础币对应定价币的价格
        /// </summary>
        public decimal Price
        {
            get
            {
                decimal price = 0M;
                // 定价币数量  除以  基础币数量，得出 基础币的价格
                decimal baseTokenAmount = LoopringConvert.GetLoopringNumber(this.PoolBaseTokenAmount, this.PoolBaseTokenId);
                if (baseTokenAmount <= 0)
                {
                    price = -1;
                }
                else
                {
                    price = LoopringConvert.GetLoopringNumber(this.PoolQuoteTokenAmount, this.PoolQuoteTokenId)
                    / LoopringConvert.GetLoopringNumber(this.PoolBaseTokenAmount, this.PoolBaseTokenId);
                }
                return price;
            }
        }

        /// <summary>
        /// 逆向价格，定价币对应基础币的价格
        /// </summary>
        public decimal ReversePrice
        {
            get
            {
                decimal reversePrice = 0M;
                // 定价币数量  除以  基础币数量，得出 基础币的价格
                decimal quoteTokenAmount = LoopringConvert.GetLoopringNumber(this.PoolBaseTokenAmount, this.PoolBaseTokenId);
                if (quoteTokenAmount <= 0)
                {
                    reversePrice = -1;
                }
                else
                {
                    reversePrice = LoopringConvert.GetLoopringNumber(this.PoolBaseTokenAmount, this.PoolBaseTokenId)
                     / LoopringConvert.GetLoopringNumber(this.PoolQuoteTokenAmount, this.PoolQuoteTokenId);
                }
                
                return reversePrice;
            }
        }

        #endregion 
    }
}
