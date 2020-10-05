using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SampleApiWebApp.Constants;
using SampleApiWebApp.Controllers.Teams.Post;
using SampleApiWebApp.Infrastructure;

namespace SampleApiWebApp.Controllers.Teams
{
    [ApiController]
    [Route("[controller]")]
    public sealed class TeamsController : Controller
    {
        private readonly IMediator mediator;

        public TeamsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("{id}")]
        [Consumes(ContentTypes.ApplicationJson)]
        [Produces(ContentTypes.ApplicationJson)]
        [ProducesResponseType(200, Type = typeof(Team))]
        [ProducesResponseType(404)]
        public Task<IActionResult> GetOne([FromRoute] long id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Consumes(ContentTypes.ApplicationJson)]
        [Produces(ContentTypes.ApplicationJson)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> Post(
            [FromBody] PostTeamCommand request,
            CancellationToken cancellationToken = default)
        {
            var result = await this.mediator.Send(request, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut]
        [Route("{id}")]
        [Consumes(ContentTypes.ApplicationJson)]
        [Produces(ContentTypes.ApplicationJson)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(404)]
        public Task<IActionResult> Put(
            [FromRoute] long id,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
