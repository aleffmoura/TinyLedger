namespace TinyLedger.WebApi.Layers.Domain.Exceptions;

public class InvalidOperationException(string msg) : BusinessException(400, msg)
{
}
