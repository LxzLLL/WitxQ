
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WitxQ.EventBus.RabbitMQ
{
    /// <summary>
    /// RabbitM 消息的消费者
    /// </summary>
    public class RabbitMqMessageConsumer : IMessageConsumer
    {
        /// <summary>
        /// 交换机参数
        /// </summary>
        public ExchangeOption ExchangeOption { get; private set; }

        /// <summary>
        /// 队列参数
        /// </summary>
        public QueueOption QueueOption { get; private set; }

        /// <summary>
        /// 连接工厂
        /// </summary>
        private readonly IConnectionFactory _connectionFactory;

        /// <summary>
        /// 通道
        /// </summary>
        public IModel Channel { get; private set; }

        /// <summary>
        /// 事件触发委托列表
        /// </summary>
        protected ConcurrentBag<Func<IModel, BasicDeliverEventArgs, Task>> Callbacks { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="exchangeOptions">交换机参数</param>
        /// <param name="queueOption">队列参数</param>
        /// <param name="connectionFactory">连接工厂</param>
        public RabbitMqMessageConsumer(ExchangeOption exchangeOptions, QueueOption queueOption, IConnectionFactory connectionFactory)
        {
            this.ExchangeOption = exchangeOptions;
            this.QueueOption = queueOption;
            this._connectionFactory = connectionFactory;
            this.Callbacks=new ConcurrentBag<Func<IModel, BasicDeliverEventArgs, Task>>();
        }


        #region IMessageConsumer
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public IMessageConsumer Initialize()
        {
            this.Channel?.Dispose();

            // create channel
            Channel = this._connectionFactory.GetConnection().CreateModel();

            // exchange declare
            Channel.ExchangeDeclare(this.ExchangeOption.Name, this.ExchangeOption.Type
                , this.ExchangeOption.Durable, this.ExchangeOption.AutoDelete, this.ExchangeOption.Arguments);
            // queuq declare
            Channel.QueueDeclare(this.QueueOption.Name,this.QueueOption.Durable,this.QueueOption.Exclusive
                , this.QueueOption.AutoDelete, this.QueueOption.Arguments);

            AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(Channel);
            consumer.Received += HandleIncomingMessageAsync;

            Channel.BasicConsume(
                QueueOption.Name,
                autoAck: false,
                consumer: consumer
            );
            return this;

        }

        /// <summary>
        /// 绑定事件。
        /// 将事件的EventName作为routingKey
        /// </summary>
        /// <typeparam name="T">IntegrationEvent</typeparam>
        /// <param name="e">事件</param>
        public void Bind<T>(T e) where T : IntegrationEvent
        {
            this.Channel.QueueBind(this.QueueOption.Name, this.ExchangeOption.Name, e.EventName);
        }

        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="callback"></param>
        public void OnMessageReceived(Func<IModel, BasicDeliverEventArgs, Task> callback)
        {
            this.Callbacks.Add(callback);
        }

        /// <summary>
        /// 解绑事件
        /// </summary>
        /// <typeparam name="T">IntegrationEvent</typeparam>
        /// <param name="e">事件</param>
        public void Unbind<T>(T e) where T : IntegrationEvent
        {
            this.Channel.QueueUnbind(this.QueueOption.Name, this.ExchangeOption.Name, e.EventName);
        }
        #endregion

        /// <summary>
        /// message进来后的处理
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="basicDeliverEventArgs">事件参数</param>
        /// <returns></returns>
        protected virtual async Task HandleIncomingMessageAsync(object sender, BasicDeliverEventArgs eArgs)
        {
            try
            {
                foreach (var callback in this.Callbacks)
                {
                    await callback(this.Channel, eArgs);
                }

                this.Channel.BasicAck(eArgs.DeliveryTag, multiple: false);
            }
            catch
            {
                try
                {
                    Channel.BasicNack(
                        eArgs.DeliveryTag,
                        multiple: false,
                        requeue: true
                    );
                }
                catch
                {
                    // TODO:logging
                }
            }
        }
    }

}
