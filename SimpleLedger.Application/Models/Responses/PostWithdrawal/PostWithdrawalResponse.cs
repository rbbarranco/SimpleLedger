namespace SimpleLedger.Application.Models.Responses.PostWithdrawal
{
    public record PostWithdrawalResponse(
        PostWithdrawalResponseCodes ResponseCode,
        string Notes,
        Guid CorrelationId) : IResponse<PostWithdrawalResponseCodes>;
}
