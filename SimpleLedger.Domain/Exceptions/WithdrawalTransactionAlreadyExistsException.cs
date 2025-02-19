namespace SimpleLedger.Domain.Exceptions
{
    public class WithdrawalTransactionAlreadyExistsException(string message) : ApplicationException(message);
}
