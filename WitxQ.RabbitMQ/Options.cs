
using RabbitMQ.Client;

namespace WitxQ.EventBus.RabbitMQ
{
    /// <summary>
    /// Exchange的option参数
    /// </summary>
    public class ExchangeOption
    {
        /// <summary>
        /// 交换器的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 交换器的类型，常见的如fanout、direct、topic
        /// </summary>
        public string Type { get; set; } = ExchangeType.Direct;

        /// <summary>
        /// 设置是否持久化。
        /// durable设置为true表示持久化，反之是非持久化。持久化可以将交换器存盘，在服务器重启的时候不会丢失相关信息
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// 设置是否自动删除。
        /// autoDelete设置为true则表示自动删除。
        /// 自动删除的前提是至少有一个队列或者交换器与这个交换器绑定，之后所有与这个交换器绑定的队列或者交换器都与此解绑才会删除。
        /// </summary>
        public bool AutoDelete { get; set; }

        /// <summary>
        /// 其他一些结构化参数
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

    }


    /// <summary>
    /// 队列的option参数
    /// </summary>
    public class QueueOption
    {
        /// <summary>
        /// 队列的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 设置是否持久化。
        /// 为true则设置队列为持久化。持久化的队列会存盘，在服务器重启的时候可以保证不丢失相关信息。
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// 设置是否排他。
        /// 为true则设置队列为排他的。
        /// </summary>
        public bool Exclusive { get; set; }

        /// <summary>
        /// 设置是否自动删除。
        /// 为true则设置队列为自动删除。自动删除的前提是：至少有一个消费者连接到这个队列，之后所有与这个队列连接的消费者都断开时，才会自动删除。
        /// </summary>
        public bool AutoDelete { get; set; }

        /// <summary>
        /// 设置队列的其他一些参数，如x-message-ttl、x-expires、x-max-length、x-max-length-bytes、x-dead-letter-exchange、x-dead-letter-routing-key、x-max-priority等。
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// rabbitmq的连接参数
    /// </summary>
    public class RabbitMqOption
    {
        /// <summary>
        /// 主机名
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }


}
