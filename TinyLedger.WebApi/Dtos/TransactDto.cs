namespace TinyLedger.WebApi.Dtos;

public class TransactDto
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
