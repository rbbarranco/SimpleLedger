using FluentValidation;
using SimpleLedger.Application.Models.Requests.PostDeposit;

namespace SimpleLedger.Application.Models.Validators.PostDeposit
{
    public class DepositValidator : AbstractValidator<Deposit>
    {
        public DepositValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
