using System;
using System.Collections.Generic;
using System.Linq;
using WitxQ.Interface.StrategyTA;

/*******************************************************************************
    本文给出了一个找到无向图中所有的环的递归算法，该算法是基于DFS（深度优先搜索）的，
    大概的思路是：在深度优先搜索无向图的过程中，当遇到起始点的时候，
    会认定为出现环（在本文中只是找出了无向图中所有的长度大于等于3的环（长度为1和2的环没有意思），
    所以在深搜的过程中，当遇到的是起始点的时候，还需要进行判断是否是环），当确定是出现了环之后，
    根据是否在遇到环之前的那个点还有其他的路径，来决定是进一步的进行深度优先搜索还是进行回退，
    在进行深度优先搜索的过程中，将访问过的节点标记，若当前的节点无路可走（不能进行深度优先搜索了），
    在回退的过程中，将标记取消

    假设以1为起点进行深度优先搜索，经过访问2，3，4，5，6会得到一个环，因为节点6还有下一条路径可走，
    此时程序会进入7，8，9，10这些点进行深度优先搜索，但是都再没有回到节点1，
    于是程序会一层一层的在从7，8，9，10（不一定是这样的顺序）这些点退出来。
    退至节点6，5，4直到3节点(将6，5，4的标记全部取消)找到了下一条路径5，在走到6，
    此时又发现了另一条环1->2->3->5->6->1.以此类推。
 **********************************************************************************/

namespace WitxQ.Strategy.TA.GraphRing
{
    /// <summary>
    /// 无向图环
    /// </summary>
    public class UDGraphRing: IUndirectedGraph
    {
        #region 私有成员变量
        /// <summary>
        /// 图的所有节点
        /// </summary>
        private List<Vertex> _vertexes = new List<Vertex>();

        /// <summary>
        /// 无向图的边
        /// </summary>
        private List<Edge> _edges = new List<Edge>();

        /// <summary>
        /// 邻接矩阵
        /// </summary>
        private int[,] _adjacentMatrix;

        /// <summary>
        /// 以节点名称为key的字典
        /// </summary>
        private Dictionary<string, Vertex> _dictNameVertex = new Dictionary<string, Vertex>();

        /// <summary>
        /// 以节点Id为key的字典
        /// </summary>
        private Dictionary<int, Vertex> _dictIdVertex = new Dictionary<int, Vertex>();

        /// <summary>
        /// 是否重新递归
        /// </summary>
        private int _isRecall;

        /// <summary>
        /// 搜索的深度
        /// </summary>
        private int _innerStep = 0;

        /// <summary>
        /// 每次遍历时压入节点id的栈
        /// </summary>
        private Stack<int> _sequenceStack;

        /// <summary>
        /// 每个节点的环路路径列表
        /// </summary>
        //private List<ArrayList> _vertexRingPath;

        /// <summary>
        /// 访问标记
        /// </summary>
        //private bool[] _visitedFlag;

        /// <summary>
        /// 环路记录
        /// </summary>
        private List<Ring> _rings;

        /// <summary>
        /// 初始化使用的无向图的边列表
        /// </summary>
        private List<string> _pairsParam = new List<string>();

        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public UDGraphRing(){}

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="pairs">无向图的边列表，不能同时出现正反向的边，且必须大写，中间有“-”连字符，例如：A-B或C-D，不能同时出现A-B和B-A</param>
        public UDGraphRing(List<string> pairs)
        {
            this._pairsParam = pairs;

            // 初始化图的所有节点
            this.InitVertex();
            // 初始化图的所有边
            this.InitEdge();
            // 初始化 邻接矩阵
            this.InitAdjacentMatrix();
        }


        #region IUndirectedGraph 接口

