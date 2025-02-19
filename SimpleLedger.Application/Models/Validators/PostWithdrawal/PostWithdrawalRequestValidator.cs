using FluentValidation;
using SimpleLedger.Application.Models.Requests.PostWithdrawal;

namespace SimpleLedger.Application.Models.Validators.PostWithdrawal
{
    public class PostWithdrawalRequestValidator : AbstractValidator<PostWithdrawalRequest>
    {
        public PostWithdrawalRequestValidator(IValidator<Withdrawal> withdrawalValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.CorrelationId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.Withdrawal).NotNull().SetValidator(withdrawalValidator);
        }
    }
}
