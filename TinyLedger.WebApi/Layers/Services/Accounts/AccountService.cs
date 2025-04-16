namespace TinyLedger.WebApi.Layers.Services.Accounts;
using TinyLedger.WebApi.Layers.Domain;
using TinyLedger.WebApi.Layers.Domain.Exceptions;
using TinyLedger.WebApi.Layers.Services.Commands;
using TinyLedger.WebApi.Layers.Services.Interfaces;

public class AccountService(IWriteRepository<Account> writeRepository, IReadRepository<Account> readRepository) : IAccountService
{
    public async Task<Account?> GetByIdAsync(int accountId, CancellationToken cancellationToken)
        => await readRepository.GetById(accountId, cancellationToken)
            ?? throw new NotFoundException($"Entity: '{nameof(Account)}' with Id: '{accountId}' not found.");

    public async Task<int> CreateAsync(AccountCommand cmd, CancellationToken cancellationToken)
    {
        var accountDb =
            readRepository.GetAll(x => x.User == cmd.User, cancellationToken)
            .FirstOrDefault();

        if (accountDb is not null)
            throw new AlreadyExistException($"Entity: '{nameof(Account)}' with User: '{cmd.User}' already exists.");

        var entity = await writeRepository.AddAsync(new ()
        {
            Balance = 0,
            CreatedAt = DateTime.UtcNow,
            User = cmd.User,
        }, cancellationToken);

        return entity.Id;
    }
}
