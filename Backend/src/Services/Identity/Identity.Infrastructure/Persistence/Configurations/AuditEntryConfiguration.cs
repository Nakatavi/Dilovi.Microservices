namespace Identity.Infrastructure.Persistence.Configurations;

internal sealed class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.Action).HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.ActorEmail).HasMaxLength(256).IsRequired();
        builder.Property(a => a.Details).HasMaxLength(1000);

        builder.HasIndex(a => new { a.TenantId, a.OccurredAt });
    }
}