namespace SimpleLedger.Application.Models.Responses.PostWithdrawal
{
    public enum PostWithdrawalResponseCodes
    {
        Success,
        RequestValidationFailed,
        AccountNotFound,
        WithdrawalAlreadyExisting,
        InsufficientFunds,
    }
}
