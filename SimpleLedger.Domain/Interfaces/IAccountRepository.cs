using SimpleLedger.Domain.Entities;

namespace SimpleLedger.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountAsync(Guid accountId);
        Task AddOrUpdateAccountAsync(Account account);
    }
}
