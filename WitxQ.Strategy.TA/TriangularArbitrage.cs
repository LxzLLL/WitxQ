using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WitxQ.Interface.Spot;
using WitxQ.Interface.Strategy;
using WitxQ.Interface.StrategyTA;
using WitxQ.Model.Markets;

namespace WitxQ.Strategy.TA
{
    /// <summary>
    /// 三角套利，仅支持三个币种之间的套利
    /// <para>
    /// 不采用价格轮询，使用事件触发更新此策略的深度信息，并发出信号，通知策略线程进行交易
    /// </para>
    /// </summary>
    public class TriangularArbitrage: ITriangularArbitrage
    {
        /// <summary>
        /// 此套利所需参数
        /// </summary>
        private TriangularArbitrageParam _taParam;

        /// <summary>
        /// 订单是否完成
        /// 目的：1、避免同时下单，导致获取到同一个订单ID，致使订单提交失败
        ///      2、避免同时满足条件的套利，由于前一个下单时间关系，使第二个下单时，已经与实际 价格与数量不符，因此在第一个未完成时，第二个单抛弃掉
        /// </summary>
        private bool _isOrderDone = true;

        /// <summary>
        /// 订单是否完成 的锁
        /// </summary>
        private readonly object _lockIsOrderDone = new object();

        /// <summary>
        /// 日志记录器
        /// </summary>
        private TALogger _logger;

        /// <summary>
        /// order的接口
        /// </summary>
        private IOrder _order;

        /// <summary>
        /// depth接口
        /// </summary>
        private IDepth _depth;

        /// <summary>
        /// balances接口
        /// </summary>
        private IBalances _balances;

        /// <summary>
        /// 策略状态
        /// </summary>
        /// <returns></returns>
        private StrategyStateEnum _strategyState;

        /// <summary>
        /// 上次完成order的时间，（由于接口调用限制，需要做处理）
        /// </summary>
        private DateTime _lastOrderDoneTime = DateTime.Now;
        /// <summary>
        /// order的间隔时间（毫秒）
        /// </summary>
        private int _intervalOrder = 0;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="taParams">创建三角套利的参数</param>
        public TriangularArbitrage(TriangularArbitrageParam taParams)
        {
            this._taParam = taParams;
            // 检查参数
            if (!this.IsCorrectPairSeqNumber())
                throw new Exception("TriangularArbitrage--初始化时检测到，交易对的交易顺序不正确，请检查！");
            // 创建必须内容
            this._logger = new TALogger(this._taParam.LoggerPath, this._taParam.ExchangeName+"("+this._taParam.TARingName + ")");
            // 各交易对的深度信息
            //this._depthModels = new List<DepthModel>();

            this._strategyState = StrategyStateEnum.UnExecuted;   // 初始为“未执行状态”

            this._intervalOrder = taParams.IntervalOrderCall;
        }


        #region ITriangularArbitrage

        /// <summary>
        /// 设置此三角套利策略
        /// </summary>
        /// <param name="order">Order接口</param>
        /// <param name="depth">深度接口</param>
        /// <param name="balances">账户接口</param>
        public void SetTATask(IOrder order, IDepth depth, IBalances balances)
        {
            this._order = order;
            this._depth = depth;
            this._balances = balances;
        }

        /// <summary>
        /// 运行此三角套利策略
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            bool blnIsRunning = false;
            // 当前是“未执行” 或“暂停” 状态，则可继续运行
            if(this._strategyState==StrategyStateEnum.UnExecuted|| this._strategyState == StrategyStateEnum.Suspend)
            {
                this._depth.GetDepthEvent += this.OnGetDepth;  //  添加深度信息改变通知事件
                this._strategyState = StrategyStateEnum.Running;
                blnIsRunning = true;
            }
            else
            {
                this._strategyState = StrategyStateEnum.Abnormal;
                throw new StrategyTAException("", "TriangularArbitrage策略当前状态不为：'UnExecuted'或'Suspend'状态，不能运行...");
            }

            return blnIsRunning;
        }

