using AutoFixture;
using FluentValidation;
using SimpleLedger.Application.Models.Requests.GetTransactionHistory;
using SimpleLedger.Application.Models.Validators.GetTransactionHistory;

namespace SimpleLedger.Application.Tests.Validators.GetTransactionHistory
{
    public class GetTransactionHistoryRequestValidatorTests
    {
        private AbstractValidator<GetTransactionHistoryRequest> GetSut()
        {
            return new GetTransactionHistoryRequestValidator();
        }

        [Fact]
        public void CorrelationId_GivenInvalid_ThenReturnInvalidResult()
        {
            var autoFixture = new Fixture();
            var sut = GetSut();

            var request = autoFixture.Build<GetTransactionHistoryRequest>()
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

            var request = autoFixture.Build<GetTransactionHistoryRequest>()
                .With(x => x.CorrelationId, Guid.NewGuid)
                .With(x => x.AccountId, Guid.Empty)
                .Create();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.Single().PropertyName== nameof(request.AccountId));
        }
    }
}
