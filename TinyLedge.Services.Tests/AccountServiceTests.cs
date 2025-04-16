namespace TinyLedge.Services.Tests;
using FluentAssertions;
using Moq;
using TinyLedger.WebApi.Layers.Domain;
using TinyLedger.WebApi.Layers.Domain.Exceptions;
using TinyLedger.WebApi.Layers.Services.Accounts;
using TinyLedger.WebApi.Layers.Services.Commands;

public class AccountServiceTests
{
    private readonly Mock<IReadRepository<Account>> _accountReadRepository;
    private readonly Mock<IWriteRepository<Account>> _accountWriteRepository;

    private readonly AccountService _service;

    public AccountServiceTests()
    {
        _accountReadRepository = new();
        _accountWriteRepository = new();
        _service = new(_accountWriteRepository.Object, _accountReadRepository.Object);
    }

    [Test]
    public async Task AccountServiceTests_Create_ShouldBeOk()
    {
        // Arrange
        var newUserId = 1;
        var cancellationToken = CancellationToken.None;
        var cmd = new AccountCommand
        {
            User = "testing"
        };
        var db = new List<Account>().AsQueryable();

        _accountReadRepository.Setup(x => x.GetAll(It.IsAny<Func<Account, bool>>(), cancellationToken))
            .Returns(db);

        _accountWriteRepository.Setup(x => x.AddAsync(It.IsAny<Account>(), cancellationToken))
            .Returns(async (Account accout, CancellationToken tk) =>
            {
                accout.Id = newUserId;
                return await Task.FromResult(accout);
            });

        // Action
        var id = await _service.CreateAsync(cmd, cancellationToken);

        // Assert
        id.Should().Be(newUserId);
        _accountReadRepository.Verify(x => x.GetAll(It.IsAny<Func<Account, bool>>(), cancellationToken));
        _accountWriteRepository.Verify(x => x.AddAsync(It.IsAny<Account>(), cancellationToken));

        _accountWriteRepository.VerifyNoOtherCalls();
        _accountReadRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AccountServiceTests_Create_AlreadyExist_User_ShouldBeOk()
    {
        // Arrange
        var newUserId = 1;
        var cancellationToken = CancellationToken.None;
        var cmd = new AccountCommand
        {
            User = "testing"
        };
        var db = new List<Account>
        {
            new()
            {
                Id = newUserId,
                User = cmd.User
            }
        }.AsQueryable();

        _accountReadRepository.Setup(x => x.GetAll(It.IsAny<Func<Account, bool>>(), cancellationToken))
            .Returns(db);

        // Action
        var act = async () => await _service.CreateAsync(cmd, cancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<AlreadyExistException>()
            .WithMessage($"Entity: '{nameof(Account)}' with User: '{cmd.User}' already exists.");
        _accountReadRepository.Verify(x => x.GetAll(It.IsAny<Func<Account, bool>>(), cancellationToken));
        _accountWriteRepository.VerifyNoOtherCalls();
        _accountReadRepository.VerifyNoOtherCalls();
    }
    [Test]
    public async Task AccountServiceTests_GetById_ShouldBeOk()
    {
        // Arrange
        var newUserId = 1;
        var cancellationToken = CancellationToken.None;
        var accountId = 1;
        var account = new Account()
        {
            Id = newUserId,
            User = "user",
            CreatedAt = DateTime.UtcNow,
            Balance = 1000,
        };

        _accountReadRepository.Setup(x => x.GetById(accountId, cancellationToken))
            .ReturnsAsync(account);

        // Action
        var accountDb = await _service.GetByIdAsync(accountId, cancellationToken);

        // Assert
        accountDb.Should().NotBeNull();
        accountDb.Should().BeEquivalentTo(account);

        _accountReadRepository.Verify(x => x.GetById(accountId, cancellationToken));

        _accountWriteRepository.VerifyNoOtherCalls();
        _accountReadRepository.VerifyNoOtherCalls();
    }
    [Test]
    public async Task AccountServiceTests_GetById_AccountNull_Should_Throw_NotFoundException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var accountId = 1;

        _accountReadRepository.Setup(x => x.GetById(accountId, cancellationToken))
            .ReturnsAsync((Account?)null);

        // Action
        var act = async () => await _service.GetByIdAsync(accountId, cancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Entity: '{nameof(Account)}' with Id: '{accountId}' not found.");

        _accountReadRepository.Verify(x => x.GetById(accountId, cancellationToken));

        _accountWriteRepository.VerifyNoOtherCalls();
        _accountReadRepository.VerifyNoOtherCalls();
    }
}
