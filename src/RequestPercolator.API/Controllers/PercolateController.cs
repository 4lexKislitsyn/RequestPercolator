using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestPercolator.API.Dto;
using RequestPercolator.API.Proxy;
using RequestPercolator.Model;
using RequestPercolator.Model.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestPercolator.API.Controllers
{
    [ApiController]
    [Route("percolate")]
    public sealed class PercolateController : ControllerBase
    {
        private const string FilterIdTemplate = "{filterId}";
        private readonly IPercolationService percolationService;

        public PercolateController(IPercolationService percolationService)
        {
            this.percolationService = percolationService;
        }

        [HttpPut(FilterIdTemplate)]
        [HttpPost(FilterIdTemplate)]
        [HttpPatch(FilterIdTemplate)]
        public Task<IActionResult> PercolateByFilterAsync([FromRoute] Guid filterId, [FromQuery] PercolationDto percolation, CancellationToken cancellationToken)
        {
            return HandlePercolationResult(percolation, () => percolationService.PercolateRequestAsync(Request, filterId, cancellationToken));
        }

        [HttpPut]
        [HttpPost]
        [HttpPatch]
        public Task<IActionResult> PercolateAsync([FromQuery] PercolationWithFilterDto percolation, CancellationToken cancellationToken)
        {
            return HandlePercolationResult(percolation, () => percolationService.PercolateRequestAsync(Request, percolation.Filter, cancellationToken));
        }

        private async Task<IActionResult> HandlePercolationResult(PercolationDto percolation, Func<Task<PercolationResult>> action)
        {
            var result = await action();
            return result switch
            {
                PercolationResult.ShouldBeReversed => new ReversedProxyResult(percolation.Destination),
                PercolationResult.ShouldBeSkipped => new ObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status202Accepted,
                    Title = "Not reversed",
                    Detail = "Request doesn't fit with filter and wasn't reversed to destination host"
                }),
                _ => throw new NotImplementedException("Not implemented percolation result"),
            };
        }
    }
}
