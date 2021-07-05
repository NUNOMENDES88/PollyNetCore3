using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly.Test.Models.Counters;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Test.Controllers
{
    public class CountersController : Controller
    {
        [HttpPost("counters")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(string),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostMultiple([FromBody] PostMultipleRequest request, CancellationToken token = default)
        {
            var result = request.Number % request.MultipleNumber == 0;
            if(request.TimeOut > 0)
            {
                await Task.Delay(request.TimeOut);
            }

            if(result)
            {
                return BadRequest($"Multiplo de {request.MultipleNumber}");
            }
            return Ok($"Não é multiplo de {request.MultipleNumber}");
        }
    }
}