        /// <summary>
        /// 停止此三角套利策略
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            this._depth.GetDepthEvent -= this.OnGetDepth;  //  添加深度信息改变通知事件
            this._strategyState = StrategyStateEnum.Completed;
            return true;
        }

        /// <summary>
        /// 暂停此策略
        /// </summary>
        /// <returns></returns>
        public bool Suspend()
        {
            this._depth.GetDepthEvent -= this.OnGetDepth;  //  添加深度信息改变通知事件
            this._strategyState = StrategyStateEnum.Suspend;
            return true;
        }

        /// <summary>
        /// 获取此策略的状态
        /// </summary>
        /// <returns></returns>
        public StrategyStateEnum GetStrategyState()
        {
            return this._strategyState;
        }

        #endregion

        /// <summary>
        /// 获取深度信息回调事件，策略执行都在此方法中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="depthEventArgs"></param>
        private void OnGetDepth(object sender, DepthModel depthEventArgs)
        {
            // 运行任务  TODO TriangularArbitrage.OnGetDepth return处都需要写日志
            Task task = Task.Run(() =>
            {
                try
                {
                    // 如果深度是空，或者 深度交易对不是 此策略中的交易对，则直接返回
                    if (depthEventArgs == null || !this._taParam.TAPairs.Exists(tap => tap.Pair.PairSymbol.Equals(depthEventArgs.PairSymbol)))
                    {
                        this._logger.Info($"回调订单薄异常，订单薄为空或订单薄交易对（{depthEventArgs.PairSymbol}）不在策略（{this._taParam.TARingName}）中" + Environment.NewLine, "OnGetDepth--Task");
                        return;
                    }

                    // 锁定下单，同步执行
                    lock(this._lockIsOrderDone)
                    {
                        if (!this._isOrderDone)
                        {
                            this._logger.Info($"上个订单未完成，抛弃本次操作" + Environment.NewLine, "OnGetDepth--Task");
                            return;
                        }
                        this._isOrderDone = false;
                    }

                    DateTime dtStart = DateTime.Now;
                    StringBuilder sbLog = new StringBuilder();  //日志
                    sbLog.Append($"开始时间：{dtStart.ToString("yyyy-MM-dd HH:mm:ss.fff")}" + Environment.NewLine);

                    // 1、获取其他交易对的深度 并 添加到此次套利的深度列表
                    Dictionary<string, DepthModel> dictDepthModels = this.GetDepths(depthEventArgs);
                    // 深度信息日志
                    dictDepthModels.Values.ToList().ForEach(dm =>
                    {
                        sbLog.Append($"深度交易对：{dm.PairSymbol}，买一：[{dm.BuyBids[0][0]},{dm.BuyBids[0][1]}]，卖一：[{dm.SellAsks[0][0]},{dm.SellAsks[0][1]}]" + Environment.NewLine);
                    });

                    if (dictDepthModels == null || dictDepthModels.Count != this._taParam.Deep)
                    {
                        this._isOrderDone = true;
                        sbLog.Append($"订单薄异常，订单薄为空或订单薄的个数（{dictDepthModels.Count}个）与策略交易对深度（deep={this._taParam.Deep}）不匹配" + Environment.NewLine);
                        this._logger.Info(sbLog.ToString(), "OnGetDepth--Task");
                        return;
                    }

                    // 2、计算是否需要下单
                    // 2.1 计算环路利差
                    decimal ringProfitRate = this.RingProfitRate(dictDepthModels);
                    sbLog.Append($"环路利差：{ringProfitRate * 100}%，配置交易的利差：{this._taParam.MinProfitRatio * 100}%" + Environment.NewLine);
                    // 环路利差 小于 配置的利差比例，则不交易
                    if (ringProfitRate < this._taParam.MinProfitRatio)
                    {
                        this._isOrderDone = true;
                        this._logger.Info(sbLog.ToString(), "OnGetDepth--Task");
                        return;
                    }

                    // 2.2 计算最小下单额度（包括账户和此次需要交易的最小额）
                    Dictionary<string, BalancesModel> dictBalances = this.GetBalances();
                    // 账户信息日志
                    dictBalances.Values.ToList().ForEach(bm =>
                    {
                        sbLog.Append($"账户Token：{bm.Symbol}，可用：{bm.Available}，锁定：{bm.Locked}" + Environment.NewLine);
                    });

                    // 交易对账户为空 或 交易对账户的token 不等于 深度数，说明套利数量不同，不交易
                    if (dictBalances == null || dictBalances.Count != this._taParam.Deep)
                    {
                        this._isOrderDone = true;
                        sbLog.Append($"交易对账户异常，交易对账户为空或交易对账户的个数（{dictBalances.Count}个）与策略交易对深度（deep={this._taParam.Deep}）不匹配" + Environment.NewLine);
                        this._logger.Info(sbLog.ToString(), "OnGetDepth--Task");
                        return;
                    }

                    decimal tranAmount = this._taParam.MinAmountTran;   // 配置的最小交易数量
                    decimal minBalanceAmount = this.MinAmountBalances(dictBalances, dictDepthModels);  // 账户最小数量
                    decimal minTransAmount = this.MinAmountTrans(dictBalances, dictDepthModels);   // 深度中的此策略的所有交易对 最小数量

                    sbLog.Append($"配置最小交易数量：{tranAmount}，账户最小数量：{minBalanceAmount}，深度中最小数量{minTransAmount}"+Environment.NewLine);

                    // 不是按配置的最小交易数据量 交易                                                                           
                    if (!this._taParam.IsMinAmountTran)
                    {
                        // 取 交易账户 和 交易对 数量的最小值
                        tranAmount = minBalanceAmount >= minTransAmount ? minTransAmount : minBalanceAmount;

                        // 如果小于最低值，则不交易
                        if (tranAmount < this._taParam.MinAmountTran)
                        {
                            this._isOrderDone = true;
                            this._logger.Info($"交易最小数量（{tranAmount}{this._taParam.TargetToken}）小于配置的最低值（{this._taParam.MinAmountTran}{this._taParam.TargetToken}）" + Environment.NewLine, "OnGetDepth--Task");
                            return;
                        }

                        // 拿交易数量的百分AmountTranPercentage进行交易
                        decimal tranAmountTemp = Math.Round(tranAmount * this._taParam.AmountTranPercentage, 4);
                        // 取最大的
                        tranAmount = tranAmountTemp >= this._taParam.MinAmountTran ? tranAmountTemp : this._taParam.MinAmountTran;
                    }

                    sbLog.Append($"最终选择最小数量：{tranAmount}" + Environment.NewLine);

                    // 3、下单 并 写 订单日志
                    StringBuilder strTranContent = new StringBuilder();
                    bool blnOrderSuccess = this.Order(tranAmount, dictDepthModels, ref strTranContent);
                    this._logger.Tran(strTranContent.ToString());

                    sbLog.Append($"***********************下单过程日志***********************" + Environment.NewLine);
                    sbLog.Append(strTranContent);
                    sbLog.Append($"***********************下单过程日志***********************" + Environment.NewLine);
                    // 4、写 每次调用的日志
                    DateTime dtEnd = DateTime.Now;
                    sbLog.Append($"结束时间：{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.fff")}，最终耗时：{(dtEnd - dtStart).TotalMilliseconds}ms" + Environment.NewLine);

                    this._logger.Info(sbLog.ToString());
                }
                catch(Exception ex)
                {
                    this._logger.Error(ex, "OnGetDepth--Task");
                }

                this._isOrderDone = true;
            });
        }

        /// <summary>
        /// 判断交易顺序是否正确
        /// </summary>
        /// <returns></returns>
        private bool IsCorrectPairSeqNumber()
        {
            bool blnIsCorrect = false;

            // 1、检查序号
            bool blnTemp = true;
            for(int i=0;i<this._taParam.TAPairs.Count;i++)
            {
                // 序号不同（序号从1开始）
                if(i+1!= this._taParam.TAPairs[i].SeqNumber)
                {
                    blnTemp = false;
                    break;
                }
            }
            if(!blnTemp)
                return blnIsCorrect;

            // 2、判断第一个和最后一个是否包含 目标token
            if (this._taParam.TAPairs.First().Pair.PairSymbol.Contains(this._taParam.TargetToken) 
                && this._taParam.TAPairs.Last().Pair.PairSymbol.Contains(this._taParam.TargetToken))
            {
                blnIsCorrect = true;
            }

            return blnIsCorrect;
        }


        /// <summary>
        /// 获取此策略的交易对的所有token的balance
        /// </summary>
        /// <returns>key:Token名称，value:BalancesModel，此Token的账户信息</returns>
        private Dictionary<string,BalancesModel> GetBalances()
        {
            Dictionary<string, BalancesModel> dictBalances = new Dictionary<string, BalancesModel>();
            HashSet<string> hs = new HashSet<string>();

            this._taParam.TAPairs.ForEach(tapp =>
            {
                string[] pairToken = tapp.Pair.PairSymbol.Split('-');
                if(pairToken.Length==2)
                {
                    hs.Add(pairToken[0]);
                    hs.Add(pairToken[1]);
                }
            });

            // 深度数 与 某角套利的token个数 一定是相同的，不同则异常
            if (hs.Count != this._taParam.Deep)
                return dictBalances;

            foreach(string token in hs)
            {
                BalancesModel balance = this._balances.GetBalancesByPair(token);
                if(balance!=null && !string.IsNullOrWhiteSpace(balance.TokenSymbol))
                    dictBalances.Add(balance.TokenSymbol, balance);
            }
            return dictBalances;
        }

        /// <summary>
        /// 获取此交易对的所有深度信息
        /// </summary>
        /// <param name="depthEventArgs">事件参数深度</param>
        /// <returns>key:PairSymbol交易对名称，value:DepthModel深度信息</returns>
        private Dictionary<string, DepthModel> GetDepths(DepthModel depthEventArgs)
        {
            Dictionary<string, DepthModel> depthModels = new Dictionary<string, DepthModel>();
            depthModels.Add(depthEventArgs.PairSymbol,depthEventArgs);
            this._taParam.TAPairs.ForEach(tapp =>
            {
                if (!tapp.Pair.PairSymbol.Equals(depthEventArgs.PairSymbol))
                {
                    DepthModel model = this._depth.GetDepthByPair(tapp.Pair.PairSymbol);
                    depthModels.Add(model.PairSymbol,model);
                }
            });

            return depthModels;
        }

        /// <summary>
        /// 根据价格列表获取此次利差率
        /// </summary>
        /// <param name="dictDepth">深度信息列表,key:交易对名称，value:DepthModel</param>
        /// <returns></returns>
        private decimal RingProfitRate(Dictionary<string, DepthModel> dictDepth)
        {
            List<TriangularArbitragePairsParam> pairs = this._taParam.TAPairs;

            // 假设初始TargetToken的数量是1（以下乘除不是固定的，是由于此套利顺序决定如下乘除方式，因为开始TargetToken，结束也为TargetToken）
            decimal dtempAmount = 1;
            for (int i=0;i<pairs.Count;i++)
            {
                // 如果是true，表明是要卖，因此需要买一的价格
                if(pairs[i].OrderSide)
                {
                    dtempAmount *= dictDepth[pairs[i].Pair.PairSymbol].BuyBids[0][0];
                }
                else
                {
                    // 如果是false，表明是要买，因此需要卖一的价格
                    dtempAmount /= dictDepth[pairs[i].Pair.PairSymbol].SellAsks[0][0];
                }
            }
            return dtempAmount - 1;
        }

        /// <summary>
        /// 获取Balances的最小数量，非下单值,需要换算成TargetToken的量
        /// <para>
        /// 只保留4个小数，之后的全部截断（非四舍五入）
        /// </para>
        /// </summary>
        /// <param name="dictBalances">账户信息key:Token名称，value:BalancesModel，此Token的账户信息</param>
        /// <param name="dictDepth">深度信息,key:交易对名称，value:DepthModel</param>
        /// <returns></returns>
        private decimal MinAmountBalances(Dictionary<string, BalancesModel> dictBalances, Dictionary<string, DepthModel> dictDepth)
        {
            // 获取账户最小的数量
            List<decimal> minAvailableAmountBalances = new List<decimal>();
            foreach(var balance in dictBalances.Values)
            {
                if(balance.TokenSymbol.Equals(this._taParam.TargetToken))
                {
                    //添加目标token的可用数量
                    minAvailableAmountBalances.Add(dictBalances[this._taParam.TargetToken].AvailableAmount);  
                }
                else
                {
                    // 添加非目标token的可用数量（需要进行计算）
                    // 计算逻辑：dictDepth的key中包括 此token和目标token，才能进行折算
                    DepthModel depth = dictDepth.Where(s => s.Key.Contains(balance.TokenSymbol) && s.Key.Contains(this._taParam.TargetToken))
                        .FirstOrDefault().Value;

                    // 此时不能使用 this._taParam.TAPairs 中的 OrderSide判断（此方向是针对交易策略的，是个固定的顺序和乘除方式）
                    string[] pairs = depth.PairSymbol.Split('-');  // string[0]:基础货币（此交易对的 交易token） string[1]:定价货币（此交易对的 计价token）
                    // 是基础货币时，需要使用 买一价格 且 为乘积的关系
                    if (balance.TokenSymbol.Equals(pairs[0]))
                    {
                        decimal amount = balance.AvailableAmount * depth.BuyBids[0][0];
                        minAvailableAmountBalances.Add(amount);
                    }
                    else if (balance.TokenSymbol.Equals(pairs[1]))
                    {
                        // 是定价货币时，需要使用 卖一价格 且 为除的关系
                        decimal amount = balance.AvailableAmount / depth.SellAsks[0][0];
                        minAvailableAmountBalances.Add(amount);
                    }
                }
            }
        
            return Math.Round(minAvailableAmountBalances.Min(), 4);  // 向下取，避免五入后不够交易额
        }

        /// <summary>
        /// 获取本次交易对的最小数量，非下单值（交易对中的对比）,需要换算成TargetToken的量
        /// <para>
        /// 只保留4个小数，之后的全部截断（非四舍五入）
        /// </para>
        /// </summary>
        /// <param name="dictBalances">账户信息key:Token名称，value:BalancesModel，此Token的账户信息</param>
        /// <param name="dictDepth">深度信息,key:交易对名称，value:DepthModel</param>
        /// <returns></returns>
        private decimal MinAmountTrans(Dictionary<string, BalancesModel> dictBalances, Dictionary<string, DepthModel> dictDepth)
        {
            // 获取 depth 挂单的最小数量
            List<decimal> minAmountTrans = new List<decimal>();
            for (int i = 0; i < this._taParam.TAPairs.Count; i++)
            {
                TriangularArbitragePairsParam pairParam = this._taParam.TAPairs[i];

                decimal tempPrice;
                decimal tempAmount;
                // 正向，价格取 买一
                if (this._taParam.TAPairs[i].OrderSide)
                {
                    tempPrice = dictDepth[pairParam.Pair.PairSymbol].BuyBids[0][0];
                    tempAmount = dictDepth[pairParam.Pair.PairSymbol].BuyBids[0][1];
                }
                else
                {
                    // 逆向，价格取 卖一
                    tempPrice = dictDepth[pairParam.Pair.PairSymbol].SellAsks[0][0];
                    tempAmount = dictDepth[pairParam.Pair.PairSymbol].SellAsks[0][1];
                }

                // 如果交易对中包括 TargetToken，表明可以直接转换
                if (pairParam.Pair.PairSymbol.Contains(this._taParam.TargetToken))
                {
                    // 此时不能使用 this._taParam.TAPairs 中的 OrderSide判断（此方向是针对交易策略的，是个固定的顺序和乘除方式）

                    // 如果TargetToken在此交易对中是基础货币时
                    if (this._taParam.TargetToken.Equals(pairParam.Pair.BaseToken))
                    {
                        minAmountTrans.Add(tempAmount);
                    }
                    else if (this._taParam.TargetToken.Equals(pairParam.Pair.QuoteToken))
                    {
                        // 是定价货币时，表明数量是基础货币的，转换为定价货币 价格*数量 即可
                        minAmountTrans.Add(tempAmount * tempPrice);
                    }
                }
                else
                {
                    // 如果交易对中不包括 TargetToken，表明需要间接的进行转换
                    // 找到非定价货币及其个数，并进行 TargetToken折算
                    string baseToken = pairParam.Pair.BaseToken;
                    decimal baseTokenAmount = tempAmount;

                    // 找此token与TargetToken的交易对深度
                    DepthModel depth = dictDepth.Where(s => s.Key.Contains(baseToken) || s.Key.Contains(this._taParam.TargetToken))
                        .FirstOrDefault().Value;

                    string[] strPair = depth.PairSymbol.Split('-');
                    // 如果 此Token在此交易对中是基础货币时，需要使用 买一价格 且 为乘积的关系
                    if (baseToken.Equals(strPair[0]))
                    {
                        minAmountTrans.Add(baseTokenAmount * depth.BuyBids[0][0]);
                    }
                    else if (baseToken.Equals(strPair[1]))
                    {
                        // 是定价货币时，需要使用 卖一价格 且 为除的关系
                        minAmountTrans.Add(baseTokenAmount / depth.SellAsks[0][0]);
                    }
                }
            }

            return Math.Round(minAmountTrans.Min(), 4);  // 向下取，避免五入后不够交易额

        }

        /// <summary>
        /// 下单
        /// </summary>
        /// <param name="tranAmount">开始交易数量</param>
        /// <param name="dictDepth">深度信息,key:交易对名称，value:DepthModel</param>
        /// <param name="strLogContent">日志内容</param>
        /// <returns></returns>
        private bool Order(decimal tranAmount, Dictionary<string, DepthModel> dictDepth,ref StringBuilder strLogContent)
        {
            DateTime dtStart = DateTime.Now;
            bool blnOrderSuccess = true;

            // 批量订单列表 string[6]个参数，0:market,1:side(订单买卖类型buy:买入;sell:卖出，针对交易对 例如LRC-ETH，买入即为买LRC),2:price,3:amount,4:sellToken,5:buyToken
            List<List<string>> orderParams = new List<List<string>>();

            decimal currentAmount = tranAmount;   // 当前组交易的数量
            strLogContent.Append($"OrderBatch param 说明: [0]:market,[1]:side(订单买卖类型buy:买入;sell:卖出，针对交易对 例如LRC-ETH，买入即为买LRC),[2]:价格,[3]:数量,[4]:sellToken,[5]:buyToken" + Environment.NewLine);
            for (int i=0;i<this._taParam.TAPairs.Count;i++)
            {
                //List<string> arrayOrderParams = new List<string>(6);
                string[] arrayOrderParams = new string[6];
                TriangularArbitragePairsParam pairParam = this._taParam.TAPairs[i];
                DepthModel depth = dictDepth[pairParam.Pair.PairSymbol];
                arrayOrderParams[0] = pairParam.Pair.PairSymbol;
                //arrayOrderParams[3] = amount.ToString();

                decimal tempPrice;
                string buyToken ,sellToken ,side;   // side:"buy" or "sell"
                // 正向，价格取 买一（卖基础货币，买定价货币）
                if (pairParam.OrderSide)
                {
                    tempPrice = dictDepth[pairParam.Pair.PairSymbol].BuyBids[0][0] * (1 - this._taParam.SlidingPoint);    // 减去滑点

                    arrayOrderParams[3] = currentAmount.ToString();  // 当前组的数量
                    currentAmount = currentAmount * tempPrice;   // 下一组交易的数量

                    buyToken = pairParam.Pair.QuoteToken;
                    sellToken = pairParam.Pair.BaseToken;
                    side = "sell";
                }
                else
                {
                    // 逆向，价格取 卖一（卖定价货币，买基础货币）
                    tempPrice = dictDepth[pairParam.Pair.PairSymbol].SellAsks[0][0] * (1 + this._taParam.SlidingPoint);    // 加上滑点

                    currentAmount = currentAmount / tempPrice;   // 当前组的数量 和 下一组交易的数量 一样
                    arrayOrderParams[3] = currentAmount.ToString();

                    buyToken = pairParam.Pair.BaseToken;
                    sellToken = pairParam.Pair.QuoteToken;
                    side = "buy";
                }

                arrayOrderParams[1] = side;
                arrayOrderParams[2] = tempPrice.ToString();
                arrayOrderParams[4] = sellToken;
                arrayOrderParams[5] = buyToken;

                orderParams.Add(arrayOrderParams.ToList());

                //记录日志 价格、数量等信息
                strLogContent.Append($"OrderBatch param {i}:"+string.Join(',', arrayOrderParams)+Environment.NewLine);
            }

            // 判断配置是否下单，不下单时只记录日志
            double intervalMS = 0;
            int sleepMS = 0;
            if (this._taParam.IsTranAuto)
            {
                string strError = string.Empty;
                List<string> orderNumbers = new List<string>();

                // 考虑order的接口调用间隔时间
                DateTime currentDT = DateTime.Now;
                intervalMS = (currentDT - this._lastOrderDoneTime).TotalMilliseconds - this._intervalOrder;
                if (intervalMS < 0)
                {
                    sleepMS = (int)Math.Ceiling(intervalMS * -1);
                    Thread.Sleep(sleepMS);
                }

                blnOrderSuccess = this._order.OrderBatch(orderParams,ref orderNumbers, ref strError);
                // 订单日志
                strLogContent.Append($"下单{(blnOrderSuccess?"成功":"失败，ErrorMessage："+strError)}" + Environment.NewLine);
            }
            else
            {
                strLogContent.Append($"配置值不进行实际下单，只记录日志" + Environment.NewLine);
            }
            //this._logger.Tran("", "Tran-Error");

            DateTime dtEnd = DateTime.Now;
            this._lastOrderDoneTime = dtEnd;
            strLogContent.Append($"批量Order耗时：{(dtEnd-dtStart).TotalMilliseconds} 毫秒，开始时间：{dtStart.ToString("yyyy-MM-dd HH:mm:ss.fff")}，结束时间：{dtEnd.ToString("yyyy-MM-dd HH:mm:ss.fff")}，休眠时长：{sleepMS}" + Environment.NewLine);
            return blnOrderSuccess;
        }
        
    }
}
