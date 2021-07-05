using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Timeout;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Polly.Api.Configurations.Policies
{
    public static class PoliciesConfiguration
    {

        public static IAsyncPolicy<HttpResponseMessage> GetDynamicPolicyAsync(string policyName, int? waitAndRetryNumberretries,
            int? waitAndRetryTimeToNewRequest, int? circuitBreakerEventsAllowedBeforeBreaking, int? circuitBreakerDurationOfBreakMs, 
            int? timeOutMs)
        {
            List<IAsyncPolicy<HttpResponseMessage>> listPolicies = new List<IAsyncPolicy<HttpResponseMessage>>();
            IAsyncPolicy<HttpResponseMessage> fallbackPolicy = GetFallBackPolicyAsync(HttpStatusCode.InternalServerError);
            AsyncTimeoutPolicy<HttpResponseMessage> timeOutPolicy = GetTimeOutPolicyAsync(timeOutMs.Value);
            IAsyncPolicy<HttpResponseMessage> waitAndRetryPolicy;

            switch (policyName)
            {
                case "Timeout":
                    listPolicies.Add(fallbackPolicy);
                    listPolicies.Add(timeOutPolicy);
                    break;

                case "WaitAndRetry":
                    waitAndRetryPolicy = GetWaitAndRetryPolicyAsync(waitAndRetryNumberretries.Value, waitAndRetryTimeToNewRequest.Value);
                    listPolicies.Add(fallbackPolicy);
                    listPolicies.Add(waitAndRetryPolicy);
                    listPolicies.Add(timeOutPolicy);
                    break;

                case "CircuitBreaker":
                    waitAndRetryPolicy = GetWaitAndRetryPolicyAsync(waitAndRetryNumberretries.Value, waitAndRetryTimeToNewRequest.Value);
                    IAsyncPolicy<HttpResponseMessage> circuitBreakerPolicy = GetCircuitBreakerPolicyAsync(circuitBreakerEventsAllowedBeforeBreaking.Value, circuitBreakerDurationOfBreakMs.Value);
                    listPolicies.Add(fallbackPolicy);
                    listPolicies.Add(circuitBreakerPolicy);
                    listPolicies.Add(waitAndRetryPolicy);
                    listPolicies.Add(timeOutPolicy);
                    break;
            }
            return Policy.WrapAsync(listPolicies.ToArray());
        }

        public static IAsyncPolicy<HttpResponseMessage> GetWaitPolicyAsync(int numberRetrys)
        {
            return Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .RetryAsync(numberRetrys, (exception, retryCount, context) =>
                {
                    string message = $"{nameof(GetWaitPolicyAsync)} - RetryCount: {retryCount} DateTime: {DateTime.Now} StatusCode: " +
                    $"{exception.Result.StatusCode}";
                    Log.Warning(message);
                });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetWaitAndRetryPolicyAsync(int numberRetrys, int waitTime)
        {
            return Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(numberRetrys, retryAttempt => TimeSpan.FromMilliseconds(waitTime),
                (exception, timeSpan, retryCount, context) => 
                {
                    if(exception?.Result?.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        //var newAccessToken = await RefreshAccessToken(context["refresh_token"].ToString());
                        //context["access_token"] = newAccessToken;
                    }
                    string message = $"{nameof(GetWaitAndRetryPolicyAsync)} - RetryCount: {retryCount} DateTime: {DateTime.Now} " +
                    $"StatusCode: {exception?.Result?.StatusCode}";
                    Log.Warning(message);
                });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicyAsync(int maxNumberAllowedBeforeBreaking, 
            int durationOfBreakMilliseconds)
        {
            return Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(maxNumberAllowedBeforeBreaking, TimeSpan.FromMilliseconds(durationOfBreakMilliseconds), 
                CircuitBreakerOnBreak, CircuitBreakerOnReset);
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerAndFallBackPolicyAsync(int maxNumberAllowedBeforeBreaking, int durationOfBreakMilliseconds)
        {
            List<IAsyncPolicy<HttpResponseMessage>> listPolicies = new List<IAsyncPolicy<HttpResponseMessage>>
            {
                GetFallBackPolicyAsync(HttpStatusCode.InternalServerError),
                GetCircuitBreakerPolicyAsync(maxNumberAllowedBeforeBreaking, durationOfBreakMilliseconds)
            };
            return Policy.WrapAsync(listPolicies.ToArray());
        }

        public static IAsyncPolicy<HttpResponseMessage> GetFallBackPolicyAsync(HttpStatusCode httpStatusCode)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(httpStatusCode);
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<BrokenCircuitException>()
                .Or<TimeoutRejectedException>()
                .FallbackAsync(httpResponseMessage);
        }

        public static AsyncTimeoutPolicy<HttpResponseMessage> GetTimeOutPolicyAsync(int timeOutMs)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(timeOutMs), TimeoutStrategy.Optimistic, 
                onTimeoutAsync: TimeoutLogInformationAsync);
        }

        public static AsyncBulkheadPolicy<HttpResponseMessage> GetBulkheadPolicyAsync(int maxParallelization, int maxQueuingActions)
        {
            return Policy.BulkheadAsync<HttpResponseMessage>(maxParallelization, maxQueuingActions, 
                onBulkheadRejectedAsync: BulkheadRejectedAsync());
        }

        private static Func<Context, Task> BulkheadRejectedAsync()
        {
            return (context) =>
            {
                Log.Warning("BulkHead");
                return Task.CompletedTask;
            };
        }

        private static Task TimeoutLogInformationAsync(Context context, TimeSpan timeSpan, Task task)
        {
            task?.ContinueWith(t =>
            {
                if (t != null)
                {
                    if (t.IsFaulted)
                    {
                        var msg = $"The execution timed out after {timeSpan.TotalSeconds} seconds, eventually terminated with: {t.Exception}.";
                        Console.WriteLine(msg);
                    }
                    else if (t.IsCanceled)
                    {
                        var msg = $"The execution timed out after {timeSpan.TotalSeconds} seconds, task cancelled.";
                        Console.WriteLine(msg);
                    }

                }
            });
            return task;
        }

        private static void CircuitBreakerOnReset()
        {
            Log.Warning("Reset - Circuit closed, requests flow normally.");
        }

        private static void CircuitBreakerOnBreak(DelegateResult<HttpResponseMessage> result, TimeSpan ts)
        {
            Log.Warning("Break - Circuit cut, requests will not flow.");
        }
    }
}
