using Kafka.Example.Filters;
using Kafka.Example.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.Example.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDepencies(this IServiceCollection services)
        {
            services.AddSingleton<KafkaLogService>();
            services.AddSingleton<FileLogService>();
            services.AddSingleton<TimeTracker>();

            return services;
        }
    }
}