using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using SimpleLedger.Application.Models.Requests.GetCurrentBalance;
using SimpleLedger.Application.Models.Requests.GetTransactionHistory;
using SimpleLedger.Application.Models.Requests.PostDeposit;
using SimpleLedger.Application.Models.Requests.PostWithdrawal;
using SimpleLedger.Application.Models.Responses.GetCurrentBalance;
using SimpleLedger.Application.Models.Responses.GetTransactionHistory;
using SimpleLedger.Application.Models.Responses.PostDeposit;
using SimpleLedger.Application.Models.Responses.PostWithdrawal;
using SimpleLedger.Application.Services;
using SimpleLedger.Domain.Entities;
using SimpleLedger.Domain.Interfaces;
using SimpleLedger.Domain.ValueObjects;

namespace SimpleLedger.Application.Tests.Services
{
    public class AccountServiceTests
    {
        private class MockProvider
        {
            public readonly Mock<IValidator<PostDepositRequest>> PostDepositRequestValidatorMock = new();
            public readonly Mock<IValidator<PostWithdrawalRequest>> PostWithdrawalRequestValidatorMock = new();
            public readonly Mock<IValidator<GetCurrentBalanceRequest>> GetCurrentBalanceRequestValidatorMock = new();
            public readonly Mock<IValidator<GetTransactionHistoryRequest>> GetTransactionHistoryRequestValidatorMock = new();
            public readonly Mock<IAccountRepository> AccountRepositoryMock = new();

            public AccountService GetSut()
            {
                return new AccountService(PostDepositRequestValidatorMock.Object,
                    PostWithdrawalRequestValidatorMock.Object, GetCurrentBalanceRequestValidatorMock.Object,
                    GetTransactionHistoryRequestValidatorMock.Object, AccountRepositoryMock.Object);
            }

            public void VerifyMocks()
            {
                PostDepositRequestValidatorMock.VerifyAll();
                PostWithdrawalRequestValidatorMock.VerifyAll();
                GetCurrentBalanceRequestValidatorMock.VerifyAll();
                GetTransactionHistoryRequestValidatorMock.VerifyAll();
                AccountRepositoryMock.VerifyAll();
                PostDepositRequestValidatorMock.VerifyNoOtherCalls();
                PostWithdrawalRequestValidatorMock.VerifyNoOtherCalls();
                GetCurrentBalanceRequestValidatorMock.VerifyNoOtherCalls();
                GetTransactionHistoryRequestValidatorMock.VerifyNoOtherCalls();
                AccountRepositoryMock.VerifyNoOtherCalls();
            }
        }

        #region PostDepositAsync tests

        [Fact]
        public async Task PostDepositAsync_GivenValidationFailed_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostDepositRequest>();

            mockProvider.PostDepositRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult(new List<ValidationFailure>
                {
                    new("AccountId", "AccountId is required")
                }))
                .Verifiable();

            var response = await sut.PostDepositAsync(request);

            Assert.NotNull(response);
            Assert.True(response.ResponseCode == PostDepositResponseCodes.RequestValidationFailed);
            Assert.True(!string.IsNullOrWhiteSpace(response.Notes));
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task PostDepositAsync_GivenDepositAlreadyExisting_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var correlationId = Guid.NewGuid();
            var request = autoFixture.Build<PostDepositRequest>()
                .With(x => x.CorrelationId, correlationId)
                .With(x => x.Deposit, new Deposit(10, DateTime.UtcNow, "ref"))
                .Create();

            var account = Account.Create(request.AccountId,
                Deposits.Create([
                    new DepositTransaction(correlationId, 10, DateTime.UtcNow, "ref"),
                ]),
                Withdrawals.Create([]));

