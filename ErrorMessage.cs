using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public class ErrorMessage
    {
        public int ErrorNr { get; set; }
        public String ErrorText { get; set; }
        public String []Param { get; set; }
        public ErrorMessage()
        {

        }
        public ErrorMessage(int errorNr)
        {
            ErrorNr = errorNr;
        }
        public ErrorMessage(int errorNr, string errorText) : this(errorNr)
        {
            ErrorText = errorText;
        }
        public ErrorMessage(int errorNr, string errorText, string[] param) : this(errorNr, errorText)
        {
            Param = param;
        }
        public override string ToString()
        {
            string result = ErrorText.Replace("{errid}", ErrorNr.ToString());
            if (Param != null)
                for (int i = 0; i < Param.Length; i++)
                    result = result.Replace($"{{{i}}}", Param[i]);
            return result;
        }
    }
}
