using Identity.Api.Contracts.Positions;
using Identity.Application.Features.Positions.CreatePosition;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/positions")]
[Authorize(Roles = "Chairman")]
public sealed class PositionsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreatePositionResponse>> Create(CreatePositionRequest request, CancellationToken cancellationToken)
    {
        var membershipId = await mediator.Send(new CreatePositionCommand(request.Role, request.Email), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new CreatePositionResponse(membershipId));
    }
}