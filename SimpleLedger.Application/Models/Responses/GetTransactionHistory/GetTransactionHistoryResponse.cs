namespace SimpleLedger.Application.Models.Responses.GetTransactionHistory
{
    public record GetTransactionHistoryResponse(
        TransactionHistory Data,
        GetTransactionHistoryResponseCodes ResponseCode,
        string Notes,
        Guid CorrelationId) : IResponseWithData<TransactionHistory, GetTransactionHistoryResponseCodes>;
}
