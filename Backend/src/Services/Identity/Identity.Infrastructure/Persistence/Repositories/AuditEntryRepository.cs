namespace Identity.Infrastructure.Persistence.Repositories;

internal sealed class AuditEntryRepository(IdentityDbContext context) : IAuditEntryRepository
{
    public void Add(AuditEntry entry) => context.AuditEntries.Add(entry);
}