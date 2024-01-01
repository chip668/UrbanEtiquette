using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public class ErrorLoggerFile : ErrorLogger
    {
        public static ErrorLogger Create(String logfilename = "debug.log")
        {
            ErrorLoggerFile result = new ErrorLoggerFile();
            result.logfile = logfilename;
            return result;
        }
        public static ErrorLogger Create(ErrorLogger prev, String logfilename = "debug.log")
        {
            ErrorLoggerFile result = new ErrorLoggerFile();
            result.logfile = logfilename;
            prev.Add(result);
            return result;
        }
        public String logfile { get; set; } = "debug.log";
        public void LogError(ErrorMessage error)
        {
            File.AppendAllText(logfile, error.ToString());
            base.LogError(error);
        }
    }
}
