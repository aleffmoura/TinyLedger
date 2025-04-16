namespace TinyLedger.WebApi.Layers.Domain;

public record EntityBase<T> where T : EntityBase<T>
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
}