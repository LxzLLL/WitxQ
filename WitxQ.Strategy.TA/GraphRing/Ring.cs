using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Strategy.TA.GraphRing
{
    /// <summary>
    /// 环的序列栈
    /// </summary>
    public class Ring
    {
        /// <summary>
        /// 目标节点（环路开始的第一个节点）
        /// </summary>
        public Vertex FirstVertex { get; set; }

        /// <summary>
        /// 环路节点序列的列表，使用节点id存储
        /// </summary>
        public List<RingSequence> VertexSequence { get; set; } = new List<RingSequence>();
    }

    /// <summary>
    /// 节点的环路序列
    /// </summary>
    public class RingSequence
    {
        /// <summary>
        /// 环路序列
        /// </summary>
        public List<int> Sequence { get; set; } = new List<int>();

        /// <summary>
        /// 深度
        /// </summary>
        public int Step { get; set; } = 0;

        public RingSequence() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="sequence">环路序列</param>
        /// <param name="step">深度</param>
        public RingSequence(List<int> sequence,int step)
        {
            this.Sequence = sequence;
            this.Step = step;
        }
    }

}
