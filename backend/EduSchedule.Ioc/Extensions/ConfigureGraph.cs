using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

namespace EduSchedule.Ioc.Extensions
{
    public static class ConfigureGraph
    {
        public static IServiceCollection AddGraph(this IServiceCollection services, IConfiguration configuration)
        {
            var azureAdConfig = configuration.GetSection("AzureAd");

            var tenantId = configuration["AzureAd:TenantId"];

            if (string.IsNullOrEmpty(tenantId))
            {
                throw new InvalidOperationException("A configuração 'AzureAd:TenantId' não foi injetada corretamente.");
            }

            var credential = new ClientSecretCredential(
                azureAdConfig["TenantId"],
                azureAdConfig["ClientId"],
                azureAdConfig["ClientSecret"]
            );

            services.AddScoped(sp => new GraphServiceClient(credential));

            return services;
        }
    }
}
