namespace TinyLedger.WebApi.Layers.Services.Interfaces;
using TinyLedger.WebApi.Layers.Domain;
using TinyLedger.WebApi.Layers.Services.Commands;

public interface ITransactService
{
    Task<int> DepositAsync(TransactCommand transaction, CancellationToken cancellationToken);
    Task<int> WithdrawAsync(TransactCommand transaction, CancellationToken cancellationToken);
    Task<IQueryable<Transact>> GetByAccountAsync(int accountId, CancellationToken cancellationToken);
}
