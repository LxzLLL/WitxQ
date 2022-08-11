using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Strategy.TA.GraphRing
{
    /// <summary>
    /// 顶点
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 结点信息
        /// </summary>
        public object Data { get; set; }    
        
        public Vertex() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="name">节点名称</param>
        /// <param name="data">结点信息</param>
        public Vertex(int id,string name,object data=null) 
        {
            this.Id = id;
            this.Name = name;
            this.Data = data;
        }
    }
}
