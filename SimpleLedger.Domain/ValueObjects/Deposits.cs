namespace SimpleLedger.Domain.ValueObjects
{
    public class Deposits
    {
        private readonly Dictionary<Guid, DepositTransaction> _deposits = new();

        public IEnumerable<DepositTransaction> Value => _deposits.Values;

        private Deposits(IEnumerable<DepositTransaction> deposits)
        {
            if (deposits is not null)
                _deposits = deposits.ToDictionary(d => d.ReferenceId);
        }

        public static Deposits Create(IEnumerable<DepositTransaction> deposits)
        {
            return new Deposits(deposits);
        }

        public bool Exists(Guid referenceId)
        {
            return _deposits.ContainsKey(referenceId);
        }

        public void Add(DepositTransaction deposit)
        {
            _deposits.TryAdd(deposit.ReferenceId, deposit);
        }
    }
}
