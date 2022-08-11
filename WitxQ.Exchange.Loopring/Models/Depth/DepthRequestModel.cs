using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.Depth
{
    /// <summary>
    /// 深度的请求实体
    /// </summary>
    public class DepthRequestModel : BaseModel
    {
        /// <summary>
        /// 市场对，不支持多市场，例如："LRC-ETH"
        /// </summary>
        public string market { get; set; }

        /// <summary>
        /// 深度等级，越大表示合并的深度越多
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 返回条数限制默认值 : 20
        /// </summary>
        public int limit { get; set; } = 20;
    }
}
