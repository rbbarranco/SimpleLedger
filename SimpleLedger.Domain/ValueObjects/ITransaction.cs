namespace SimpleLedger.Domain.ValueObjects
{
    public interface ITransaction
    {
        public Guid ReferenceId { get; }
        public decimal Amount { get; }
        public DateTime TransactionDate { get; }
        public string Reference { get; }
    }
}
