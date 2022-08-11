using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.SwapSnapshot
{
    /// <summary>
    /// Amm所有快照 实体的响应实体
    /// </summary>
    public class AmmSnapshotsResponseModel : BaseModel
    {
        /// <summary>
        /// API返回信息
        /// </summary>
        public ResultInfo resultInfo { get; set; }

        /// <summary>
        /// 市场信息
        /// </summary>
        public List<AmmSnapshotModel> data { get; set; }
    }
}
