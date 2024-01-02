using System;
using System.Runtime.InteropServices;

namespace Anzeige
{
    class ErrorLoggerNetMsg : ErrorLogger
    {
        [DllImport("shell32.dll")]
        static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        public static ErrorLogger Create()
        {
            ErrorLoggerNetMsg result = new ErrorLoggerNetMsg();
            return result;
        }
        public static ErrorLogger Create(ErrorLogger prev)
        {
            ErrorLoggerNetMsg result = new ErrorLoggerNetMsg();
            prev.Add(result);
            return result;
        }
        public void LogError(ErrorMessage error)
        {
            String message = error.ToString();
            ShellExecute(IntPtr.Zero, "open", "msg", $"* {message}", null, 1);
            base.LogError(error);
        }
    }
}
