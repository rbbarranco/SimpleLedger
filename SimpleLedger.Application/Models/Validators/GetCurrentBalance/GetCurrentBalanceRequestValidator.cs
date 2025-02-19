using FluentValidation;
using SimpleLedger.Application.Models.Requests.GetCurrentBalance;

namespace SimpleLedger.Application.Models.Validators.GetCurrentBalance
{
    public class GetCurrentBalanceRequestValidator : AbstractValidator<GetCurrentBalanceRequest>
    {
        public GetCurrentBalanceRequestValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.CorrelationId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
        }
    }
}
