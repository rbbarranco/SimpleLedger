using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using SimpleLedger.Application.Models.Requests.PostDeposit;
using SimpleLedger.Application.Models.Requests.PostWithdrawal;
using SimpleLedger.Application.Models.Validators.PostDeposit;
using SimpleLedger.Application.Models.Validators.PostWithdrawal;

namespace SimpleLedger.Application.Tests.Validators.PostWithdrawal
{
    public class PostWithdrawalRequestValidatorTests
    {
        private AbstractValidator<PostWithdrawalRequest> GetSut()
        {
            var depositValidatorMock = new Mock<IValidator<Withdrawal>>();
            depositValidatorMock.Setup(x => x.Validate(It.IsAny<Withdrawal>()))
                .Returns(new ValidationResult());

            return new PostWithdrawalRequestValidator(depositValidatorMock.Object);
        }

        [Fact]
        public void CorrelationId_GivenInvalid_ThenReturnInvalidResult()
        {
            var autoFixture = new Fixture();
            var sut = GetSut();

            var request = autoFixture.Build<PostWithdrawalRequest>()
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

            var request = autoFixture.Build<PostWithdrawalRequest>()
                .With(x => x.CorrelationId, Guid.NewGuid)
                .With(x => x.AccountId, Guid.Empty)
                .Create();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.Single().PropertyName == nameof(request.AccountId));
        }
    }
}
