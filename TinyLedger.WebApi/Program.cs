using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyLedger.WebApi.Dtos;
using TinyLedger.WebApi.Layers.Domain;
using TinyLedger.WebApi.Layers.Domain.Exceptions;
using TinyLedger.WebApi.Layers.Infra;
using TinyLedger.WebApi.Layers.Services.Accounts;
using TinyLedger.WebApi.Layers.Services.Commands;
using TinyLedger.WebApi.Layers.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LedgerContext>(opt => opt.UseInMemoryDatabase("LedgerContext"));

builder.Services.AddScoped<IWriteRepository<Transact>, WriteRepository<Transact>>();
builder.Services.AddScoped<IWriteRepository<Account>, WriteRepository<Account>>();

builder.Services.AddScoped<IReadRepository<Transact>, ReadRepository<Transact>>();
builder.Services.AddScoped<IReadRepository<Account>, ReadRepository<Account>>();

builder.Services.AddScoped<ITransactService, TransactionService>();
builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

static async Task<IResult> GetResult(Func<Task<IResult>> action)
{
    try
    {
        return await action();
    }
    catch (BusinessException ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: ex.ErrorCode, title: "BusinessError");
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500, title: "Exception");
    }
}

var accounts = app.MapGroup("Accounts").WithOpenApi();
accounts.MapPost("/", async (
    [FromServices] IAccountService service,
    [FromBody] AccountCommand cmd,
    CancellationToken cancellationToken)
        => await GetResult(async () =>
        {
            var result = await service.CreateAsync(cmd, cancellationToken);
            return Results.Created($"/accounts/{result}", result);
        }));

accounts.MapPost("/transfer", async (
    [FromServices] IAccountService service,
    [FromBody] AccountTransferCommand cmd,
    CancellationToken cancellationToken)
        => await GetResult(async () =>
        {
            await service.Transfer(cmd, cancellationToken);
            return Results.Ok();
        }));

accounts.MapGet("/{accountId}", async (
    [FromServices] IAccountService service,
    [FromRoute] int accountId,
    CancellationToken cancellationToken)
        => await GetResult(async () =>
        {
            var account = await service.GetByIdAsync(accountId, cancellationToken);
            return Results.Ok(new
            {
                account!.Balance
            });
        }));

accounts.MapPost("/{accountId}/transactions", async (
    [FromRoute] int accountId,
    [FromBody] TransactDto dto,
    [FromServices] ITransactService transactService,
    CancellationToken cancellationToken) =>
    await GetResult(async () =>
    {
        var result = await transactService.DepositAsync(new TransactCommand
        {
            AccountId = accountId,
            Amount = dto.Amount,
            Description = dto.Description
        }, cancellationToken);
        return Results.Created($"/{accountId}/transactions/{result}", result);
    }));

accounts.MapPatch("/{accountId}/transactions", async (
    [FromRoute] int accountId,
    [FromBody] TransactDto dto,
    [FromServices] ITransactService transactService,
    CancellationToken cancellationToken) =>
{
    return await GetResult(async () =>
    {
        var result = await transactService.WithdrawAsync(new TransactCommand
        {
            AccountId = accountId,
            Amount = dto.Amount,
            Description = dto.Description
        }, cancellationToken);

        return Results.Created($"/{accountId}/transactions/{result}", result);
    });
});

accounts.MapGet("/{accountId}/transactions", async (
    [FromServices] ITransactService transactService,
    [FromRoute] int accountId,
    CancellationToken cancellationToken)
        => await GetResult(async () => Results.Ok(await transactService.GetByAccountAsync(accountId, cancellationToken))));

app.Run();
