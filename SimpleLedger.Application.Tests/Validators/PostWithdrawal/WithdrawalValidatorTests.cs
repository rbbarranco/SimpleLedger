using AutoFixture;
using FluentValidation;
using SimpleLedger.Application.Models.Requests.PostDeposit;
using SimpleLedger.Application.Models.Requests.PostWithdrawal;
using SimpleLedger.Application.Models.Validators.PostDeposit;
using SimpleLedger.Application.Models.Validators.PostWithdrawal;

namespace SimpleLedger.Application.Tests.Validators.PostWithdrawal
{
    public class WithdrawalValidatorTests
    {
        private AbstractValidator<Withdrawal> GetSut()
        {
            return new WithdrawalValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Amount_GivenInvalid_ThenReturnInvalidResult(decimal input)
        {
            var autoFixture = new Fixture();
            var sut = GetSut();

            var request = autoFixture.Build<Withdrawal>()
                .With(x => x.Amount, input)
                .Create();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.Single().PropertyName == nameof(request.Amount));
        }
    }
}