        /// <summary>
        /// 返回所有点环
        /// <para>
        /// 返回值，所有顶点的所有环集合
        /// 1、第一层Dictionary：key：顶点标识，value：顶点的所有环集合信息
        /// 2、第二层Dictionary：key：RingName环的字符串表达（例如：A-B-C-A），value：此环路下的环节点信息
        /// 3、Tuple：item1：SeqNumber（顺序，从1开始）,item2：pair（即类初始化时接收的图的边），item3：EdgeSide（边的方向）
        /// </para>
        /// </summary>
        /// <param name="targetVertexName">节点名称，返回此节点的所有环，如果为空或""，则返回所有节点的环</param>
        /// <param name="deep">深度，如果小于3，则返回所有深度的环，大于等于3，则返回指定深度的环</param>
        /// <returns>返回所有点环</returns>
        public Dictionary<string, Dictionary<string, List<Tuple<int,string, bool>>>> GetUDGraphRings(List<string> pairs, string targetVertexName="", int deep=-1)
        {
            this._pairsParam = pairs;

            // 初始化图的所有节点
            this.InitVertex();
            // 初始化图的所有边
            this.InitEdge();
            // 初始化 邻接矩阵
            this.InitAdjacentMatrix();
            // 运算深度优先 获取环路算法
            this.DFSTraverse();

            return this.GetRings(targetVertexName, deep);
        }
        #endregion

        /// <summary>
        /// 深度优先遍历，获取环路
        /// </summary>
        public void DFSTraverse()
        {
            // 初始化访问标记
            bool[] visitedFlag = new bool[this._vertexes.Count];

            // 初始化环路记录
            this._rings = new List<Ring>();

            Console.WriteLine("VisitedFlagArray:");
            string str = string.Empty;
            for (int i = 0; i < visitedFlag.Length; i++)
            {
                str += visitedFlag[i] + "  ";
            }
            Console.WriteLine(str);

            // 以每个节点开始，遍历查找环路
            this._vertexes.ForEach(v =>
            {
                // 重置访问标记
                Array.Clear(visitedFlag, 0, visitedFlag.Length);

                Console.WriteLine("VisitedFlagArray重置:");
                string str = string.Empty;
                for (int i = 0; i < visitedFlag.Length; i++)
                {
                    str += visitedFlag[i] + "  ";
                }
                Console.WriteLine(str);

                // 此节点未访问过
                if(!visitedFlag[v.Id])
                {

                    this._sequenceStack = new Stack<int>();
                    //this._vertexRingPath = new List<ArrayList>();
                    this._innerStep = 0;
                    this._isRecall = 0;
                    DFS(v,visitedFlag,v);

                    //this._vertexRingPath.ForEach(array =>
                    //{
                    //    string str = string.Empty;
                    //    for (int i = 0; i < array.Count; i++)
                    //    {
                    //        str += array[i] + $"({this._dictIdVertex[(int)array[i]].Name})-->";
                    //    }
                    //    str = str.Substring(0, str.Length - 3);
                    //    Console.WriteLine(str);
                    //});
                }

            });

            // 所有节点的所有环路都查找完毕后 补上最后的节点（即原点，因为环路，只记录了开始，例如：A-B-C-A）
            this._rings.ForEach(ring =>
            {
                ring.VertexSequence.ForEach(ringSeq =>
                {
                    ringSeq.Sequence.Add(ring.FirstVertex.Id);
                });
            });

            this._rings.ForEach(ring =>
            {
                Console.WriteLine($"\n-----------the loop start and end with {ring.FirstVertex.Id}-{ring.FirstVertex.Name}-----------\n");

                string str = string.Empty;
                ring.VertexSequence.ForEach(ringSeq =>
                {
                    str = $"The Loop Length:{ringSeq.Step}  ";
                    //ringSeq.Sequence.Add(ring.FirstVertex.Id);
                    for (int i = 0; i < ringSeq.Sequence.Count; i++)
                    {
                        str += ringSeq.Sequence[i] + $"({this._dictIdVertex[ringSeq.Sequence[i]].Name})-->";
                    }
                    Console.WriteLine(str.Substring(0, str.Length - 3));
                });
            });

        }

