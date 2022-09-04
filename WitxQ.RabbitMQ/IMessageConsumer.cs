using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WitxQ.EventBus;


namespace WitxQ.EventBus.RabbitMQ
{
    /// <summary>
    /// 消息的消费者接口
    /// </summary>
    public interface IMessageConsumer
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        IMessageConsumer Initialize();

        /// <summary>
        /// 绑定事件
        /// </summary>
        /// <typeparam name="T">IntegrationEvent</typeparam>
        /// <param name="e">事件</param>
        void Bind<T>(T e) where T:IntegrationEvent;

        /// <summary>
        /// 解绑事件
        /// </summary>
        /// <typeparam name="T">IntegrationEvent</typeparam>
        /// <param name="e">事件</param>
        void Unbind<T>(T e) where T : IntegrationEvent;

        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="callback"></param>
        void OnMessageReceived(Func<IModel, BasicDeliverEventArgs, Task> callback);
    }
}
