namespace SimpleLedger.Application.Models.Requests.GetTransactionHistory
{
    public record GetTransactionHistoryRequest(Guid AccountId, Guid CorrelationId) : IRequest;
}
