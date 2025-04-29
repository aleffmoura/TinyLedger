namespace TinyLedger.WebApi.Layers.Services.Accounts;
using TinyLedger.WebApi.Layers.Domain;
using TinyLedger.WebApi.Layers.Domain.Exceptions;
using TinyLedger.WebApi.Layers.Services.Commands;
using TinyLedger.WebApi.Layers.Services.Interfaces;

public class AccountService(
    IWriteRepository<Account> writeRepository,
    IReadRepository<Account> readRepository,
    ITransactService transactService) : IAccountService
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

        var entity = await writeRepository.AddAsync(new()
        {
            Balance = 0,
            CreatedAt = DateTime.UtcNow,
            User = cmd.User,
        }, cancellationToken);

        return entity.Id;
    }
    public async Task Transfer(AccountTransferCommand cmd, CancellationToken cancellationToken)
    {
        if (cmd.AccountFromId == cmd.AccountToId)
            throw new InvalidOperationException($"'{nameof(Account)}s' cant be the same.");

        var from =
             await readRepository.GetById(cmd.AccountFromId, cancellationToken)
             ?? throw new NotFoundException($"Entity: '{nameof(Account)}' with Id: '{cmd.AccountFromId}' not found.");

        var to =
             await readRepository.GetById(cmd.AccountToId, cancellationToken)
             ?? throw new NotFoundException($"Entity: '{nameof(Account)}' with Id: '{cmd.AccountToId}' not found.");

        var id = await transactService.WithdrawAsync(new TransactCommand
        {
            AccountId = cmd.AccountFromId,
            Amount = cmd.Value,
            Description = $"Transfer to Account: '{to.Id}' and Name: '{to.User}'"
        }, cancellationToken);

        id = await transactService.DepositAsync(new TransactCommand
        {
            AccountId = cmd.AccountToId,
            Amount = cmd.Value,
            Description = $"Transfer From Account: '{from.Id}' and Name: '{from.User}'"
        }, cancellationToken);
    }
}
