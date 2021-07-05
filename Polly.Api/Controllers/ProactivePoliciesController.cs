using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly.Api.Extensions;
using Polly.Api.Models.Counters;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Polly.Api.Controllers
{
    public class ProactivePoliciesController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ProactivePoliciesController> _logger;

        public ProactivePoliciesController(IHttpClientFactory clientFactory, ILogger<ProactivePoliciesController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [HttpGet("proactivePolicies/timeOut")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTimeOutPolicy()
        {
            var clientHttp = _clientFactory.CreateClient("GetTimeOutPolicy");
            for (int i = 0; i < 6; i++)
            {
                _logger.LogWarning($"{nameof(GetTimeOutPolicy)} Id {i}");
                PostMultipleRequest request = new PostMultipleRequest()
                {
                    Number = i,
                    MultipleNumber = 3,
                    TimeOut = i < 4 ? 0 : i * 1000
                };
                try
                {
                    string requestJsonString = request.SerializeObj();
                    var responsePost = await clientHttp.PostAsync("/counters", requestJsonString.ToStringContent()).ConfigureAwait(false);
                    string result = responsePost.Content.ReadAsStringAsync().Result;
                    if (responsePost.IsSuccessStatusCode)
                    {
                        _logger.LogWarning($"{nameof(GetTimeOutPolicy)} Retornou Ok");
                    }
                    else
                    {
                        _logger.LogWarning($"{nameof(GetTimeOutPolicy)} Retornou Erro");
                    }
                }
                catch (System.Exception)
                {
                    _logger.LogWarning($"{nameof(GetTimeOutPolicy)} - TimeOut Exception");
                }
            }
            return Ok("ok");
        }


        [HttpGet("proactivePolicies/bulkhead")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBulkheadPolicy()
        {
            List<Task> listTasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                listTasks.Add(GetClientAsync(i));
            }

            await Task.WhenAll(listTasks).ConfigureAwait(false);
            return Ok("ok");
        }

        private async Task GetClientAsync(int i)
        {
            var clientHttp = _clientFactory.CreateClient("GetBulkheadPolicy");
            _logger.LogWarning($"{nameof(GetTimeOutPolicy)} Id {i}");
            PostMultipleRequest request = new PostMultipleRequest()
            {
                Number = i,
                MultipleNumber = 3,
                TimeOut = 0
            };
            try
            {
                string requestJsonString = request.SerializeObj();
                var responsePost = await clientHttp.PostAsync("/counters", requestJsonString.ToStringContent()).ConfigureAwait(false);
                string result = responsePost.Content.ReadAsStringAsync().Result;
                if (responsePost.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"{nameof(GetBulkheadPolicy)} - {i} Retornou Ok");
                }
                else
                {
                    _logger.LogWarning($"{nameof(GetBulkheadPolicy)} - {i}  Retornou Erro");
                }
            }
            catch (System.Exception)
            {
                _logger.LogWarning($"{nameof(GetBulkheadPolicy)} - {i} - BulkHead Exception");
            }
        }
    }
}
