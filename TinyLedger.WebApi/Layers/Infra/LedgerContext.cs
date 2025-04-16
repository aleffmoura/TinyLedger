namespace TinyLedger.WebApi.Layers.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TinyLedger.WebApi.Layers.Domain;
using TinyLedger.WebApi.Layers.Infra.EntityConfigurations;

public class LedgerContext : DbContext
{
    public virtual DbSet<Account> Customer { get; set; }
    public virtual DbSet<Transact> Transactions { get; set; }

    public LedgerContext(DbContextOptions<LedgerContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TransactEntityConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        optionsBuilder.LogTo(Console.WriteLine);
        base.OnConfiguring(optionsBuilder);
    }
}
