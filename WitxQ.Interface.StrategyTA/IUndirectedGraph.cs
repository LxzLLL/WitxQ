using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Interface.StrategyTA
{
    /// <summary>
    /// 无向图接口
    /// </summary>
    public interface IUndirectedGraph
    {
        /// <summary>
        /// 获取无向图条件内的所有环路
        /// <para>
        /// 返回值，所有顶点的所有环集合
        /// 1、第一层Dictionary：key：顶点标识，value：顶点的所有环集合信息
        /// 2、第二层Dictionary：key：RingName环的字符串表达（例如：A-B-C-A），value：此环路下的环节点信息
        /// 3、Tuple：item1：SeqNumber（顺序，从1开始）,item2：pair（即类初始化时接收的图的边），item3：EdgeSide（边的方向）
        /// </para>
        /// </summary>
        /// <param name="pairs">无向图的边列表，不能同时出现正反向的边，且必须大写，中间有“-”连字符，例如：A-B或C-D，不能同时出现A-B和B-A</param>
        /// <param name="targetVertexName">节点名称，返回此节点的所有环，如果为空或""，则返回所有节点的环</param>
        /// <param name="deep">深度，如果小于3，则返回所有深度的环，大于等于3，则返回指定深度的环</param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, List<Tuple<int,string, bool>>>> GetUDGraphRings(List<string> pairs, string targetVertexName = "", int deep = -1);
    }
}
