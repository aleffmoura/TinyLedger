namespace TinyLedger.WebApi.Layers.Infra.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TinyLedger.WebApi.Layers.Domain;

public class AccountEntityConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.User).IsRequired();
        builder.Property(e => e.Balance).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasMany(a => a.Transactions)
            .WithOne()
            .HasForeignKey(a => a.AccountId);

        builder.HasData(new Account
        {
            Id = 1,
            Balance = 1000,
            CreatedAt = DateTime.UtcNow,
            User = "testing user",
        });
    }
}
