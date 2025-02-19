namespace SimpleLedger.Application.Models.Responses.GetCurrentBalance
{
    public record GetCurrentBalanceResponse(
        decimal? Data,
        GetCurrentBalanceResponseCodes ResponseCode,
        string Notes,
        Guid CorrelationId) : IResponseWithData<decimal?, GetCurrentBalanceResponseCodes>;
}
