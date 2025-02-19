using AutoFixture;
using FluentValidation;
using SimpleLedger.Application.Models.Requests.GetCurrentBalance;
using SimpleLedger.Application.Models.Validators.GetCurrentBalance;

namespace SimpleLedger.Application.Tests.Validators.GetCurrentBalance
{
    public class GetCurrentBalanceRequestValidatorTests
    {
        private AbstractValidator<GetCurrentBalanceRequest> GetSut()
        {
            return new GetCurrentBalanceRequestValidator();
        }

        [Fact]
        public void CorrelationId_GivenInvalid_ThenReturnInvalidResult()
        {
            var autoFixture = new Fixture();
            var sut = GetSut();

            var request = autoFixture.Build<GetCurrentBalanceRequest>()
                .With(x => x.CorrelationId, Guid.Empty)
                .Create();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.Single().PropertyName == nameof(request.CorrelationId));
        }

        [Fact]
        public void AccountId_GivenInvalid_ThenReturnInvalidResult()
        {
            var autoFixture = new Fixture();
            var sut = GetSut();

            var request = autoFixture.Build<GetCurrentBalanceRequest>()
                .With(x => x.CorrelationId, Guid.NewGuid)
                .With(x => x.AccountId, Guid.Empty)
                .Create();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.Single().PropertyName== nameof(request.AccountId));
        }
    }
}