        /// <summary>
        /// 返回所有点环
        /// <para>
        /// 返回值，所有顶点的所有环集合
        /// 1、第一层Dictionary：key：顶点标识，value：顶点的所有环集合信息
        /// 2、第二层Dictionary：key：RingName环的字符串表达（例如：A-B-C-A），value：此环路下的环节点信息
        /// 3、Tuple：item1：SeqNumber（顺序，从1开始）,item2：pair（即类初始化时接收的图的边），item3：EdgeSide（边的方向）
        /// </para>
        /// </summary>
        /// <param name="targetVertexName">节点名称，返回此节点的所有环，如果为空或""，则返回所有节点的环</param>
        /// <param name="deep">深度，如果小于3，则返回所有深度的环，大于等于3，则返回指定深度的环</param>
        /// <returns>返回所有点环</returns>
        public Dictionary<string, Dictionary<string,List<Tuple<int,string, bool>>>> GetRings(string targetVertexName = "",int deep=-1)
        {
            var rings = new Dictionary<string, Dictionary<string, List<Tuple<int,string,bool>>>>();
            // 找边 并返回（Ring、Pair、StartName、EndName、EdgeSide）
            // 1、第一层Dictionary：key：顶点标识，value：顶点的所有环集合信息
            // 2、第二层Dictionary：key：RingName环的字符串表达（例如：A-B-C-A），value：此环路下的环节点信息
            // 3、Tuple：item1：SeqNumber（顺序，从1开始）,item2：pair（即类初始化时接收的图的边），item3：EdgeSide（边的方向）
            // Dictionary<string, Dictionary<string,List<Tuple< string, string, string, bool>>>>

            List<Ring>  ringTemp = !string.IsNullOrWhiteSpace(targetVertexName)
                ? this._rings.FindAll(r => r.FirstVertex.Name.Equals(targetVertexName))
                : this._rings;

            // 遍历过滤后的环
            ringTemp.ForEach(r =>
            {
                var dictRing = new Dictionary<string, List<Tuple<int, string,bool>>>();
                
                // 遍历r这个顶点下的所有环路
                r.VertexSequence.ForEach(rs =>
                {
                    if((deep>=3 && rs.Step==deep) || deep<3)
                    {
                        var ringInfos = new List<Tuple<int, string, bool>>();
                        string strRingName = string.Empty;

                        int seqCount = rs.Sequence.Count;
                        for (int i = 0; i < seqCount; i++)
                        {
                            strRingName += this._dictIdVertex[rs.Sequence[i]].Name + "-";
                            // 表明不是最后一个
                            if (i != seqCount - 1)
                            {
                                // 找边
                                Edge edge = this._edges.Find(e => e.Start.Id == rs.Sequence[i] && e.End.Id == rs.Sequence[i + 1]);
                                var rInfo = new Tuple<int, string, bool>(i+1,edge.Pair, edge.EdgeSide);
                                ringInfos.Add(rInfo);
                            }
                        }
                        strRingName = strRingName.Substring(0, strRingName.Length - 1);
                        // 添加环详细信息 到 此环字典中
                        dictRing.Add(strRingName, ringInfos);
                    }
                });

                // 添加 顶点的所有环到字典
                rings.Add(r.FirstVertex.Name, dictRing);
            });

            return rings;
        }

