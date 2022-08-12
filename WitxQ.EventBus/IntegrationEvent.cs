using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WitxQ.EventBus
{
    /// <summary>
    /// 事件基类
    /// </summary>
    public record class IntegrationEvent
    {
        /// <summary>
        /// 事件guid
        /// </summary>
        public Guid Id { get; private init; }

        /// <summary>
        /// 创建时间（utc时间）
        /// </summary>
        public DateTime CreateTime { get; private init; }

        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; private init; }


        public IntegrationEvent()
        {
            this.Id = Guid.NewGuid();
            this.CreateTime = DateTime.UtcNow;
            this.EventName = "IntegrationEvent";
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="id">guid</param>
        /// <param name="createTime">创建时间（utc）</param>
        /// <param name="eventName">事件名称</param>
        public IntegrationEvent(Guid id, DateTime createTime, string eventName)
        {
            this.Id = id;
            this.CreateTime = createTime;
            this.EventName = eventName;
        }
    }
}
