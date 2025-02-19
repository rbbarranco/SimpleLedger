namespace SimpleLedger.Application.Models.Requests.PostDeposit
{
    public record PostDepositRequest(
        Guid AccountId,
        Deposit Deposit,
        Guid CorrelationId) : IRequest;
}
