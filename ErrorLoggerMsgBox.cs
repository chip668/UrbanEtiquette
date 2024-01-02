using System.Windows.Forms;

namespace Anzeige
{
    public class ErrorLoggerMsgBox : ErrorLogger
    {
        public void LogError(ErrorMessage error)
        {
            MessageBox.Show(error.ToString());
            base.LogError(error);
        }
    }
}
