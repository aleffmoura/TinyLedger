namespace TinyLedge.Services.Tests;
using FluentAssertions;
using Moq;
using TinyLedger.WebApi.Layers.Domain;
using TinyLedger.WebApi.Layers.Domain.Exceptions;
using TinyLedger.WebApi.Layers.Services.Accounts;
using TinyLedger.WebApi.Layers.Services.Commands;
using TinyLedger.WebApi.Layers.Services.Interfaces;

public class TransactionServiceTests
{
    private readonly Mock<IReadRepository<Account>> _accountReadRepository;
    private readonly Mock<IWriteRepository<Account>> _accountWriteRepository;

    private readonly Mock<IReadRepository<Transact>> _readRepository;
    private readonly Mock<IWriteRepository<Transact>> _writeRepository;

    private readonly ITransactService _transactionService;

    public TransactionServiceTests()
    {
        _accountReadRepository = new();
        _accountWriteRepository = new();
        _readRepository = new();
        _writeRepository = new();
        _transactionService = new TransactionService(_readRepository.Object, _writeRepository.Object, _accountReadRepository.Object, _accountWriteRepository.Object);
    }

    [Test]
    public async Task TransactionServiceTests_Deposit_ShouldBeOk()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var accountId = 1;
        var cmd = new TransactCommand
        {
            AccountId = accountId,
            Amount = 1100,
            Description = "Test"
        };

