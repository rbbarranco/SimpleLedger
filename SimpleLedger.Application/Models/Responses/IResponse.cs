namespace SimpleLedger.Application.Models.Responses
{
    public interface IResponse<out TResponseCode> : ICorrelatedMessage
    {
        TResponseCode ResponseCode { get; }
        string Notes { get; }
    }
}
