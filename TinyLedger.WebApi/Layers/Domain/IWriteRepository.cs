namespace TinyLedger.WebApi.Layers.Domain;

public interface IWriteRepository<T> where T : EntityBase<T>
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken);
}
