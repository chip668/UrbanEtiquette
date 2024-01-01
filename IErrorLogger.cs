using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public interface IErrorLogger
    {
        void LogError(ErrorMessage error);
        void LogError(int errorNr);
        void LogError(int errorNr, string errorText);
        void LogError(int errorNr, string errorText, string[] param);
    }
}
