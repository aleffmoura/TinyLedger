namespace TinyLedger.WebApi.Layers.Services.Commands;

public class AccountTransferCommand
{
    public int AccountFromId { get; set; }
    public int AccountToId { get; set; }
    public decimal Value { get; set; }
}
