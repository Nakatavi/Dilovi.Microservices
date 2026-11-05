namespace Notification.Infrastructure.Persistence.Configurations;

internal sealed class EmailMessageConfiguration : IEntityTypeConfiguration<EmailMessage>
{
    public void Configure(EntityTypeBuilder<EmailMessage> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Recipient).HasMaxLength(256).IsRequired();
        builder.Property(m => m.LastError).HasMaxLength(2000);
        builder.Property(m => m.ProviderMessageId).HasMaxLength(200);

        builder.HasIndex(m => new { m.SourceEventId, m.Recipient }).IsUnique();
        builder.HasIndex(m => m.CreatedAt);
        builder.HasIndex(m => m.Recipient);
    }
}