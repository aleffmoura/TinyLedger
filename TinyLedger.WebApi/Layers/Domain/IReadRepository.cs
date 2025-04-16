namespace TinyLedger.WebApi.Layers.Domain;
public interface IReadRepository<T> where T : EntityBase<T>
{
    Task<T?> GetById(int id, CancellationToken cancellationToken);
    IQueryable<T> GetAll(Func<T, bool> predicate, CancellationToken cancellationToken);
}