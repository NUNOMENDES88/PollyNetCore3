using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly.Api.Extensions;
using Polly.Api.Models.Counters;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Polly.Api.Controllers
{
    public class ReactivePoliciesController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ReactivePoliciesController> _logger;

        public ReactivePoliciesController(IHttpClientFactory clientFactory, ILogger<ReactivePoliciesController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [HttpGet("reactivePolicies/retry")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRetryPolicy()
        {
            var clientHttp = _clientFactory.CreateClient("RetryPolicy");
            for (int i = 0; i < 10; i++)
            {
                _logger.LogWarning($"{nameof(GetRetryPolicy)} Id {i}");
                PostMultipleRequest request = new PostMultipleRequest()
                {
                    Number = i,
                    MultipleNumber = 3
                };

                string requestJsonString = request.SerializeObj();
                var responsePost = await clientHttp.PostAsync("/counters", requestJsonString.ToStringContent()).ConfigureAwait(false);
                string result = responsePost.Content.ReadAsStringAsync().Result;
                if (responsePost.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"{nameof(GetRetryPolicy)} Retornou Ok");
                }
                else
                {
                    _logger.LogWarning($"{nameof(GetRetryPolicy)} Retornou Erro");
                }
            }
            return Ok("ok");
        }

        [HttpGet("reactivePolicies/waitAndretry")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWaitAndRetryPolicy()
        {
            var clientHttp = _clientFactory.CreateClient("WaitAndRetryPolicy");
            for (int i = 0; i < 10; i++)
            {
                _logger.LogWarning($"{nameof(GetWaitAndRetryPolicy)} Id {i}");
                PostMultipleRequest request = new PostMultipleRequest()
                {
                    Number = i,
                    MultipleNumber = 3
                };

                string requestJsonString = request.SerializeObj();
                var responsePost = await clientHttp.PostAsync("/counters", requestJsonString.ToStringContent()).ConfigureAwait(false);
                string result = responsePost.Content.ReadAsStringAsync().Result;
                if (responsePost.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"{nameof(GetWaitAndRetryPolicy)} Retornou Ok");
                }
                else
                {
                    _logger.LogWarning($"{nameof(GetWaitAndRetryPolicy)} Retornou Erro");
                }
            }
            return Ok("ok");
        }


        [HttpGet("reactivePolicies/circuitBreaker")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCircuitBreakerPolicy()
        {
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    var clientHttp = _clientFactory.CreateClient("GetCircuitBreakerPolicy");
                    _logger.LogWarning($"{nameof(GetCircuitBreakerPolicy)} Id {i}");
                    PostMultipleRequest request = new PostMultipleRequest()
                    {
                        Number = 2,
                        MultipleNumber = 2
                    };

                    string requestJsonString = request.SerializeObj();
                    var responsePost = await clientHttp.PostAsync("/counters", requestJsonString.ToStringContent()).ConfigureAwait(false);
                    string result = responsePost.Content.ReadAsStringAsync().Result;
                    if (responsePost.IsSuccessStatusCode)
                    {
                        _logger.LogWarning($"{nameof(GetCircuitBreakerPolicy)} Retornou Ok");
                    }
                    else
                    {
                        _logger.LogWarning($"{nameof(GetCircuitBreakerPolicy)} Retornou Erro");
                    }
                }
                catch (Exception)
                {
                    _logger.LogWarning($"{nameof(GetCircuitBreakerPolicy)} CircuitBreaker");
                }
            }
            return Ok("ok");
        }

        [HttpGet("reactivePolicies/fallBack")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCircuitBreakerAndFallBackPolicy()
        {
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    var clientHttp = _clientFactory.CreateClient("GetCircuitBreakerAndFallBackPolicy");
                    _logger.LogWarning($"{nameof(GetCircuitBreakerAndFallBackPolicy)} Id {i}");
                    PostMultipleRequest request = new PostMultipleRequest()
                    {
                        Number = 2,
                        MultipleNumber = 2
                    };

                    string requestJsonString = request.SerializeObj();
                    var responsePost = await clientHttp.PostAsync("/counters", requestJsonString.ToStringContent()).ConfigureAwait(false);
                    if (responsePost.IsSuccessStatusCode)
                    {
                        _logger.LogWarning($"{nameof(GetCircuitBreakerAndFallBackPolicy)} Retornou Ok");
                    }
                    else
                    {
                        _logger.LogWarning($"{nameof(GetCircuitBreakerAndFallBackPolicy)} Retornou Erro StatusCode: {responsePost.StatusCode}");
                    }
                }
                catch (Exception)
                {
                    _logger.LogWarning($"{nameof(GetCircuitBreakerAndFallBackPolicy)} CircuitBreaker");
                }
            }
            return Ok("ok");
        }
    }
}
