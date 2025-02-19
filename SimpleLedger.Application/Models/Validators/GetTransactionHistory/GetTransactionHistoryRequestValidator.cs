using FluentValidation;
using SimpleLedger.Application.Models.Requests.GetTransactionHistory;

namespace SimpleLedger.Application.Models.Validators.GetTransactionHistory
{
    public class GetTransactionHistoryRequestValidator : AbstractValidator<GetTransactionHistoryRequest>
    {
        public GetTransactionHistoryRequestValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.CorrelationId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
        }
    }
}
