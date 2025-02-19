using FluentValidation;
using SimpleLedger.Application.Models.Requests.GetCurrentBalance;
using SimpleLedger.Application.Models.Requests.GetTransactionHistory;
using SimpleLedger.Application.Models.Requests.PostDeposit;
using SimpleLedger.Application.Models.Requests.PostWithdrawal;
using SimpleLedger.Application.Models.Responses.GetCurrentBalance;
using SimpleLedger.Application.Models.Responses.GetTransactionHistory;
using SimpleLedger.Application.Models.Responses.PostDeposit;
using SimpleLedger.Application.Models.Responses.PostWithdrawal;
using SimpleLedger.Domain.Entities;
using SimpleLedger.Domain.Exceptions;
using SimpleLedger.Domain.Interfaces;
using SimpleLedger.Domain.ValueObjects;

namespace SimpleLedger.Application.Services
{
    public class AccountService(
        IValidator<PostDepositRequest> postDepositRequestValidator,
        IValidator<PostWithdrawalRequest> postWithdrawalRequestValidator,
        IValidator<GetCurrentBalanceRequest> getCurrentBalanceRequestValidator,
        IValidator<GetTransactionHistoryRequest> getTransactionHistoryRequestValidator,
        IAccountRepository accountRepository) : IAccountService
    {
        public async Task<PostDepositResponse> PostDepositAsync(PostDepositRequest request)
        {
            //Validate
            var validationResult = postDepositRequestValidator.Validate(request);
            if (!validationResult.IsValid)
                return new PostDepositResponse(PostDepositResponseCodes.RequestValidationFailed, validationResult.Errors.FirstOrDefault()?.ErrorMessage, request.CorrelationId);

            //Get or create account
            var account = await GetOrCreateAccountAsync(request.AccountId);

            //Try post deposit
            try
            {
                account.TryAddDeposit(new DepositTransaction(request.CorrelationId, request.Deposit.Amount, request.Deposit.TransactionDate, request.Deposit.Reference));
            }
            catch (DepositTransactionAlreadyExistsException e)
            {
                return new PostDepositResponse(PostDepositResponseCodes.DepositAlreadyExisting, "Deposit already exists.", request.CorrelationId);
            }

            //Save account
            await accountRepository.AddOrUpdateAccountAsync(account);

            //Return response
            return new PostDepositResponse(PostDepositResponseCodes.Success, string.Empty, request.CorrelationId);
        }

        public async Task<PostWithdrawalResponse> PostWithdrawalAsync(PostWithdrawalRequest request)
        {
            //Validate
            var validationResult = postWithdrawalRequestValidator.Validate(request);
            if (!validationResult.IsValid)
                return new PostWithdrawalResponse(PostWithdrawalResponseCodes.RequestValidationFailed,
                    validationResult.Errors.FirstOrDefault()?.ErrorMessage, request.CorrelationId);

            //Get account
            var account = await accountRepository.GetAccountAsync(request.AccountId);
            if (account is null)
                return new PostWithdrawalResponse(PostWithdrawalResponseCodes.AccountNotFound, "Account not found.", request.CorrelationId);

            //Try post withdrawal
            try
            {
                account.TryAddWithdrawal(new WithdrawalTransaction(request.CorrelationId, request.Withdrawal.Amount, request.Withdrawal.TransactionDate, request.Withdrawal.Reference));
            }
            catch (WithdrawalTransactionAlreadyExistsException e)
            {
                return new PostWithdrawalResponse(PostWithdrawalResponseCodes.WithdrawalAlreadyExisting, "Withdrawal already exists.", request.CorrelationId);
            }
            catch (InsufficientFundsException e)
            {
                return new PostWithdrawalResponse(PostWithdrawalResponseCodes.InsufficientFunds, "Insufficient funds.", request.CorrelationId);
            }

            //Save account
            await accountRepository.AddOrUpdateAccountAsync(account);

            //Return response
            return new PostWithdrawalResponse(PostWithdrawalResponseCodes.Success, string.Empty, request.CorrelationId);
        }

        public async Task<GetCurrentBalanceResponse> GetCurrentBalanceAsync(GetCurrentBalanceRequest request)
        {
            //Validate
            var validationResult = getCurrentBalanceRequestValidator.Validate(request);
            if (!validationResult.IsValid)
                return new GetCurrentBalanceResponse(default, GetCurrentBalanceResponseCodes.RequestValidationFailed,
                    validationResult.Errors.FirstOrDefault()?.ErrorMessage, request.CorrelationId);

            //Get account
            var account = await accountRepository.GetAccountAsync(request.AccountId);
            if (account is null)
                return new GetCurrentBalanceResponse(default, GetCurrentBalanceResponseCodes.AccountNotFound, "Account not found.", request.CorrelationId);

            //Return response
            return new GetCurrentBalanceResponse(account.GetCurrentBalance(), GetCurrentBalanceResponseCodes.Success, string.Empty,
                request.CorrelationId);
        }

        public async Task<GetTransactionHistoryResponse> GetTransactionHistoryAsync(GetTransactionHistoryRequest request)
        {
            //Validate
            var validationResult = getTransactionHistoryRequestValidator.Validate(request);
            if (!validationResult.IsValid)
                return new GetTransactionHistoryResponse(default,
                    GetTransactionHistoryResponseCodes.RequestValidationFailed,
                    validationResult.Errors.FirstOrDefault()?.ErrorMessage, request.CorrelationId);

            //Get account
            var account = await accountRepository.GetAccountAsync(request.AccountId);
            if (account is null)
                return new GetTransactionHistoryResponse(default, GetTransactionHistoryResponseCodes.AccountNotFound, "Account not found.", request.CorrelationId);

            //Get transactions
            var transactions = new List<Transaction>();
            transactions.AddRange(account.Deposits.Value.Select(d => new Transaction(TransactionType.Deposit, d.ReferenceId, d.Amount, d.TransactionDate, d.Reference)));
            transactions.AddRange(account.Withdrawals.Value.Select(w => new Transaction(TransactionType.Withdrawal, w.ReferenceId, w.Amount, w.TransactionDate, w.Reference)));

            //Return response
            var transactionHistory =
                new TransactionHistory(request.AccountId, transactions.OrderByDescending(t => t.TransactionDate));
            return new GetTransactionHistoryResponse(transactionHistory, GetTransactionHistoryResponseCodes.Success, string.Empty, request.CorrelationId);
        }

        private async Task<Account> GetOrCreateAccountAsync(Guid accountId)
        {
            var maybeAccount = await accountRepository.GetAccountAsync(accountId);
            return maybeAccount ?? Account.Create(accountId, Deposits.Create([]), Withdrawals.Create([]));
        }
    }
}
