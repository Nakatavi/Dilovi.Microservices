namespace Identity.Application.Abstractions;

public interface IAuditEntryRepository
{
    void Add(AuditEntry entry);
}