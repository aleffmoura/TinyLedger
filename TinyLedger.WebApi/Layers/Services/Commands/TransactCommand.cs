namespace TinyLedger.WebApi.Layers.Services.Commands;

public class TransactCommand
{
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
