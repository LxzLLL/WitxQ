using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WitxQ.EventBus.RabbitMQ
{
    public static class ServicesCollectionExtension
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, Action<RabbitMqOption> mqConfig,
            Action<ExchangeOption> exchangeConfig = null, Action<QueueOption> queueConfig = null)
        {
            services.AddSingleton(ConfigureRabbitMqOption(mqConfig));
            services.AddSingleton(ConfigureExchangeOption(exchangeConfig));
            services.AddSingleton(ConfigureQueueOption(queueConfig));
            services.AddSingleton<IConnectionFactory, RabbitMqConnectionFactory>();
            services.AddSingleton<IMessageConsumer, RabbitMqMessageConsumer>();

            return services;
        }

        public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services, Action<IEventBus, IServiceProvider> subscribeAction = null)
        {
            services.AddSingleton<RabbitMqEventBus>();
            services.AddSingleton<IEventBus>(sp =>
            {
                var eventBus = sp.GetRequiredService<RabbitMqEventBus>();
                subscribeAction?.Invoke(eventBus, sp);
                return eventBus;
            });

            return services;
        }


        private static RabbitMqOption ConfigureRabbitMqOption(Action<RabbitMqOption> mqConfig)
        {
            var option = new RabbitMqOption();
            mqConfig?.Invoke(option);
            return option;
        }

        private static ExchangeOption ConfigureExchangeOption(Action<ExchangeOption> exchangeConfig)
        {
            var option = new ExchangeOption();
            exchangeConfig?.Invoke(option);
            return option;
        }

        private static QueueOption ConfigureQueueOption(Action<QueueOption> queueConfig)
        {
            var option = new QueueOption();
            queueConfig?.Invoke(option);
            return option;
        }
    }
}
