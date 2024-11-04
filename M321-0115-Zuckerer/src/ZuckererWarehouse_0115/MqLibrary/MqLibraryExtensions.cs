using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace MqLibrary
{
    public static class MqLibraryExtensions
    {
        public static IServiceCollection AddMqLibraryConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

            return services;
        }

        public static IServiceCollection AddMqLibrary(this IServiceCollection services)
        {
            services.AddSingleton<RabbitMqClientAdapter>();
            services.AddSingleton<IModel>((services) => services.GetRequiredService<RabbitMqClientAdapter>().CreateModel());
            services.AddSingleton<MqSender>();

            return services;
        }
    }
}
