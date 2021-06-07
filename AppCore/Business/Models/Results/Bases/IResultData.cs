namespace AppCore.Business.Models.Results.Bases
{
    public interface IResultData<out TResultType>
    {
        TResultType Data { get; }
    }
}
