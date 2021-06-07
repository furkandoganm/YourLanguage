using System;

namespace AppCore.Business.Models.Results
{
    public class ExceptionResult: Result
    {
        public ExceptionResult(Exception exc, bool showException = true)
            : base(ResultStatus.Exception,
                  showException?
                  (exc != null? "Exception: " + exc.Message + 
                  (exc.InnerException != null? " | Inner Exceptionİ: " + exc.InnerException.Message
                  : "")
                  : "")
                  : "Exception")
        {

        }
        public ExceptionResult(): base(ResultStatus.Exception, "Exception")
        {

        }
    }
    public class ExceptionResult<TResultType>: Result<TResultType>
    {
        public ExceptionResult(Exception exc, bool showException = true)
            : base(ResultStatus.Exception,
                  showException ?
                  (exc != null ? "Exception: " + exc.Message +
                  (exc.InnerException != null ? " | Inner Exceptionİ: " + exc.InnerException.Message
                  : "")
                  : "")
                  : "Exception", default)
        {

        }
        public ExceptionResult() : base(ResultStatus.Exception, "Exception", default)
        {

        }
    }
}
