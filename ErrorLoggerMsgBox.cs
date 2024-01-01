using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