        /// <summary>
        /// 深度优先搜索
        /// </summary>
        /// <param name="startVertex">节点</param>
        /// <param name="visitedFlag">访问标记</param>
        /// <param name="visitedFlag">最初的节点</param>
        private void DFS(Vertex startVertex, bool[] visitedFlag, Vertex topVertex)
        {
            // 设置此节点被访问过
            visitedFlag[startVertex.Id] = true;

            // 压入第一个节点
            this._sequenceStack.Push(startVertex.Id);

            // 获取临近的第一个节点
            Vertex nextVertex = this.FirstAdjacentVertex(startVertex);

            this._innerStep++;
            // 开始循环遍历
            while (true)
            {
                // 表明有临近节点
                if(nextVertex!=null)
                {
                    // 1、相邻节点被访问过，且 相邻节点为最初节点，且 长度为2时（此时还不为环，继续找）
                    if (visitedFlag[nextVertex.Id] && nextVertex.Id == topVertex.Id && this._innerStep == 2)
                    {
                        // 是否有nextVertaxID后的下一个相邻节点，有的话返回此下一个相邻节点，并继续
                        nextVertex = this.NextAdjacentVertex(startVertex, nextVertex);
                        continue;
                    }
                    // 2、相邻节点被访问过，且 相邻节点为最初节点，且 长度不为2时（表示为环，进行记录）
                    else if (visitedFlag[nextVertex.Id] && nextVertex.Id == topVertex.Id && this._innerStep != 2)
                    {
                        // 打印序列栈
                        //print_stack(loop_stack);
                        int[] array = this._sequenceStack.ToArray();

                        // 环路集合中不存在当前的开始节点的环路（即新的节点环路），则添加
                        if (!this._rings.Exists(r => r.FirstVertex.Id==topVertex.Id))
                        {
                            Ring ring = new Ring();
                            ring.FirstVertex = topVertex;
                            List<RingSequence> vSequence = new List<RingSequence>();
                            vSequence.Add(new RingSequence(array.Reverse().ToList(), this._innerStep));  // 队列打印出来是倒序的，需要翻转一下
                            ring.VertexSequence = vSequence;

                            this._rings.Add(ring);
                        }
                        else
                        {
                            Ring ring = this._rings.Find(r => r.FirstVertex.Id == topVertex.Id);
                            ring.VertexSequence.Add(new RingSequence(array.Reverse().ToList(), this._innerStep));  // 队列打印出来是倒序的，需要翻转一下
                            
                        }
                        //this._vertexRingPath.Add(new ArrayList(array.Reverse().ToList()));
                        // 是否有nextVertaxID后的下一个相邻节点，有的话返回此下一个相邻节点，并继续
                        nextVertex = this.NextAdjacentVertex(startVertex, nextVertex);
                        continue;
                    }
                    // 3、 相邻节点未访问过，则递归（不为环）
                    else if (!visitedFlag[nextVertex.Id])
                    {
                        DFS(nextVertex, visitedFlag, topVertex);
                    }

                    // 4、如果是重新调用DFS，表示已经换了一个搜索标的（heap）
                    if (this._isRecall == 1)
                    {
                        this._innerStep--;  // 退至最初状态，即0
                        Vertex temp = nextVertex;
                        nextVertex = this.NextAdjacentVertex(startVertex, nextVertex);
                        this._sequenceStack.Pop();  // 退栈
                        //pop_stack(&loop_stack, &pop_value);   // 退栈
                        visitedFlag[temp.Id] = false;    //设置为未访问
                        this._isRecall = 0;
                        continue;
                    }

                    // 是否有nextVertaxID后的下一个相邻节点，有的话返回此下一个相邻节点
                    nextVertex = this.NextAdjacentVertex(startVertex, nextVertex);
                }
                // 无节点了
                else
                {
                    this._isRecall = 1;
                    break;
                }
            }
        }

        /// <summary>
        /// 获取相邻的第一个节点
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private Vertex FirstAdjacentVertex(Vertex vertex)
        {
            for(int i=0;i<this._vertexes.Count;i++)
            {
                if (this._adjacentMatrix[vertex.Id, i] == 1)
                    return this._dictIdVertex[i];
            }
            return null;
        }


        /// <summary>
        /// 获取相邻的nextVertax后面的第一个节点
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="nextVertex"></param>
        /// <returns></returns>
        private Vertex NextAdjacentVertex(Vertex vertex,Vertex nextVertex)
        {
            for (int i = nextVertex.Id+1; i < this._vertexes.Count; i++)
            {
                if (this._adjacentMatrix[vertex.Id, i] == 1)
                    return this._dictIdVertex[i];
            }
            return null;
        }

