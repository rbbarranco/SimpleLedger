using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using SimpleLedger.Application.Models.Requests.PostDeposit;
using SimpleLedger.Application.Models.Validators.PostDeposit;

namespace SimpleLedger.Application.Tests.Validators.PostDeposit
{
    public class PostDepositRequestValidatorTests
    {
        private AbstractValidator<PostDepositRequest> GetSut()
        {
            var depositValidatorMock = new Mock<IValidator<Deposit>>();
            depositValidatorMock.Setup(x => x.Validate(It.IsAny<Deposit>()))
                .Returns(new ValidationResult());

            return new PostDepositRequestValidator(depositValidatorMock.Object);
        }

        [Fact]
        public void CorrelationId_GivenInvalid_ThenReturnInvalidResult()
        {
            var autoFixture = new Fixture();
            var sut = GetSut();

            var request = autoFixture.Build<PostDepositRequest>()
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

            var request = autoFixture.Build<PostDepositRequest>()
                .With(x => x.CorrelationId, Guid.NewGuid)
                .With(x => x.AccountId, Guid.Empty)
                .Create();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.Single().PropertyName == nameof(request.AccountId));
        }
    }
}
