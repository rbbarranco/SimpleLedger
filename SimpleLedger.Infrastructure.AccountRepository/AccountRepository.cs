using SimpleLedger.Domain.Entities;
using SimpleLedger.Domain.Interfaces;
using System.Collections.Concurrent;

namespace SimpleLedger.Infrastructure.AccountRepository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ConcurrentDictionary<Guid, Account> _accounts = new();

        public Task<Account?> GetAccountAsync(Guid accountId)
        {
            return Task.FromResult(_accounts.GetValueOrDefault(accountId));
        }

        public Task AddOrUpdateAccountAsync(Account account)
        {
            if (!_accounts.ContainsKey(account.AccountId))
                _accounts.TryAdd(account.AccountId, account);
            else
                _accounts[account.AccountId] = account;

            return Task.CompletedTask;
        }
    }
}
