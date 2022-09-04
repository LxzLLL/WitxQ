using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WitxQ.EventBus
{
    /// <summary>
    /// IEventHandler事件处理程序接口
    /// </summary>
    public interface IEventHandler:IHander
    {
        /// <summary>
        /// 异步回调task
        /// </summary>
        /// <param name="eventData">事件所需数据</param>
        /// <returns></returns>
        Task InvokeAsync<T>(T @event);
    }

    /// <summary>
    /// hander处理器接口
    /// </summary>
    public interface IHander
    {

    }
}
