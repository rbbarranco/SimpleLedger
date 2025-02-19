using SimpleLedger.Application.Models.Requests.GetCurrentBalance;
using SimpleLedger.Application.Models.Requests.GetTransactionHistory;
using SimpleLedger.Application.Models.Requests.PostDeposit;
using SimpleLedger.Application.Models.Requests.PostWithdrawal;
using SimpleLedger.Application.Models.Responses.GetCurrentBalance;
using SimpleLedger.Application.Models.Responses.GetTransactionHistory;
using SimpleLedger.Application.Models.Responses.PostDeposit;
using SimpleLedger.Application.Models.Responses.PostWithdrawal;

namespace SimpleLedger.Application.Services
{
    public interface IAccountService
    {
        Task<PostDepositResponse> PostDepositAsync(PostDepositRequest request);
        Task<PostWithdrawalResponse> PostWithdrawalAsync(PostWithdrawalRequest request);
        Task<GetCurrentBalanceResponse> GetCurrentBalanceAsync(GetCurrentBalanceRequest request);
        Task<GetTransactionHistoryResponse> GetTransactionHistoryAsync(GetTransactionHistoryRequest request);
    }
}
