namespace TinyLedger.WebApi.Layers.Domain;

public record Account
    : EntityBase<Account>
{
    public string User { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public ICollection<Transact> Transactions { get; set; } = [];
}