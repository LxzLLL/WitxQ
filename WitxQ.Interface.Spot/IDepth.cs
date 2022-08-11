using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Model.Markets;

namespace WitxQ.Interface.Spot
{
    /// <summary>
    /// 深度接口
    /// </summary>
    public interface IDepth
    {
        /// <summary>
        /// 获取交易深度事件
        /// </summary>
        public event EventHandler<DepthModel> GetDepthEvent;

        /// <summary>
        /// 通过交易对获取深度信息
        /// </summary>
        /// <param name="pair">交易对，全部为中间“-”连字符的大写形式</param>
        /// <returns></returns>
        public DepthModel GetDepthByPair(string pair);


        
    }
}
