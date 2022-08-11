using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Strategy.TA.GraphRing
{
    /// <summary>
    /// 边
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// 边的开始节点
        /// </summary>
        public Vertex Start { get; set; }
        /// <summary>
        /// 边的结束节点
        /// </summary>
        public Vertex End { get; set; }

        /// <summary>
        /// 边上附加的信息
        /// <para>
        /// 交易对，必须大写且中间有“-”连字符，例如LRC-ETH
        /// </para>
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// 边的方向（正向边还是逆向边）,值为true则为正向，false为逆向
        /// <para>
        /// 影响买卖方向，例如pair交易对LRC-ETH，正向边时为LRC-->ETH,方向为sell LRC
        /// 逆向边时为ETH-->LRC，方向为buy LRC
        /// </para>
        /// </summary>
        public bool EdgeSide
        {
            get
            {
                string[] pairs = this.Pair.Split('-');
                if (this.Start.Name == pairs[0] && this.End.Name == pairs[1])
                    return true;
                return false;
            }
        }

        public Edge() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="startId">边的开始节点</param>
        /// <param name="endId">边的结束节点</param>
        /// <param name="pair">边上附加的信息，交易对，必须大写且中间有“-”连字符，例如LRC-ETH</param>
        public Edge(Vertex start, Vertex end, string pair) 
        {
            this.Start = start;
            this.End = end;
            this.Pair = pair;
        }
    }
}
