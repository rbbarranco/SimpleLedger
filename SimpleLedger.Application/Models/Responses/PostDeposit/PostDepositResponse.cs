namespace SimpleLedger.Application.Models.Responses.PostDeposit
{
    public record PostDepositResponse(
        PostDepositResponseCodes ResponseCode,
        string Notes,
        Guid CorrelationId) : IResponse<PostDepositResponseCodes>;
}
