namespace TinyLedger.WebApi.Layers.Domain;

public record Transact
    : EntityBase<Transact>
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int AccountId { get; set; }
    public virtual Account? Account { get; set; }
}