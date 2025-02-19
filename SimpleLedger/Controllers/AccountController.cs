using Microsoft.AspNetCore.Mvc;
using SimpleLedger.Application.Models.Requests.GetCurrentBalance;
using SimpleLedger.Application.Models.Requests.GetTransactionHistory;
using SimpleLedger.Application.Models.Requests.PostDeposit;
using SimpleLedger.Application.Models.Requests.PostWithdrawal;
using SimpleLedger.Application.Models.Responses.GetCurrentBalance;
using SimpleLedger.Application.Models.Responses.GetTransactionHistory;
using SimpleLedger.Application.Models.Responses.PostDeposit;
using SimpleLedger.Application.Models.Responses.PostWithdrawal;
using SimpleLedger.Application.Services;

namespace SimpleLedger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
        [HttpPost]
        [Route("deposit")]
        public async Task<ActionResult<PostDepositResponse>> PostDepositAsync([FromBody] PostDepositRequest request)
        {
            var result = await HandleAsync<PostDepositResponse>(async () =>
            {
                var response = await accountService.PostDepositAsync(request);

                return response.ResponseCode switch
                {
                    PostDepositResponseCodes.RequestValidationFailed => new BadRequestObjectResult(response),
                    PostDepositResponseCodes.DepositAlreadyExisting => new ConflictObjectResult(response),
                    _ => new OkObjectResult(response)
                };
            });

            return result;
        }

        [HttpPost]
        [Route("withdrawal")]
        public async Task<ActionResult<PostWithdrawalResponse>> PostWithdrawalAsync([FromBody] PostWithdrawalRequest request)
        {
            var result = await HandleAsync<PostWithdrawalResponse>(async () =>
            {
                var response = await accountService.PostWithdrawalAsync(request);

                return response.ResponseCode switch
                {
                    PostWithdrawalResponseCodes.RequestValidationFailed => new BadRequestObjectResult(response),
                    PostWithdrawalResponseCodes.AccountNotFound => new NotFoundObjectResult(response),
                    PostWithdrawalResponseCodes.InsufficientFunds => new BadRequestObjectResult(response),
                    PostWithdrawalResponseCodes.WithdrawalAlreadyExisting => new ConflictObjectResult(response),
                    _ => new OkObjectResult(response)
                };
            });

            return result;
        }

        [HttpGet]
        [Route("{accountId}/balance")]
        public async Task<ActionResult<GetCurrentBalanceResponse>> GetCurrentBalanceAsync([FromRoute]Guid accountId, Guid correlationId)
        {
            var result = await HandleAsync<GetCurrentBalanceResponse>(async () =>
            {
                var response = await accountService.GetCurrentBalanceAsync(new GetCurrentBalanceRequest(accountId, correlationId));

                return response.ResponseCode switch
                {
                    GetCurrentBalanceResponseCodes.RequestValidationFailed => new BadRequestObjectResult(response),
                    GetCurrentBalanceResponseCodes.AccountNotFound => new NotFoundObjectResult(response),
                    _ => new OkObjectResult(response)
                };
            });

            return result;
        }

        [HttpGet]
        [Route("{accountId}/transactions")]
        public async Task<ActionResult<GetTransactionHistoryResponse>> GetTransactionHistoryAsync([FromRoute] Guid accountId, Guid correlationId)
        {
            var result = await HandleAsync<GetTransactionHistoryResponse>(async () =>
            {
                var response = await accountService.GetTransactionHistoryAsync(new GetTransactionHistoryRequest(accountId, correlationId));

                return response.ResponseCode switch
                {
                    GetTransactionHistoryResponseCodes.RequestValidationFailed => new BadRequestObjectResult(response),
                    GetTransactionHistoryResponseCodes.AccountNotFound => new NotFoundObjectResult(response),
                    _ => new OkObjectResult(response)
                };
            });

            return result;
        }

        private async Task<ActionResult<T>> HandleAsync<T>(Func<Task<ActionResult<T>>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception e)
            {
                return StatusCode(500, "An error occurred.");
            }
        }
    }
}
