using System;

namespace Anzeige
{
    public class ErrorLogger : IErrorLogger
    {
        public static ErrorLogger Create()
        {
            ErrorLogger result = new ErrorLogger();
            return result;
        }
        public static ErrorLogger Create(ErrorLogger prev)
        {
            ErrorLoggerFile result = new ErrorLoggerFile();
            prev.Add(result);
            return result;
        }
        public IErrorLogger next = null;
        public virtual void Add()
        {
        }
        public void Add(IErrorLogger next)
        {
            IErrorLogger result = this.next;
            // while (result.next!=null)
            this.next = next;
        }
        public virtual void LogError(ErrorMessage error)
        {
            if (next != null)
                next.LogError(error);
        }
        public virtual void LogError(int errorNr)
        {
            LogError(errorNr, "");
        }
        public virtual void LogError(int errorNr, string errorText)
        {
            LogError(errorNr, errorText, new String[0]);
        }
        public virtual void LogError(int errorNr, string errorText, string[] param)
        {
            ErrorMessage message = new ErrorMessage(errorNr, errorText, param);
            LogError(message);
        }
        public virtual void LogError(int errorNr, string errorText, string param)
        {
            ErrorMessage message = new ErrorMessage(errorNr, errorText, param.Split(','));
        }
    }
}
