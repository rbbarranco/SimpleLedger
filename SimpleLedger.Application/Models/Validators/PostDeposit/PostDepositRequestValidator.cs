using FluentValidation;
using SimpleLedger.Application.Models.Requests.PostDeposit;

namespace SimpleLedger.Application.Models.Validators.PostDeposit
{
    public class PostDepositRequestValidator : AbstractValidator<PostDepositRequest>
    {
        public PostDepositRequestValidator(IValidator<Deposit> depositValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.CorrelationId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.Deposit).NotNull().SetValidator(depositValidator);
        }
    }
}
