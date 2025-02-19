namespace SimpleLedger.Application.Models.Requests.PostWithdrawal
{
    public record Withdrawal(decimal Amount, DateTime TransactionDate, string Reference);
}
