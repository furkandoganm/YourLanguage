namespace AppCore.Business.Models.Results
{
    public class SuccessResult: Result
    {
        public SuccessResult(string message): base(ResultStatus.Success, message)
        {

        }
        public SuccessResult(): base(ResultStatus.Success, "")
        {

        }
    }
    public class SuccessResult<TResultType>: Result<TResultType>
    {
        public SuccessResult(string message, TResultType data): base(ResultStatus.Success, message, data)
        {

        }
        public SuccessResult(string message): base(ResultStatus.Success, message, default)
        {

        }
        public SuccessResult(TResultType data): base(ResultStatus.Success, "", data)
        {

        }
        public SuccessResult(): base(ResultStatus.Success, "", default)
        {

        }
    }
}
