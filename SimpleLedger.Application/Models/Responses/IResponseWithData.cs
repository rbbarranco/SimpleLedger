namespace SimpleLedger.Application.Models.Responses
{
    public interface IResponseWithData<out TData, out TResponseCode> : IResponse<TResponseCode>
    {
        TData Data { get; }
    }
}
