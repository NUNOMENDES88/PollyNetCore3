using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Polly.Api.Configurations.Documentation;
using Polly.Api.DependencyInjection;

namespace Polly.Api.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddCustomConfigureServices(this IServiceCollection services)
        {
            services.AddCustomApiDocumentation();
            services.AddHttpClientDependencyRegister();
        }

        public static void AddCustomConfigure(this IApplicationBuilder app)
        {
            app.UseCustomApiDocumentation();
        }
    }
}
