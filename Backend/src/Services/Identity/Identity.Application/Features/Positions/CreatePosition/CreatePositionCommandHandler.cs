
namespace Identity.Application.Features.Positions.CreatePosition;

public sealed class CreatePositionCommandHandler(
    IMembershipRepository memberships,
    InvitationIssuer invitationIssuer,
    IInvitationSender invitationSender,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<CreatePositionCommand, Guid>
{
    public async Task<Guid> Handle(CreatePositionCommand command, CancellationToken cancellationToken)
    {
        if (command.Role == MembershipRole.Chairman)
            throw new ValidationException(ApplicationErrors.Positions.ChairmanNotAllowed);

        var tenantId = currentUser.TenantId
            ?? throw new ValidationException(ApplicationErrors.Positions.TenantRequired);

        if (await memberships.ExistsAsync(tenantId, command.Role, cancellationToken))
            throw new ConflictException(ApplicationErrors.Positions.AlreadyExists);

        var now = timeProvider.GetUtcNow().UtcDateTime;

        var position = Membership.Create(tenantId, command.Role, now);
        memberships.Add(position);

        var token = await invitationIssuer.IssueAsync(position, command.Email, now, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await invitationSender.SendAsync(command.Email.Trim().ToLowerInvariant(), token, cancellationToken);

        return position.Id;
    }
}