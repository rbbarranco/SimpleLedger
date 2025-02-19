using SimpleLedger.Domain.Entities;
using SimpleLedger.Domain.Exceptions;
using SimpleLedger.Domain.ValueObjects;

namespace SimpleLedger.Domain.Tests.Entities
{
    public class AccountTests
    {
        #region TryAddDeposit tests

        [Fact]
        public void TryAddDeposit_WhenAlreadyExisting_ThenThrowExceptionAndDepositNotAdded()
        {
            var referenceId = Guid.NewGuid();

            var account = Account.Create(Guid.NewGuid(),
                Deposits.Create([new DepositTransaction(referenceId, 10, DateTime.UtcNow, "ref")]),
                Withdrawals.Create([]));

            Assert.Throws<DepositTransactionAlreadyExistsException>(() =>
                account.TryAddDeposit(new DepositTransaction(referenceId, 5, DateTime.UtcNow, "ref2")));

            Assert.True(account.Deposits.Value.Count() == 1);
        }

        [Fact]
        public void TryAddDeposit_WhenSuccessful_ThenDepositAdded()
        {
            var account = Account.Create(Guid.NewGuid(),
                Deposits.Create([new DepositTransaction(Guid.NewGuid(), 10, DateTime.UtcNow, "ref")]),
                Withdrawals.Create([]));

            var deposit = new DepositTransaction(Guid.NewGuid(), 5, DateTime.UtcNow, "ref2");

            account.TryAddDeposit(deposit);

            Assert.True(account.Deposits.Value.Count() == 2);
            Assert.True(account.Deposits.Value.ToList()[1].Equals(deposit));
        }

        #endregion

        #region TryAddWithdrawal tests

        [Fact]
        public void TryAddWithdrawal_WhenAlreadyExisting_ThenThrowExceptionAndWithdrawalNotAdded()
        {
            var referenceId = Guid.NewGuid();

            var account = Account.Create(Guid.NewGuid(),
                Deposits.Create([new DepositTransaction(Guid.NewGuid(), 10, DateTime.UtcNow, "ref")]),
                Withdrawals.Create([new WithdrawalTransaction(referenceId, 5, DateTime.UtcNow, "ref2")]));

            Assert.Throws<WithdrawalTransactionAlreadyExistsException>(() =>
                account.TryAddWithdrawal(new WithdrawalTransaction(referenceId, 5, DateTime.UtcNow, "ref3")));

            Assert.True(account.Withdrawals.Value.Count() == 1);
        }

        [Fact]
        public void TryAddWithdrawal_WhenFundsInsufficient_ThenThrowExceptionAndWithdrawalNotAdded()
        {
            var account = Account.Create(Guid.NewGuid(),
                Deposits.Create([new DepositTransaction(Guid.NewGuid(), 10, DateTime.UtcNow, "ref")]),
                Withdrawals.Create([new WithdrawalTransaction(Guid.NewGuid(), 5, DateTime.UtcNow, "ref2")]));

            Assert.Throws<InsufficientFundsException>(() =>
                account.TryAddWithdrawal(new WithdrawalTransaction(Guid.NewGuid(), 6, DateTime.UtcNow, "ref3")));

            Assert.True(account.Withdrawals.Value.Count() == 1);
        }

        [Fact]
        public void TryAddWithdrawal_WhenSuccessful_ThenWithdrawalAdded()
        {
            var account = Account.Create(Guid.NewGuid(),
                Deposits.Create([new DepositTransaction(Guid.NewGuid(), 10, DateTime.UtcNow, "ref")]),
                Withdrawals.Create([new WithdrawalTransaction(Guid.NewGuid(), 5, DateTime.UtcNow, "ref2")]));

            var withdrawal = new WithdrawalTransaction(Guid.NewGuid(), 2, DateTime.UtcNow, "ref3");

            account.TryAddWithdrawal(withdrawal);

            Assert.True(account.Withdrawals.Value.Count() == 2);
            Assert.True(account.Withdrawals.Value.ToList()[1].Equals(withdrawal));
        }

        #endregion

        #region GetCurrentBalance

        [Fact]
        public void GetCurrentBalance_WhenNoTransactions_ThenReturnZero()
        {
            var account = Account.Create(Guid.NewGuid(), Deposits.Create([]), Withdrawals.Create([]));

            Assert.True(account.GetCurrentBalance() == 0);
        }

        [Fact]
        public void GetCurrentBalance_WhenThereAreTransactions_ThenReturnCorrectBalance()
        {
            var account = Account.Create(Guid.NewGuid(),
                Deposits.Create([new DepositTransaction(Guid.NewGuid(), 10, DateTime.UtcNow, "ref")]),
                Withdrawals.Create([
                    new WithdrawalTransaction(Guid.NewGuid(), 5, DateTime.UtcNow, "ref2"),
                    new WithdrawalTransaction(Guid.NewGuid(), 2, DateTime.UtcNow, "ref3")
                ]));

            Assert.True(account.GetCurrentBalance() == 3);
        }

        #endregion
    }
}