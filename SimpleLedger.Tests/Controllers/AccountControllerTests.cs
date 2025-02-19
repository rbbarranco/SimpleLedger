using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
using SimpleLedger.Controllers;

namespace SimpleLedger.Tests.Controllers
{
    public class AccountControllerTests
    {
        private class MockProvider
        {
            public readonly Mock<IAccountService> AccountServiceMock = new();

            public AccountController GetSut()
            {
                return new AccountController(AccountServiceMock.Object);
            }

            public void VerifyMocks()
            {
                AccountServiceMock.VerifyAll();
                AccountServiceMock.VerifyNoOtherCalls();
            }
        }

        #region PostDepositAsync tests

        [Fact]
        public async Task PostDepositAsync_WhenServiceThrowsException_ThenReturn500AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostDepositRequest>();

            mockProvider.AccountServiceMock
                .Setup(x => x.PostDepositAsync(request))
                .ThrowsAsync(new Exception())
                .Verifiable();

            var response = await sut.PostDepositAsync(request);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 500);
            Assert.Equal(result.Value, "An error occurred.");

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task PostDepositAsync_WhenServiceReturnsRequestValidationFailed_ThenReturn400AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostDepositRequest>();
            var serviceResponse = autoFixture.Build<PostDepositResponse>()
                .With(x => x.ResponseCode, PostDepositResponseCodes.RequestValidationFailed)
                .Create();

            mockProvider.AccountServiceMock
                .Setup(x => x.PostDepositAsync(request))
                .ReturnsAsync(serviceResponse)
                .Verifiable();

