namespace TinyLedger.WebApi.Layers.Services.Accounts;
using TinyLedger.WebApi.Layers.Domain;
using TinyLedger.WebApi.Layers.Domain.Exceptions;
using TinyLedger.WebApi.Layers.Services.Commands;
using TinyLedger.WebApi.Layers.Services.Interfaces;

public class TransactionService(
    IReadRepository<Transact> readRepository,
    IWriteRepository<Transact> writeRepository,
    IReadRepository<Account> accountReadRepository,
    IWriteRepository<Account> accountWriteRepository) : ITransactService
{
    public async Task<int> WithdrawAsync(TransactCommand cmd, CancellationToken cancellationToken)
    {
        var account =
            await accountReadRepository.GetById(cmd.AccountId, cancellationToken)
            ?? throw new NotFoundException($"Entity: '{nameof(Account)}' with Id: '{cmd.AccountId}' not found.");

        if (account.Balance < cmd.Amount)
            throw new InvalidOperationException($"'{nameof(Account)}' don't have enough money.");

        _ = await accountWriteRepository.UpdateAsync(account with
        {
            Balance = account.Balance - cmd.Amount,
        }, cancellationToken);

        var transaction = await writeRepository.AddAsync(new Transact
        {
            Amount = cmd.Amount,
            AccountId = account.Id,
            CreatedAt = DateTime.UtcNow,
            Description = cmd.Description,
        }, cancellationToken);

        return transaction.Id;
    }

    public async Task<int> DepositAsync(TransactCommand cmd, CancellationToken cancellationToken)
    {
        var account =
            await accountReadRepository.GetById(cmd.AccountId, cancellationToken)
            ?? throw new NotFoundException($"Entity: '{nameof(Account)}' with Id: '{cmd.AccountId}' not found.");

        var updatedAccount = await accountWriteRepository.UpdateAsync(account with
        {
            Balance = account.Balance + cmd.Amount
        }, cancellationToken);

        var transaction = await writeRepository.AddAsync(new Transact
        {
            AccountId = account.Id,
            Amount = cmd.Amount,
            Description = cmd.Description,
            CreatedAt = DateTime.UtcNow,
        }, cancellationToken);

        return transaction.Id;
    }

    public async Task<IQueryable<Transact>> GetByAccountAsync(int accountId, CancellationToken cancellationToken)
    {
        var account =
            await accountReadRepository.GetById(accountId, cancellationToken)
            ?? throw new NotFoundException($"Entity: '{nameof(Account)}' with Id: '{accountId}' not found.");
        var list = readRepository.GetAll(t => t.AccountId == accountId, cancellationToken).ToList();
        return readRepository.GetAll(t => t.AccountId == accountId, cancellationToken);
    }

}
