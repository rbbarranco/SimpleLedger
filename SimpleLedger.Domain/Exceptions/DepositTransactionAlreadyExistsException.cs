namespace SimpleLedger.Domain.Exceptions
{
    public class DepositTransactionAlreadyExistsException(string message) : ApplicationException(message);
}
