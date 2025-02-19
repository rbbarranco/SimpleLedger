namespace SimpleLedger.Domain.ValueObjects
{
    public record DepositTransaction(Guid ReferenceId, decimal Amount, DateTime TransactionDate, string Reference)
        : ITransaction;
}
