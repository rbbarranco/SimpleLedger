namespace SimpleLedger.Application.Models.Responses.GetTransactionHistory
{
    public record Transaction(
        TransactionType TransactionType,
        Guid ReferenceId,
        decimal Amount,
        DateTime TransactionDate,
        string Reference);
}
