using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WitxQ.EventBus
{
    /// <summary>
    /// 事件总线接口
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="event">事件</param>
        /// <param name="hander">事件的处理程序</param>
        void Subscribe<T, TH>(T @event, TH hander)
            where T:IntegrationEvent
            where TH:IEventHandler<T>;

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="event">事件</param>
        /// <param name="hander">事件的处理程序</param>
        void Unsubscribe<T, TH>(T @event, TH hander) 
            where T : IntegrationEvent
            where TH : IEventHandler<T>;

        /// <summary>
        /// 取消事件的所有订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event">事件</param>
        void UnsubscribeAll<T>(T @event)
            where T : IntegrationEvent;

        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventData"></param>
        /// <returns></returns>
        Task PublishAsync<TEvent>(TEvent eventData) where TEvent : IntegrationEvent;
    }
}
