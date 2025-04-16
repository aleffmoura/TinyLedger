namespace TinyLedger.WebApi.Layers.Infra.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TinyLedger.WebApi.Layers.Domain;

public class TransactEntityConfiguration : IEntityTypeConfiguration<Transact>
{
    public void Configure(EntityTypeBuilder<Transact> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.AccountId).IsRequired();
        builder.Property(e => e.Amount).IsRequired();
        builder.Property(e => e.Description);
        builder.Property(e => e.CreatedAt).IsRequired();
    }
}
