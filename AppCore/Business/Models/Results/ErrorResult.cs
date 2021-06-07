namespace AppCore.Business.Models.Results
{
    public class ErrorResult: Result
    {
        public ErrorResult(string message): base(ResultStatus.Error, message)
        {

        }
        public ErrorResult(): base(ResultStatus.Error, "")
        {

        }
    }
    public class ErrorResult<TResultType>: Result<TResultType>
    {
        public ErrorResult(string message, TResultType data): base(ResultStatus.Error, message, data)
        {

        }
        public ErrorResult(string message): base(ResultStatus.Error, message, default)
        {

        }
        public ErrorResult(TResultType data): base(ResultStatus.Error, "", data)
        {

        }
        public ErrorResult(): base(ResultStatus.Error, "", default)
        {

        }
    }
}