            var response = await sut.PostDepositAsync(request);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 400);
            Assert.Equal(result.Value, serviceResponse);

            mockProvider.VerifyMocks();
        }


        [Fact]
        public async Task PostDepositAsync_WhenServiceReturnsDepositAlreadyExisting_ThenReturn409AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostDepositRequest>();
            var serviceResponse = autoFixture.Build<PostDepositResponse>()
                .With(x => x.ResponseCode, PostDepositResponseCodes.DepositAlreadyExisting)
                .Create();

            mockProvider.AccountServiceMock
                .Setup(x => x.PostDepositAsync(request))
                .ReturnsAsync(serviceResponse)
                .Verifiable();

            var response = await sut.PostDepositAsync(request);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 409);
            Assert.Equal(result.Value, serviceResponse);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task PostDepositAsync_WhenServiceReturnsSuccess_ThenReturn200AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostDepositRequest>();
            var serviceResponse = autoFixture.Build<PostDepositResponse>()
                .With(x => x.ResponseCode, PostDepositResponseCodes.Success)
                .Create();

            mockProvider.AccountServiceMock
                .Setup(x => x.PostDepositAsync(request))
                .ReturnsAsync(serviceResponse)
                .Verifiable();

            var response = await sut.PostDepositAsync(request);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 200);
            Assert.Equal(result.Value, serviceResponse);

            mockProvider.VerifyMocks();
        }

        #endregion

        #region PostWithdrawalAsync tests

        [Fact]
        public async Task PostWithdrawalAsync_WhenServiceThrowsException_ThenReturn500AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostWithdrawalRequest>();

            mockProvider.AccountServiceMock
                .Setup(x => x.PostWithdrawalAsync(request))
                .ThrowsAsync(new Exception())
                .Verifiable();

            var response = await sut.PostWithdrawalAsync(request);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 500);
            Assert.Equal(result.Value, "An error occurred.");

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task PostWithdrawalAsync_WhenServiceReturnsRequestValidationFailed_ThenReturn400AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostWithdrawalRequest>();
            var serviceResponse = autoFixture.Build<PostWithdrawalResponse>()
                .With(x => x.ResponseCode, PostWithdrawalResponseCodes.RequestValidationFailed)
                .Create();

            mockProvider.AccountServiceMock
                .Setup(x => x.PostWithdrawalAsync(request))
                .ReturnsAsync(serviceResponse)
                .Verifiable();

            var response = await sut.PostWithdrawalAsync(request);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 400);
            Assert.Equal(result.Value, serviceResponse);

            mockProvider.VerifyMocks();
        }


        [Fact]
        public async Task PostWithdrawalAsync_WhenServiceReturnsAccountNotFound_ThenReturn404AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostWithdrawalRequest>();
            var serviceResponse = autoFixture.Build<PostWithdrawalResponse>()
                .With(x => x.ResponseCode, PostWithdrawalResponseCodes.AccountNotFound)
                .Create();

            mockProvider.AccountServiceMock
                .Setup(x => x.PostWithdrawalAsync(request))
                .ReturnsAsync(serviceResponse)
                .Verifiable();

            var response = await sut.PostWithdrawalAsync(request);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 404);
            Assert.Equal(result.Value, serviceResponse);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task PostWithdrawalAsync_WhenServiceReturnsInsufficientFunds_ThenReturn400AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostWithdrawalRequest>();
            var serviceResponse = autoFixture.Build<PostWithdrawalResponse>()
                .With(x => x.ResponseCode, PostWithdrawalResponseCodes.InsufficientFunds)
                .Create();

            mockProvider.AccountServiceMock
                .Setup(x => x.PostWithdrawalAsync(request))
                .ReturnsAsync(serviceResponse)
                .Verifiable();

            var response = await sut.PostWithdrawalAsync(request);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 400);
            Assert.Equal(result.Value, serviceResponse);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task PostWithdrawalAsync_WhenServiceReturnsWithdrawalAlreadyExisting_ThenReturn409AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostWithdrawalRequest>();
            var serviceResponse = autoFixture.Build<PostWithdrawalResponse>()
                .With(x => x.ResponseCode, PostWithdrawalResponseCodes.WithdrawalAlreadyExisting)
                .Create();

            mockProvider.AccountServiceMock
                .Setup(x => x.PostWithdrawalAsync(request))
                .ReturnsAsync(serviceResponse)
                .Verifiable();

            var response = await sut.PostWithdrawalAsync(request);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 409);
            Assert.Equal(result.Value, serviceResponse);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task PostWithdrawalAsync_WhenServiceReturnsSuccess_ThenReturn200AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var request = autoFixture.Create<PostWithdrawalRequest>();
            var serviceResponse = autoFixture.Build<PostWithdrawalResponse>()
                .With(x => x.ResponseCode, PostWithdrawalResponseCodes.Success)
                .Create();

            mockProvider.AccountServiceMock
                .Setup(x => x.PostWithdrawalAsync(request))
                .ReturnsAsync(serviceResponse)
                .Verifiable();

            var response = await sut.PostWithdrawalAsync(request);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 200);
            Assert.Equal(result.Value, serviceResponse);

            mockProvider.VerifyMocks();
        }

        #endregion

        #region GetCurrentBalanceAsync tests

        [Fact]
        public async Task GetCurrentBalanceAsync_WhenServiceThrowsException_ThenReturn500AndObject()
        {
            var mockProvider = new MockProvider();
            var sut = mockProvider.GetSut();

            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();

            var expectedGetCurrentBalanceRequest = new GetCurrentBalanceRequest(accountId, correlationId);

            mockProvider.AccountServiceMock
                .Setup(x => x.GetCurrentBalanceAsync(expectedGetCurrentBalanceRequest))
                .ThrowsAsync(new Exception())
                .Verifiable();

            var response = await sut.GetCurrentBalanceAsync(accountId, correlationId);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 500);
            Assert.Equal(result.Value, "An error occurred.");

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task GetCurrentBalanceAsync_WhenServiceReturnsRequestValidationFailed_ThenReturn400AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();

            var expectedGetCurrentBalanceRequest = new GetCurrentBalanceRequest(accountId, correlationId);
            var serviceResponse = autoFixture.Build<GetCurrentBalanceResponse>()
                .With(x => x.ResponseCode, GetCurrentBalanceResponseCodes.RequestValidationFailed)
                .Create();

            mockProvider.AccountServiceMock
                .Setup(x => x.GetCurrentBalanceAsync(expectedGetCurrentBalanceRequest))
                .ReturnsAsync(serviceResponse)
                .Verifiable();

            var response = await sut.GetCurrentBalanceAsync(accountId, correlationId);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 400);
            Assert.Equal(result.Value, serviceResponse);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task GetCurrentBalanceAsync_WhenServiceReturnsSuccess_ThenReturn200AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();

            var expectedGetCurrentBalanceRequest = new GetCurrentBalanceRequest(accountId, correlationId);
            var serviceResponse = autoFixture.Build<GetCurrentBalanceResponse>()
                .With(x => x.ResponseCode, GetCurrentBalanceResponseCodes.Success)
                .Create();

            mockProvider.AccountServiceMock
                .Setup(x => x.GetCurrentBalanceAsync(expectedGetCurrentBalanceRequest))
                .ReturnsAsync(serviceResponse)
                .Verifiable();

            var response = await sut.GetCurrentBalanceAsync(accountId, correlationId);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 200);
            Assert.Equal(result.Value, serviceResponse);

            mockProvider.VerifyMocks();
        }

        #endregion

        #region GetTransactionHistoryAsync tests

        [Fact]
        public async Task GetTransactionHistoryAsync_WhenServiceThrowsException_ThenReturn500AndObject()
        {
            var mockProvider = new MockProvider();
            var sut = mockProvider.GetSut();

            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();

            var expectedGetTransactionHistoryRequest = new GetTransactionHistoryRequest(accountId, correlationId);

            mockProvider.AccountServiceMock
                .Setup(x => x.GetTransactionHistoryAsync(expectedGetTransactionHistoryRequest))
                .ThrowsAsync(new Exception())
                .Verifiable();

            var response = await sut.GetTransactionHistoryAsync(accountId, correlationId);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 500);
            Assert.Equal(result.Value, "An error occurred.");

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task GetTransactionHistoryAsync_WhenServiceReturnsRequestValidationFailed_ThenReturn400AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();

            var expectedGetTransactionHistoryRequest = new GetTransactionHistoryRequest(accountId, correlationId);
            var serviceResponse = autoFixture.Build<GetTransactionHistoryResponse>()
                .With(x => x.ResponseCode, GetTransactionHistoryResponseCodes.RequestValidationFailed)
                .Create();

            mockProvider.AccountServiceMock
                .Setup(x => x.GetTransactionHistoryAsync(expectedGetTransactionHistoryRequest))
                .ReturnsAsync(serviceResponse)
                .Verifiable();

            var response = await sut.GetTransactionHistoryAsync(accountId, correlationId);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 400);
            Assert.Equal(result.Value, serviceResponse);

            mockProvider.VerifyMocks();
        }

        [Fact]
        public async Task GetTransactionHistoryAsync_WhenServiceReturnsSuccess_ThenReturn200AndObject()
        {
            var mockProvider = new MockProvider();
            var autoFixture = new Fixture();
            var sut = mockProvider.GetSut();

            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();

            var expectedGetTransactionHistoryRequest = new GetTransactionHistoryRequest(accountId, correlationId);
            var serviceResponse = autoFixture.Build<GetTransactionHistoryResponse>()
                .With(x => x.ResponseCode, GetTransactionHistoryResponseCodes.Success)
                .Create();

            mockProvider.AccountServiceMock
                .Setup(x => x.GetTransactionHistoryAsync(expectedGetTransactionHistoryRequest))
                .ReturnsAsync(serviceResponse)
                .Verifiable();

            var response = await sut.GetTransactionHistoryAsync(accountId, correlationId);
            var result = response.Result as ObjectResult;

            Assert.Equal(result.StatusCode, 200);
            Assert.Equal(result.Value, serviceResponse);

            mockProvider.VerifyMocks();
        }

        #endregion
    }
}