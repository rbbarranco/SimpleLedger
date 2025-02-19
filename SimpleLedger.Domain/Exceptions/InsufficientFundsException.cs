namespace SimpleLedger.Domain.Exceptions
{
    public class InsufficientFundsException(string message) : ApplicationException(message);
}
