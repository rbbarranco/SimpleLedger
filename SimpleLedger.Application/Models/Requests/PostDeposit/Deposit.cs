namespace SimpleLedger.Application.Models.Requests.PostDeposit
{
    public record Deposit(decimal Amount, DateTime TransactionDate, string Reference);
}
