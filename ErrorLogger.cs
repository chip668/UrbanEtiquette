using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public void LogError(ErrorMessage error)
        {
            if (next != null)
                next.LogError(error);
        }
        public void LogError(int errorNr)
        {
            LogError(errorNr, "");
        }
        public void LogError(int errorNr, string errorText)
        {
            LogError(errorNr, errorText, null);
        }
        public void LogError(int errorNr, string errorText, string[] param)
        {
            ErrorMessage message = new ErrorMessage(errorNr, errorText, param);
            LogError(message);
        }
    }
}