        /// <summary>
        /// 初始化图的所有节点
        /// </summary>
        /// <param name="pairs">无向图的边列表，例如LRC-ETH交易对，必须大写且中间有“-”连字符，例如LRC-ETH</param>
        private void InitVertex()
        {
            // 消除重复token
            HashSet<string> tokensSet = new HashSet<string>();
            if (this._pairsParam != null && this._pairsParam.Count != 0)
            {
                this._pairsParam.ForEach(p =>
                {
                    if(!string.IsNullOrEmpty(p))
                    {
                        string[] pair = p.Split("-");
                        if (pair.Length == 2)
                        {
                            tokensSet.Add(pair[0]);
                            tokensSet.Add(pair[1]);
                        }
                    }
                });
                
            }

            if (tokensSet.Count > 0)
            {
                List<string> tokensList = tokensSet.ToList();
                for (int i = 0; i < tokensList.Count; i++)
                {
                    Vertex vertex = new Vertex(i, tokensList[i]);
                    this._dictNameVertex.Add(vertex.Name, vertex);
                    this._dictIdVertex.Add(i, vertex);
                    this._vertexes.Add(vertex);
                }
            }


            Console.WriteLine("Vertex:");
            string str = string.Empty;
            this._vertexes.ForEach(v =>
            {
                str += $"({v.Name}:{v.Id})  ";
            });

            Console.WriteLine(str);
        }

        /// <summary>
        /// 初始化图的所有边
        /// </summary>
        /// <param name="pairs">无向图的边列表，例如LRC-ETH交易对，必须大写且中间有“-”连字符，例如LRC-ETH</param>
        private void InitEdge()
        {
            if (this._pairsParam != null && this._pairsParam.Count > 0)
            {
                this._pairsParam.ForEach(p =>
                {
                    if (!string.IsNullOrEmpty(p))
                    {
                        string[] pair = p.Split("-");
                        if (pair.Length == 2)
                        {
                            string vStart = pair[0];
                            string vEnd = pair[1];
                            // 正边，如果不存在，则添加（避免输入源出现带方向的边，例如：同时存在A-B，B-A）
                            if (!this._edges.Exists(e => e.Start.Name.Equals(vStart) && e.End.Name.Equals(vEnd)))
                            {
                                Edge edge = new Edge(this._dictNameVertex[vStart], this._dictNameVertex[vEnd], p);
                                this._edges.Add(edge);
                            }

                            // 逆边，如果不存在，则添加（避免输入源出现带方向的边，例如：同时存在A-B，B-A）
                            if (!this._edges.Exists(e => e.End.Name.Equals(vStart) && e.Start.Name.Equals(vEnd)))
                            {
                                Edge edgeReverse = new Edge(this._dictNameVertex[vEnd], this._dictNameVertex[vStart], p);
                                this._edges.Add(edgeReverse);
                            }
                        }
                        
                    }
                });
            }
        }

        /// <summary>
        /// 初始化 邻接矩阵，整理出各个节点的所有邻接节点
        /// </summary>
        private void InitAdjacentMatrix()
        {
            int vertexCount = this._vertexes.Count;
            this._adjacentMatrix = new int[vertexCount, vertexCount];
            this._edges.ForEach(e =>
            {
                // 存在邻接的 节点数组值置位1，其他为0，表示不相接
                this._adjacentMatrix[e.Start.Id, e.End.Id] = 1;
            });

            Console.WriteLine("AdjacentMatrix:");
            for (int i=0;i< vertexCount; i++)
            {
                string str = string.Empty;
                for(int j=0;j<vertexCount;j++)
                {
                    str += $"({i},{j})"+this._adjacentMatrix[i,j]+"  ";
                }
                Console.WriteLine(str);
            }
        }

    }
}
