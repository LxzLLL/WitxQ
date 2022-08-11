using WitxQ.Interface.Strategy;

namespace WitxQ.Strategy.TA
{
    /// <summary>
    /// 三角套利异常
    /// </summary>
    public class StrategyTAException : StrategyException
    {
        /// <summary>
        /// 三角套利异常
        /// </summary>
        /// <param name="errCode">异常编码</param>
        /// <param name="message">异常消息</param>
        public StrategyTAException(string errCode, string message) : base("TriangularArbitrageException", errCode, message)
        {

        }
    }
}
