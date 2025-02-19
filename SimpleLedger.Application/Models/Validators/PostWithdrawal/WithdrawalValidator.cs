using FluentValidation;
using SimpleLedger.Application.Models.Requests.PostWithdrawal;

namespace SimpleLedger.Application.Models.Validators.PostWithdrawal
{
    public class WithdrawalValidator : AbstractValidator<Withdrawal>
    {
        public WithdrawalValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
