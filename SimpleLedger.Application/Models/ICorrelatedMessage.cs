namespace SimpleLedger.Application.Models
{
    public interface ICorrelatedMessage
    {
        Guid CorrelationId { get; }
    }
}
