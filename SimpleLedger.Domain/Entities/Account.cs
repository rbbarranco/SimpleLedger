using SimpleLedger.Domain.Exceptions;
using SimpleLedger.Domain.ValueObjects;

namespace SimpleLedger.Domain.Entities
{
    public class Account
    {
        public Guid AccountId { get; private set; }
        public Deposits Deposits { get; init; }
        public Withdrawals Withdrawals { get; init; }

        public static Account Create(Guid accountId, Deposits deposits, Withdrawals withdrawals)
        {
            return new Account
            {
                AccountId = accountId,
                Deposits = deposits,
                Withdrawals = withdrawals
            };
        }

        public void TryAddDeposit(DepositTransaction deposit)
        {
            //remove if idempotency checks done in account service (exercise).
            //if (Deposits.Exists(deposit.ReferenceId))
            //    throw new DepositTransactionAlreadyExistsException($"Deposit transaction with reference id {deposit.ReferenceId} already exists.");

            Deposits.Add(deposit);
        }

        public void TryAddWithdrawal(WithdrawalTransaction withdrawal)
        {
            if (Withdrawals.Exists(withdrawal.ReferenceId))
                throw new WithdrawalTransactionAlreadyExistsException($"Withdrawal transaction with reference id {withdrawal.ReferenceId} already exists.");

            if (withdrawal.Amount > GetCurrentBalance())
                throw new InsufficientFundsException($"Insufficient funds to complete withdrawal. Reference id {withdrawal.ReferenceId}.");

            Withdrawals.Add(withdrawal);
        }

        public decimal GetCurrentBalance()
        {
            return Deposits.Value.Sum(d => d.Amount) - Withdrawals.Value.Sum(w => w.Amount);
        }
    }
}
