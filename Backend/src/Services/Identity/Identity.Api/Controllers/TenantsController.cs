using Identity.Api.Contracts.Tenants;
using Identity.Application.Features.Tenants.RegisterTenant;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/tenants")]
[Authorize(Roles = "SuperAdmin")]
public sealed class TenantsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RegisterTenantResponse>> Register(RegisterTenantRequest request, CancellationToken cancellationToken)
    {
        var tenantId = await mediator.Send(new RegisterTenantCommand(request.Name, request.ChairmanEmail), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new RegisterTenantResponse(tenantId));
    }
}