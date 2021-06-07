using AppCore.Business.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppCore.Business.Models.Results
{
    public abstract class Result
    {
        public ResultStatus Status { get; set; }
        public string Message { get; set; }
        public Result(ResultStatus status, string message)
        {
            Status = status;
            Message = message;
        }
    }
    public abstract class Result<TResultType> : Result, IResultData<TResultType>
    {
        public TResultType Data { get; }
        public Result(ResultStatus status, string message, TResultType data): base(status,message)
        {
            Data = data;
        }
    }
}
