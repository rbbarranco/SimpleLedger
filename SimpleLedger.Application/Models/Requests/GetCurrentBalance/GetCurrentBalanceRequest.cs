namespace SimpleLedger.Application.Models.Requests.GetCurrentBalance
{
    public record GetCurrentBalanceRequest(Guid AccountId, Guid CorrelationId) : IRequest;
}
