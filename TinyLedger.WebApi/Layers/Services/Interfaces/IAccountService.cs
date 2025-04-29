namespace TinyLedger.WebApi.Layers.Services.Interfaces;

using TinyLedger.WebApi.Layers.Domain;
using TinyLedger.WebApi.Layers.Services.Commands;

public interface IAccountService
{
    Task<int> CreateAsync(AccountCommand cmd, CancellationToken cancellationToken);
    Task<Account?> GetByIdAsync(int accountId, CancellationToken cancellationToken);
    Task Transfer(AccountTransferCommand cmd, CancellationToken cancellationToken);
}
