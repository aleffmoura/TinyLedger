namespace TinyLedger.WebApi.Layers.Domain.Exceptions;

public class NotFoundException(string msg) : BusinessException(404, msg)
{
}
