namespace TinyLedger.WebApi.Layers.Infra;
using Microsoft.EntityFrameworkCore;
using TinyLedger.WebApi.Layers.Domain;

public class ReadRepository<Entity>(LedgerContext ledgerContext) : IReadRepository<Entity> where Entity : EntityBase<Entity>
{
    public IQueryable<Entity> GetAll(Func<Entity, bool> predicate, CancellationToken cancellationToken)
        => ledgerContext
        .Set<Entity>()
        .AsNoTracking()
        .Where(predicate)
        .AsQueryable();

    public async Task<Entity?> GetById(int id, CancellationToken cancellationToken)
        => await ledgerContext
        .Set<Entity>()
        .AsNoTracking()
        .FirstAsync(x => x.Id == id, cancellationToken);
}
