using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Interface.StrategyTA;

namespace WitxQ.Strategy.TA
{
    /// <summary>
    /// 三角套利工厂，通过给定标的token，从支持的市场中，获取环路
    /// </summary>
    public class TriangularArbitrageFactory:ITriangularArbitrageFactory
    {

        #region ITriangularArbitrageFactory

        /// <summary>
        /// 创建三角套利
        /// </summary>
        /// <param name="taParams">创建三角套利的参数</param>
        /// <returns></returns>
        public ITriangularArbitrage Create(TriangularArbitrageParam taParams)
        {
            if (taParams == null)
                return null;

            return new TriangularArbitrage(taParams);
        }

        #endregion

        ///// <summary>
        ///// 获取三角套利的所有环路
        ///// </summary>
        ///// <param name="targetToken">目标token（要套利的token）</param>
        ///// <param name="pairs">市场上的交易对</param>
        ///// <returns>所有环路，key:交易路径,value:交易对列表,Tuple:Pair:市场交易对，Tuple:OrderSideEnum:交易方向</returns>
        //public Dictionary<string, List<Tuple<Pair, OrderSideEnum>>> GetTriangularArbitrageRings(string targetToken, List<Pair> pairs)
        //{
        //    Dictionary<string, List<Tuple<Pair, OrderSideEnum>>> dictRings = new Dictionary<string, List<Tuple<Pair, OrderSideEnum>>>();

        //    if (string.IsNullOrEmpty(targetToken) || pairs == null || pairs.Count < 0)
        //    {
        //        this._logger.Error("GetTriangularArbitrageRings params error", "");
        //        return dictRings;
        //    }


        //    return dictRings;
        //}

        ///// <summary>
        ///// 创建指定 交易所和指定标的token 的所有三角套利
        ///// </summary>
        ///// <param name="exchange">交易所</param>
        ///// <param name="targetToken">目标token</param>
        ///// <param name="pairs">市场交易对所有列表</param>
        ///// <returns></returns>
        //public List<TriangularArbitrage> CreateTriangularArbitrages(ExchangeEnum exchange, string targetToken, List<Pair> pairs)
        //{
        //    List<TriangularArbitrage> arbitrages = new List<TriangularArbitrage>();
        //    if (string.IsNullOrEmpty(targetToken) || pairs == null || pairs.Count < 0)
        //    {
        //        this._logger.Error("CreateTriangularArbitrages params error", "");
        //        return arbitrages;
        //    }


        //    Dictionary<string, List<Tuple<Pair, OrderSideEnum>>> dictRings = this.GetTriangularArbitrageRings(targetToken, pairs);
        //    if (dictRings.Count <= 0)
        //    {
        //        this._logger.Error("CreateTriangularArbitrages error,the targetToken has no ring", "");
        //        return arbitrages;
        //    }

        //    // 循环套利环路
        //    foreach(var kv in dictRings)
        //    {
        //        string ringPath = kv.Key;
        //        List<Tuple<Pair, OrderSideEnum>> ringPairs = kv.Value;
        //        if(ringPairs==null || ringPairs.Count<=0)
        //        {
        //            this._logger.Error($"CreateTriangularArbitrages error,the {ringPath}(ring path) has no pairs", "");
        //            continue;
        //        }
        //        TriangularArbitrage ta = CreateTriangularArbitrage(exchange, targetToken, ringPath, ringPairs);
        //        arbitrages.Add(ta);
        //    }

        //    return arbitrages;
        //}

        ///// <summary>
        ///// 创建指定的三角套利
        ///// </summary>
        ///// <param name="exchange">交易所</param>
        ///// <param name="targetToken">目标token</param>
        ///// <param name="ringPath">交易路径</param>
        ///// <param name="ringPairs">套利环路的交易对列表</param>
        ///// <returns></returns>
        //public TriangularArbitrage CreateTriangularArbitrage(ExchangeEnum exchange, string targetToken, string ringPath, List<Tuple<Pair, OrderSideEnum>> ringPairs)
        //{
        //    TriangularArbitrage arbitrage = null;
        //    if (string.IsNullOrWhiteSpace(targetToken) || string.IsNullOrWhiteSpace(ringPath)
        //        || ringPairs == null || ringPairs.Count < 0)
        //    {
        //        this._logger.Error("CreateTriangularArbitrage params error", "");
        //        return arbitrage;
        //    }

        //    arbitrage = new TriangularArbitrage(exchange, targetToken, ringPath, ringPairs);
        //    return arbitrage;
        //}


    }
}
