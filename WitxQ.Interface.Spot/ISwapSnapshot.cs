using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Model.Markets;

namespace WitxQ.Interface.Spot
{
    /// <summary>
    /// Swap的市场快照信息
    /// </summary>
    public interface ISwapSnapshot
    {

        /// <summary>
        /// 获取Amm闪兑交易深度事件
        /// </summary>
        public event EventHandler<List<SwapSnapshotModel>> GetSwapDepthEvent;


        /// <summary>
        /// 获取swap的snapshots
        /// </summary>
        /// <param name="pairNames">交易对名称列表，如果为null，则获取全部数据，默认为null</param>
        /// <returns></returns>
        public List<SwapSnapshotModel> GetSnapshotsByPairs(List<string> pairNames = null);


        /// <summary>
        /// 通过swap交易对名称获取snapshot
        /// </summary>
        /// <param name="swapMarketPairName">交易对，全部为中间“-”连字符的大写形式，例如AMM-LRC-USDT</param>
        /// <returns></returns>
        public SwapSnapshotModel GetSnapshotByPair(string swapMarketPairName);

    }
}
