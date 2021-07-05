using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Polly.Api.Configurations.Policies;
using System;

namespace Polly.Api.DependencyInjection
{
    public static class HttpClientsDependencyInjection
    {
        private static readonly string _baseAddress = "https://localhost:44336/";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void AddHttpClientDependencyRegister(this IServiceCollection services)
        {
            services.AddHttpClient("RetryPolicy", clientConfig =>
            {
                clientConfig.BaseAddress = new Uri(_baseAddress);
                clientConfig.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json; charset=utf-8");
                clientConfig.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "Polly.Api");
            })
                .AddPolicyHandler(PoliciesConfiguration.GetWaitPolicyAsync(2));

            services.AddHttpClient("WaitAndRetryPolicy", clientConfig =>
            {
                clientConfig.BaseAddress = new Uri(_baseAddress);
                clientConfig.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json; charset=utf-8");
                clientConfig.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "Polly.Api");
            })
                .AddPolicyHandler(PoliciesConfiguration.GetWaitAndRetryPolicyAsync(2,1000));

            services.AddHttpClient("GetCircuitBreakerPolicy", clientConfig =>
            {
                clientConfig.BaseAddress = new Uri(_baseAddress);
                clientConfig.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json; charset=utf-8");
                clientConfig.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "Polly.Api");
            })
                .AddPolicyHandler(PoliciesConfiguration.GetCircuitBreakerPolicyAsync(2, 5000));

            services.AddHttpClient("GetCircuitBreakerAndFallBackPolicy", clientConfig =>
            {
                clientConfig.BaseAddress = new Uri(_baseAddress);
                clientConfig.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json; charset=utf-8");
                clientConfig.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "Polly.Api");
            })
                .AddPolicyHandler(PoliciesConfiguration.GetCircuitBreakerAndFallBackPolicyAsync(2, 5000));

            services.AddHttpClient("GetTimeOutPolicy", clientConfig =>
            {
                clientConfig.BaseAddress = new Uri(_baseAddress);
                clientConfig.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json; charset=utf-8");
                clientConfig.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "Polly.Api");
            })
                .AddPolicyHandler(PoliciesConfiguration.GetTimeOutPolicyAsync(5000));


            services.AddHttpClient("GetBulkheadPolicy", clientConfig =>
            {
                clientConfig.BaseAddress = new Uri(_baseAddress);
                clientConfig.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json; charset=utf-8");
                clientConfig.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "Polly.Api");
            })
                .AddPolicyHandler(PoliciesConfiguration.GetBulkheadPolicyAsync(1,3));
        }
    }
}
