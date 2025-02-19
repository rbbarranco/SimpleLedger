using AutoFixture;
using FluentValidation;
using SimpleLedger.Application.Models.Requests.PostDeposit;
using SimpleLedger.Application.Models.Validators.PostDeposit;

namespace SimpleLedger.Application.Tests.Validators.PostDeposit
{
    public class DepositValidatorTests
    {
        private AbstractValidator<Deposit> GetSut()
        {
            return new DepositValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Amount_GivenInvalid_ThenReturnInvalidResult(decimal input)
        {
            var autoFixture = new Fixture();
            var sut = GetSut();

            var request = autoFixture.Build<Deposit>()
                .With(x => x.Amount, input)
                .Create();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.Single().PropertyName == nameof(request.Amount));
        }
    }
}
