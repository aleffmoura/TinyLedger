namespace TinyLedger.WebApi.Layers.Infra;

using Microsoft.EntityFrameworkCore;
using TinyLedger.WebApi.Layers.Domain;

public class WriteRepository<T>(LedgerContext ledgerContext) : IWriteRepository<T> where T : EntityBase<T>
{
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken)
    {
        var newEntity = await ledgerContext.Set<T>().AddAsync(entity, cancellationToken);

        await ledgerContext.SaveChangesAsync(cancellationToken);

        return newEntity.Entity;
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        _ = ledgerContext.Set<T>()
                    .Entry(entity)
                    .State = EntityState.Modified;

        await ledgerContext.SaveChangesAsync(cancellationToken);

        return entity;
    }
}