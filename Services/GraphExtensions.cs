using GoogleMessage.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GoogleMessage.Services
{
    public static class GraphModuleExtensions
    {
        public static IServiceCollection AddPublicFactory(
            this IServiceCollection services,
            Action<PublicClientOption> option)
        {
            var clientOption = new PublicClientOption();
            option(clientOption);

            var publicClientFactory = new GraphPublicClientFactory(clientOption);

            services.AddSingleton<IGraphPublicClientFactory>(publicClientFactory);

            return services;
        }

        public static IServiceCollection AddConfidentialFactory(
            this IServiceCollection services,
            Action<ConfidentialAppOption> option)
        {
            services.RegisterGraphServices();

            var clientOption = new ConfidentialAppOption();
            option(clientOption);

            var confidentialClientFactory = new GraphConfidentialClientFactory(clientOption);

            services.AddSingleton<IGraphConfidentialClientFactory>(confidentialClientFactory);

            return services;
        }

        public static IServiceCollection AddPresenceHostService(this IServiceCollection services)
        {
            services.RegisterGraphServices();            
            return services;
        }

        internal static IServiceCollection RegisterGraphServices(this IServiceCollection services)
        {            
            return services;
        }
    }
}