        var account = new Account
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            Balance = 1000,
            Transactions = []
        };

        var newBalance = account.Balance + cmd.Amount;

        _accountReadRepository.Setup(x => x.GetById(accountId, cancellationToken))
            .ReturnsAsync(account);

        var transaction = new Transact
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            Description = cmd.Description,
            AccountId = account.Id,
            Amount = cmd.Amount,
        };

        _writeRepository.Setup(x => x.AddAsync(It.Is<Transact>(it => it.Amount == cmd.Amount), cancellationToken))
            .ReturnsAsync(transaction);

        _accountWriteRepository.Setup(x => x.UpdateAsync(It.Is<Account>(it => it.Balance == newBalance), cancellationToken))
            .ReturnsAsync(account with
            {
                Balance = newBalance,
                Transactions = [transaction]
            });

        // Action
        var transactionId = await _transactionService.DepositAsync(cmd, cancellationToken);

        // Asserts
        transactionId.Should().Be(transaction.Id);
        _accountReadRepository.Verify(x => x.GetById(cmd.AccountId, cancellationToken));
        _writeRepository.Verify(x => x.AddAsync(It.Is<Transact>(it => it.Amount == cmd.Amount), cancellationToken));
        _accountWriteRepository.Verify(x => x.UpdateAsync(It.Is<Account>(it => it.Balance == newBalance), cancellationToken));

        _accountReadRepository.VerifyNoOtherCalls();
        _writeRepository.VerifyNoOtherCalls();
        _accountWriteRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TransactionServiceTests_WithdrawAsync_ShouldBeOk()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var accountId = 1;

        var cmd = new TransactCommand
        {
            AccountId = accountId,
            Amount = 1000,
            Description = "Test"
        };

        var account = new Account
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            Balance = 10000,
            Transactions = []
        };

        var newBalance = account.Balance - cmd.Amount;

        _accountReadRepository.Setup(x => x.GetById(accountId, cancellationToken))
            .ReturnsAsync(account);

        var transaction = new Transact
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            Description = cmd.Description,
            AccountId = account.Id,
            Amount = cmd.Amount,
        };

        _writeRepository.Setup(x => x.AddAsync(It.Is<Transact>(it => it.Amount == cmd.Amount), cancellationToken))
            .ReturnsAsync(transaction);

        _accountWriteRepository.Setup(x => x.UpdateAsync(It.Is<Account>(it => it.Balance == newBalance), cancellationToken))
            .ReturnsAsync(account with
            {
                Balance = newBalance,
                Transactions = [transaction]
            });

        // Action
        var transactionId = await _transactionService.WithdrawAsync(cmd, cancellationToken);

        // Asserts
        transactionId.Should().Be(transaction.Id);
        _accountReadRepository.Verify(x => x.GetById(cmd.AccountId, cancellationToken));
        _writeRepository.Verify(x => x.AddAsync(It.Is<Transact>(it => it.Amount == cmd.Amount), cancellationToken));
        _accountWriteRepository.Verify(x => x.UpdateAsync(It.Is<Account>(it => it.Balance == newBalance), cancellationToken));

        _accountReadRepository.VerifyNoOtherCalls();
        _writeRepository.VerifyNoOtherCalls();
        _accountWriteRepository.VerifyNoOtherCalls();
    }
    [Test]
    public async Task TransactionServiceTests_WithdrawAsync_NotHaveMoney_ShouldBeOk()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var accountId = 1;

        var cmd = new TransactCommand
        {
            AccountId = accountId,
            Amount = 1000,
            Description = "Description"
        };

        var account = new Account
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            Balance = 500,
            Transactions = []
        };

        var newBalance = account.Balance - cmd.Amount;

        _accountReadRepository.Setup(x => x.GetById(accountId, cancellationToken))
            .ReturnsAsync(account);

        // Action
        var act = async () => await _transactionService.WithdrawAsync(cmd, cancellationToken);

        // Asserts
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"'{nameof(Account)}' don't have enough money.");

        _accountReadRepository.Verify(x => x.GetById(cmd.AccountId, cancellationToken));
        _accountReadRepository.VerifyNoOtherCalls();
        _writeRepository.VerifyNoOtherCalls();
        _accountWriteRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TransactionServiceTests_WithdrawAsync_AccountNotFound()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var accountId = 1;
        var cmd = new TransactCommand
        {
            AccountId = accountId,
            Amount = 1100,
            Description = "Test"
        };

        _accountReadRepository.Setup(x => x.GetById(accountId, cancellationToken))
            .ReturnsAsync((Account?)null);

        // Action
        var act = async () => await _transactionService.WithdrawAsync(cmd, cancellationToken);

        // Asserts
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity: '{nameof(Account)}' with Id: '{cmd.AccountId}' not found.");
        _accountReadRepository.Verify(x => x.GetById(cmd.AccountId, cancellationToken));

        _accountReadRepository.VerifyNoOtherCalls();
        _writeRepository.VerifyNoOtherCalls();
        _accountWriteRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TransactionServiceTests_Deposit_AccountNotFound()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var accountId = 1;
        var cmd = new TransactCommand
        {
            AccountId = accountId,
            Amount = 1100,
            Description = "Test"
        };

        _accountReadRepository.Setup(x => x.GetById(accountId, cancellationToken))
            .ReturnsAsync((Account?)null);

        // Action
        var act = async () => await _transactionService.DepositAsync(cmd, cancellationToken);

        // Asserts
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity: '{nameof(Account)}' with Id: '{cmd.AccountId}' not found.");
        _accountReadRepository.Verify(x => x.GetById(cmd.AccountId, cancellationToken));

        _accountReadRepository.VerifyNoOtherCalls();
        _writeRepository.VerifyNoOtherCalls();
        _accountWriteRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TransactionServiceTests_GetByAccountAsync_ShouldBeOk()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var accountId = 1;

        var account = new Account
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            Balance = 1000,
            Transactions = [new() {
                Id = 1,
                AccountId = accountId,
                CreatedAt = DateTime.UtcNow,
                Amount = 1000,
                Description = "Test"
            }]
        };
        _accountReadRepository.Setup(x => x.GetById(accountId, cancellationToken))
            .ReturnsAsync(account);

        _readRepository.Setup(x => x.GetAll(It.IsAny<Func<Transact, bool>>(), cancellationToken))
            .Returns(account.Transactions.AsQueryable());

        // Action
        var transactions = await _transactionService.GetByAccountAsync(accountId, cancellationToken);

        // Asserts
        transactions.Should().BeEquivalentTo(account.Transactions);
        _accountReadRepository.Verify(x => x.GetById(accountId, cancellationToken));
        _readRepository.Verify(x => x.GetAll(It.IsAny<Func<Transact, bool>>(), cancellationToken));
        _readRepository.VerifyNoOtherCalls();
        _writeRepository.VerifyNoOtherCalls();
        _accountReadRepository.VerifyNoOtherCalls();
        _accountWriteRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TransactionServiceTests_GetByAccountAsync_AccountNotFound_ThrowsNotFoundExn()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var accountId = 1;

        _accountReadRepository.Setup(x => x.GetById(accountId, cancellationToken))
            .ReturnsAsync((Account?)null);

        // Action
        var act = async () => await _transactionService.GetByAccountAsync(accountId, cancellationToken);

        // Asserts
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity: '{nameof(Account)}' with Id: '{accountId}' not found.");
        _accountReadRepository.Verify(x => x.GetById(accountId, cancellationToken));

        _readRepository.VerifyNoOtherCalls();
        _accountReadRepository.VerifyNoOtherCalls();
        _writeRepository.VerifyNoOtherCalls();
        _accountWriteRepository.VerifyNoOtherCalls();
    }
}
