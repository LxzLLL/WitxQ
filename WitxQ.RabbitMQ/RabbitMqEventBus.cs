using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WitxQ.EventBus.RabbitMQ
{
    /// <summary>
    /// rabbitmq实现的eventbus
    /// </summary>
    public class RabbitMqEventBus : IEventBus
    {
        // Rabbitmq连接工厂
        private readonly IConnectionFactory _connectionFactory;
        // Rabbitmq消费者
        private readonly IMessageConsumer _consumer;
        // 交换机参数
        private readonly ExchangeOption _exchangeOption;

        // 事件处理程序与event的对应
        private readonly ConcurrentDictionary<Type, List<IEventHandler>> _handlers;

        // 事件名称（也是路由key）与event的对应，用于反射获取event
        private readonly Dictionary<string, Type> _evenTypes;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="connectionFactory">Rabbitmq连接工厂</param>
        /// <param name="consumer">Rabbitmq消费者</param>
        /// <param name="exchangeOption">交换机参数</param>
        public RabbitMqEventBus(IConnectionFactory connectionFactory, IMessageConsumer consumer, ExchangeOption exchangeOption)
        {
            _connectionFactory = connectionFactory;
            _consumer = consumer;
            _exchangeOption = exchangeOption;
            _evenTypes = new Dictionary<string, Type>();
            _handlers = new ConcurrentDictionary<Type, List<IEventHandler>>();
            Initialize();
        }

        /// <summary>
        /// 消费者init
        /// </summary>
        public void Initialize()
        {
            _consumer.Initialize();
            _consumer.OnMessageReceived(ProcessEventAsync);
        }


        #region IEventBus
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="@event"></param>
        /// <returns></returns>
        public Task PublishAsync<TEvent>(TEvent @event) 
            where TEvent : IntegrationEvent
        {
            //string eName = @event.EventName;
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

            using (var channel = _connectionFactory.GetConnection().CreateModel())
            {
                channel.ExchangeDeclare(
                    _exchangeOption.Name,
                    _exchangeOption.Type,
                    _exchangeOption.Durable,
                    _exchangeOption.AutoDelete,
                    _exchangeOption.Arguments
                );

                var properties = channel.CreateBasicProperties();
                // 消息的持久化在消息的投递模式
                properties.DeliveryMode = 1;  // 非持久性的

                // mandatory true时，交换器无法根据自动的类型和路由键找到一个符合条件的队列，
                // 那么RabbitMq会调用Basic.Ruturn命令将消息返回给生产者，
                // 为false时，出现上述情况消息被直接丢弃
                channel.BasicPublish(
                    _exchangeOption.Name,
                    @event.EventName,
                    false,
                    properties,
                    body
                );

            }
            return Task.CompletedTask;
        }


        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="event">事件</param>
        /// <param name="handler">事件的处理程序</param>
        public void Subscribe<T, TH>(T @event, TH handler)
            where T : IntegrationEvent
            where TH : IEventHandler
        {
            if (@event == null || handler == null)
                return;
            // 添加 event type下的 handler处理事件
            this._handlers.GetOrAdd(@event.GetType(), _ => new List<IEventHandler>()).Add(handler);

            _consumer.Bind(@event);
            if (!_evenTypes.ContainsKey(@event.EventName))
            {
                _evenTypes.Add(@event.EventName, @event.GetType());
            }
        }


        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="event">事件</param>
        /// <param name="handler">事件的处理程序</param>
        public void Unsubscribe<T, TH>(T @event, TH handler)
            where T : IntegrationEvent
            where TH : IEventHandler
        {
            if (@event == null || handler == null)
                return;

            if (this._handlers.TryGetValue(@event.GetType(), out var handlers))
            {
                handlers.Remove(handler);
            }
        }

        /// <summary>
        /// 取消事件的所有订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event">事件</param>
        public void UnsubscribeAll<T>(T @event)
            where T : IntegrationEvent
        {
            if (@event == null)
                return;

            if (this._handlers.TryGetValue(@event.GetType(), out var handlers))
            {
                handlers.Clear();
            }
        }


        #endregion


        #region private

        /// <summary>
        /// 回调处理程序
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="ea"></param>
        /// <returns></returns>
        private async Task ProcessEventAsync(IModel channel, BasicDeliverEventArgs ea)
        {
            string eventName = ea.RoutingKey;
            _evenTypes.TryGetValue(eventName, out var eventType);
            if (eventType == null)
            {
                return;
            }

            // 实例化的event
            var eventData = JsonSerializer.Deserialize(Encoding.UTF8.GetString(ea.Body.ToArray()), eventType);

            this._handlers.TryGetValue(eventType, out var handlers);
            if (handlers != null)
            {
                foreach (var h in handlers)
                {
                    await h.InvokeAsync(eventData);
                }
            }
        }

        #endregion



    }
}
