namespace SimpleLedger.Domain.ValueObjects
{
    public record WithdrawalTransaction(Guid ReferenceId, decimal Amount, DateTime TransactionDate, string Reference)
        : ITransaction;
}
