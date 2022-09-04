using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;

namespace WitxQ.EventBus.RabbitMQ
{
    /// <summary>
    /// RabbitMq连接工厂
    /// </summary>
    public class RabbitMqConnectionFactory:IConnectionFactory
    {
        /// <summary>
        /// 连接参数
        /// </summary>
        public RabbitMqOption RabbitMqOption { get; private set; }

        /// <summary>
        /// 连接
        /// </summary>
        private IConnection _instance;

        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object Monitor = new object();

        public RabbitMqConnectionFactory(RabbitMqOption rabbitMqOption) => this.RabbitMqOption = rabbitMqOption;


        #region IConnectionFactory，获取单例Connection

        public IConnection GetConnection()
        {
            if (_instance != null) return _instance;
            lock (Monitor)
            {
                if (_instance == null)
                {
                    _instance = new ConnectionFactory
                    {
                        HostName = RabbitMqOption.Host,
                        Port = RabbitMqOption.Port,
                        UserName = RabbitMqOption.UserName,
                        Password = RabbitMqOption.Password,
                        AutomaticRecoveryEnabled = true,
                        DispatchConsumersAsync = true
                    }.CreateConnection();
                }
            }

            return _instance;
        }

        #endregion




    }
}
