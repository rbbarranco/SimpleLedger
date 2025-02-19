namespace SimpleLedger.Application.Models.Requests.PostWithdrawal
{
    public record PostWithdrawalRequest(Guid AccountId, Withdrawal Withdrawal, Guid CorrelationId) : IRequest;
}
