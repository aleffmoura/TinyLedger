namespace TinyLedger.WebApi.Layers.Domain.Exceptions;

public class AlreadyExistException(string msg) : BusinessException(409, msg)
{
}
