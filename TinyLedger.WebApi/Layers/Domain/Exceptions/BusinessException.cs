namespace TinyLedger.WebApi.Layers.Domain.Exceptions;

public class BusinessException(int errorCode, string? message) : Exception(message)
{
    public int ErrorCode { get; init; } = errorCode;
}
