namespace SimpleLedger.Application.Models.Responses.GetTransactionHistory
{
    public record TransactionHistory(Guid AccountId, IEnumerable<Transaction> Transactions);
}
