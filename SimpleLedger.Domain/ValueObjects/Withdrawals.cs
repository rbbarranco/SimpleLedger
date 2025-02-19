namespace SimpleLedger.Domain.ValueObjects
{
    public class Withdrawals
    {
        private readonly Dictionary<Guid, WithdrawalTransaction> _withdrawals = new();

        public IEnumerable<WithdrawalTransaction> Value => _withdrawals.Values;

        private Withdrawals(IEnumerable<WithdrawalTransaction> withdrawals)
        {
            if (withdrawals is not null)
                _withdrawals = withdrawals.ToDictionary(d => d.ReferenceId);
        }

        public static Withdrawals Create(IEnumerable<WithdrawalTransaction> deposits)
        {
            return new Withdrawals(deposits);
        }

        public bool Exists(Guid referenceId)
        {
            return _withdrawals.ContainsKey(referenceId);
        }

        public void Add(WithdrawalTransaction withdrawal)
        {
            _withdrawals.TryAdd(withdrawal.ReferenceId, withdrawal);
        }
    }
}