            mockProvider.PostDepositRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult())
                .Verifiable();

            mockProvider.AccountRepositoryMock
                .Setup(x => x.GetAccountAsync(request.AccountId))
                .ReturnsAsync(account)
                .Verifiable();

            var response = await sut.PostDepositAsync(request);

            Assert.NotNull(response);
            Assert.True(response.ResponseCode == PostDepositResponseCodes.DepositAlreadyExisting);
            Assert.True(response.Notes == "Deposit already exists.");
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task PostDepositAsync_GivenAccountNotExistingAndSuccessful_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostDepositRequest>();

            mockProvider.PostDepositRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult())
                .Verifiable();

            mockProvider.AccountRepositoryMock
                .Setup(x => x.GetAccountAsync(request.AccountId))
                .ReturnsAsync(default(Account?))
                .Verifiable();

            var response = await sut.PostDepositAsync(request);

            Assert.NotNull(response);
            Assert.True(response.ResponseCode == PostDepositResponseCodes.Success);
            Assert.True(string.IsNullOrWhiteSpace(response.Notes));
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.AccountRepositoryMock.Verify(m => m.AddOrUpdateAccountAsync(It.Is<Account>(a =>
                a.AccountId == request.AccountId &&
                a.Deposits.Value.Count() == 1)), Times.Once);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task PostDepositAsync_GivenAccountExistingAndSuccessful_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostDepositRequest>();

            var account = Account.Create(request.AccountId,
                Deposits.Create([
                    new DepositTransaction(Guid.NewGuid(), 10, DateTime.UtcNow, "ref"),
                ]),
                Withdrawals.Create([]));

            mockProvider.PostDepositRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult())
                .Verifiable();

            mockProvider.AccountRepositoryMock
                .Setup(x => x.GetAccountAsync(request.AccountId))
                .ReturnsAsync(account)
                .Verifiable();

            var response = await sut.PostDepositAsync(request);

            Assert.NotNull(response);
            Assert.True(response.ResponseCode == PostDepositResponseCodes.Success);
            Assert.True(string.IsNullOrWhiteSpace(response.Notes));
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.AccountRepositoryMock.Verify(m => m.AddOrUpdateAccountAsync(account), Times.Once);

            mockProvider.VerifyMocks();
        }

        #endregion

        #region PostWithdrawalAsync tests

        [Fact]
        public async Task PostWithdrawalAsync_GivenValidationFailed_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostWithdrawalRequest>();

            mockProvider.PostWithdrawalRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult(new List<ValidationFailure>
                {
                    new("AccountId", "AccountId is required")
                }))
                .Verifiable();

            var response = await sut.PostWithdrawalAsync(request);

            Assert.NotNull(response);
            Assert.True(response.ResponseCode == PostWithdrawalResponseCodes.RequestValidationFailed);
            Assert.True(!string.IsNullOrWhiteSpace(response.Notes));
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task PostWithdrawalAsync_GivenAccountNotFound_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostWithdrawalRequest>();

            mockProvider.PostWithdrawalRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult())
                .Verifiable();

            mockProvider.AccountRepositoryMock
                .Setup(x => x.GetAccountAsync(request.AccountId))
                .ReturnsAsync(default(Account?))
                .Verifiable();

            var response = await sut.PostWithdrawalAsync(request);

            Assert.NotNull(response);
            Assert.True(response.ResponseCode == PostWithdrawalResponseCodes.AccountNotFound);
            Assert.True(response.Notes == "Account not found.");
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task PostWithdrawalAsync_GivenWithdrawalAlreadyExisting_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var correlationId = Guid.NewGuid();
            var request = autoFixture.Build<PostWithdrawalRequest>()
                .With(x => x.CorrelationId, correlationId)
                .With(x => x.Withdrawal, new Withdrawal(5, DateTime.UtcNow, "ref"))
                .Create();

            var account = Account.Create(request.AccountId,
                Deposits.Create([
                    new DepositTransaction(Guid.NewGuid(), 10, DateTime.UtcNow, "ref"),
                ]),
                Withdrawals.Create([new WithdrawalTransaction(correlationId, 5, DateTime.UtcNow, "ref")]));

            mockProvider.PostWithdrawalRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult())
                .Verifiable();

            mockProvider.AccountRepositoryMock
                .Setup(x => x.GetAccountAsync(request.AccountId))
                .ReturnsAsync(account)
                .Verifiable();

            var response = await sut.PostWithdrawalAsync(request);

            Assert.NotNull(response);
            Assert.True(response.ResponseCode == PostWithdrawalResponseCodes.WithdrawalAlreadyExisting);
            Assert.True(response.Notes == "Withdrawal already exists.");
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task PostWithdrawalAsync_GivenInsufficientFunds_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Build<PostWithdrawalRequest>()
                .With(x => x.Withdrawal, new Withdrawal(10, DateTime.UtcNow, "ref"))
                .Create();

            var account = Account.Create(request.AccountId,
                Deposits.Create([
                    new DepositTransaction(Guid.NewGuid(), 10, DateTime.UtcNow, "ref"),
                ]),
                Withdrawals.Create([new WithdrawalTransaction(Guid.NewGuid(), 5, DateTime.UtcNow, "ref")]));

            mockProvider.PostWithdrawalRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult())
                .Verifiable();

            mockProvider.AccountRepositoryMock
                .Setup(x => x.GetAccountAsync(request.AccountId))
                .ReturnsAsync(account)
                .Verifiable();

            var response = await sut.PostWithdrawalAsync(request);

            Assert.NotNull(response);
            Assert.True(response.ResponseCode == PostWithdrawalResponseCodes.InsufficientFunds);
            Assert.True(response.Notes == "Insufficient funds.");
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task PostWithdrawalAsync_GivenSuccessful_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Build<PostWithdrawalRequest>()
                .With(x => x.Withdrawal, new Withdrawal(5, DateTime.UtcNow, "ref"))
                .Create();

            var account = Account.Create(request.AccountId,
                Deposits.Create([
                    new DepositTransaction(Guid.NewGuid(), 10, DateTime.UtcNow, "ref"),
                ]),
                Withdrawals.Create([new WithdrawalTransaction(Guid.NewGuid(), 5, DateTime.UtcNow, "ref")]));

            mockProvider.PostWithdrawalRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult())
                .Verifiable();

            mockProvider.AccountRepositoryMock
                .Setup(x => x.GetAccountAsync(request.AccountId))
                .ReturnsAsync(account)
                .Verifiable();

            var response = await sut.PostWithdrawalAsync(request);

            Assert.NotNull(response);
            Assert.True(response.ResponseCode == PostWithdrawalResponseCodes.Success);
            Assert.True(string.IsNullOrWhiteSpace(response.Notes));
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.AccountRepositoryMock.Verify(m => m.AddOrUpdateAccountAsync(account), Times.Once);

            mockProvider.VerifyMocks();
        }

        #endregion

        #region GetCurrentBalanceAsync tests

        [Fact]
        public async Task GetCurrentBalanceAsync_GivenValidationFailed_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<GetCurrentBalanceRequest>();

            mockProvider.GetCurrentBalanceRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult(new List<ValidationFailure>
                {
                    new("AccountId", "AccountId is required")
                }))
                .Verifiable();

            var response = await sut.GetCurrentBalanceAsync(request);

            Assert.NotNull(response);
            Assert.Null(response.Data);
            Assert.True(response.ResponseCode == GetCurrentBalanceResponseCodes.RequestValidationFailed);
            Assert.True(!string.IsNullOrWhiteSpace(response.Notes));
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task GetCurrentBalanceAsync_GivenAccountNotFound_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<GetCurrentBalanceRequest>();

            mockProvider.GetCurrentBalanceRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult())
                .Verifiable();

            mockProvider.AccountRepositoryMock
                .Setup(x => x.GetAccountAsync(request.AccountId))
                .ReturnsAsync(default(Account?))
                .Verifiable();

            var response = await sut.GetCurrentBalanceAsync(request);

            Assert.NotNull(response);
            Assert.Null(response.Data);
            Assert.True(response.ResponseCode == GetCurrentBalanceResponseCodes.AccountNotFound);
            Assert.True(response.Notes == "Account not found.");
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task GetCurrentBalanceAsync_GivenSuccessful_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<GetCurrentBalanceRequest>();

            var account = Account.Create(request.AccountId,
                Deposits.Create([new DepositTransaction(Guid.NewGuid(), 10, DateTime.UtcNow, "ref")]),
                Withdrawals.Create([
                    new WithdrawalTransaction(Guid.NewGuid(), 5, DateTime.UtcNow, "ref2"),
                    new WithdrawalTransaction(Guid.NewGuid(), 2, DateTime.UtcNow, "ref3")
                ]));
            var expectedBalance = 3;

            mockProvider.GetCurrentBalanceRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult())
                .Verifiable();

            mockProvider.AccountRepositoryMock
                .Setup(x => x.GetAccountAsync(request.AccountId))
                .ReturnsAsync(account)
                .Verifiable();

            var response = await sut.GetCurrentBalanceAsync(request);

            Assert.NotNull(response);
            Assert.True(response.Data == expectedBalance);
            Assert.True(response.ResponseCode == GetCurrentBalanceResponseCodes.Success);
            Assert.True(string.IsNullOrWhiteSpace(response.Notes));
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.VerifyMocks();
        }

        #endregion

        #region GetTransactionHistoryAsync tests

        [Fact]
        public async Task GetTransactionHistoryAsync_GivenValidationFailed_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<GetTransactionHistoryRequest>();

            mockProvider.GetTransactionHistoryRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult(new List<ValidationFailure>
                {
                    new("AccountId", "AccountId is required")
                }))
                .Verifiable();

            var response = await sut.GetTransactionHistoryAsync(request);

            Assert.NotNull(response);
            Assert.Null(response.Data);
            Assert.True(response.ResponseCode == GetTransactionHistoryResponseCodes.RequestValidationFailed);
            Assert.True(!string.IsNullOrWhiteSpace(response.Notes));
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task GetTransactionHistoryAsync_GivenAccountNotFound_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<GetTransactionHistoryRequest>();

            mockProvider.GetTransactionHistoryRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult())
                .Verifiable();

            mockProvider.AccountRepositoryMock
                .Setup(x => x.GetAccountAsync(request.AccountId))
                .ReturnsAsync(default(Account?))
                .Verifiable();

            var response = await sut.GetTransactionHistoryAsync(request);

            Assert.NotNull(response);
            Assert.Null(response.Data);
            Assert.True(response.ResponseCode == GetTransactionHistoryResponseCodes.AccountNotFound);
            Assert.True(response.Notes == "Account not found.");
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task GetTransactionHistoryAsync_GivenSuccessful_ThenReturnCorrectResponse()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<GetTransactionHistoryRequest>();

            var deposit1 = new DepositTransaction(Guid.NewGuid(), 10, DateTime.UtcNow.AddDays(-3), "ref");
            var withdrawal1 = new WithdrawalTransaction(Guid.NewGuid(), 5, DateTime.UtcNow.AddDays(-2), "ref2");
            var withdrawal2 = new WithdrawalTransaction(Guid.NewGuid(), 2, DateTime.UtcNow.AddDays(-1), "ref3");

            var account = Account.Create(request.AccountId,
                Deposits.Create([deposit1]),
                Withdrawals.Create([
                    withdrawal1,
                    withdrawal2
                ]));

            var expectedTransactions = new List<Transaction>([
                new Transaction(TransactionType.Withdrawal, withdrawal2.ReferenceId, withdrawal2.Amount,
                    withdrawal2.TransactionDate, withdrawal2.Reference),
                new Transaction(TransactionType.Withdrawal, withdrawal1.ReferenceId, withdrawal1.Amount,
                    withdrawal1.TransactionDate, withdrawal1.Reference),
                new Transaction(TransactionType.Deposit, deposit1.ReferenceId, deposit1.Amount,
                    deposit1.TransactionDate, deposit1.Reference)
            ]);

            mockProvider.GetTransactionHistoryRequestValidatorMock
                .Setup(x => x.Validate(request))
                .Returns(new ValidationResult())
                .Verifiable();

            mockProvider.AccountRepositoryMock
                .Setup(x => x.GetAccountAsync(request.AccountId))
                .ReturnsAsync(account)
                .Verifiable();

            var response = await sut.GetTransactionHistoryAsync(request);

            Assert.NotNull(response);
            Assert.True(response.Data.AccountId == request.AccountId);
            Assert.True(response.Data.Transactions.Count() == expectedTransactions.Count);
            for(int i =0; i<= expectedTransactions.Count-1; i++)
            {
                var expectedTransaction = expectedTransactions[i];
                Assert.True(expectedTransaction.TransactionType == expectedTransaction.TransactionType);
                Assert.True(expectedTransaction.ReferenceId == expectedTransaction.ReferenceId);
                Assert.True(expectedTransaction.Amount == expectedTransaction.Amount);
                Assert.True(expectedTransaction.TransactionDate == expectedTransaction.TransactionDate);
                Assert.True(expectedTransaction.Reference == expectedTransaction.Reference);
            };
            Assert.True(response.ResponseCode == GetTransactionHistoryResponseCodes.Success);
            Assert.True(string.IsNullOrWhiteSpace(response.Notes));
            Assert.True(response.CorrelationId == request.CorrelationId);

            mockProvider.VerifyMocks();
        }

        #endregion
    }
}