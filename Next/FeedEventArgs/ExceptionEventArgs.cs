namespace Next
{
    using System;

    public class ExceptionEventArgs
    {
        public ExceptionEventArgs(string message, Exception exception)
        {
            this.Message = message;
            this.Exception = exception;
        }

        public string Message { get; private set; }
        public Exception Exception { get; private set; }

    }
}