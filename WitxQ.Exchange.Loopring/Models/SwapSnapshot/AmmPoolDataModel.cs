using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models.SwapSnapshot
{
    /// <summary>
    /// AmmPool订阅信息的返回data
    /// </summary>
    public class AmmPoolDataModel:BaseModel
    {
        /// <summary>
        /// AmmPool订阅信息的返回data
        /// </summary>
        public Tuple<List<string>, string> data;
    }
}
