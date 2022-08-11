using System;
using System.Collections.Generic;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Operation;

namespace WitxQ.Exchange.Loopring
{
    /// <summary>
    /// loopring账户交易所实体
    /// </summary>
    public class ExAccountLoopring
    {

        /// <summary>
        /// 账户信息
        /// </summary>
        public AccountModel Account { get; private set; }

        /// <summary>
        /// 获取此账号的 账户接口
        /// <para>
        /// 用于 底层策略使用
        /// </para>
        /// </summary>
        public BalanceOperation Balance { get; private set; }

        /// <summary>
        /// 账户的交易所 实例构造
        /// </summary>
        /// <param name="account">账户信息</param>
        public ExAccountLoopring(AccountModel account)
        {
            if (account == null || string.IsNullOrWhiteSpace(account.Address) || string.IsNullOrWhiteSpace(account.ApiKey)
                || string.IsNullOrWhiteSpace(account.PublicKeyX) || string.IsNullOrWhiteSpace(account.PublicKeyY)
                || string.IsNullOrWhiteSpace(account.SecretKey))
                throw new ArgumentNullException("ExAccountLoopring构造时，参数异常！", new Exception());
            
            this.Account = account;

            // 添加账户信息到全局ExLoopring
            //if(!ExLoopring.EXCHANGE_ACCOUNTS.ContainsKey(this._account.AccountId))
            //    ExLoopring.EXCHANGE_ACCOUNTS.Add(this._account.AccountId, this);

            // 初始化 此账号的 余额查询操作实例（每个账号一个）
            this.Balance = new BalanceOperation(this.Account);
        }

        /// <summary>
        /// 增加市场交易对（同时会添加深度回调事件）
        /// <para>
        /// 用户在选择交易市场后，必须设置 获取此市场的深度信息
        /// </para>
        /// </summary>
        /// <param name="pairs">交易对列表</param>
        public void SetDepthPair(List<string> pairs)
        {
            ExLoopring.OPERATION_DEPTH.SetMarkPair(pairs);
        }

    }
}
