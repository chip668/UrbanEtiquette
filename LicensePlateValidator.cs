using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public class LicensePlateValidator
    {
        private String Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜ";
        private String Dashes = "- ";
        private String Digits = "0123456789";
        private String DigitOrHEs = "0123456789HE";
        private String[] Specialdigits =
        {
            "B",
            "BBL",
            "BWL",
            "BYL",
            "HB",
            "HEL",
            "HH",
            "LSA",
            "LSN",
            "MVL",
            "NL",
            "NRW",
            "RPL",
            "SAL",
            "SH",
            "THL"
        };
        private String Offices = "0123456789";
        private int count = 0;
        public enum State
        {
            Start,
            Letter,
            Letter2,
            DigitOrHE,
            Dash,
            Sonderkennzeichen,
            SDash,
            Digit,
            End,
            Invalid
        }
        State currentstate = State.Start;

        public State ChangeState (String c)
        {
            State result = currentstate;
            if (currentstate == State.Start)
            {
                count = 1;
                //  if (Valid(Specialdigits, c))
                //    result = State.Sonderkennzeichen;
                /// else if (Valid(Letters, c))
                if (Valid(Letters, c))
                    result = State.Letter;
                else 
                    result = State.Invalid;
            }
            else if (currentstate == State.Letter)
            {
                if (Valid(Letters, c) && count < 3)
                {
                    result = State.Letter;
                    count++;
                }
                else if (Valid(Dashes, c))
                {
                    count = 0;
                    result = State.Dash;
                }
                else
                    result = State.Invalid;
            }
            else if (currentstate == State.Dash)
            {
                if (Valid(Letters, c) && count < 3)
                {
                    result = State.Letter2;
                    count++;
                }
                else
                    result = State.Invalid;
            }
            else if (currentstate == State.Letter2)
            {
                if (Valid(Letters, c) && count < 3)
                {
                    result = State.Letter2;
                    count++;
                }
                else if (Valid(Digits, c))
                {
                    count = 1;
                    result = State.Digit;
                }
                else
                    result = State.Invalid;
            }
            else if (currentstate == State.Letter2)
            {
                if (Valid(DigitOrHEs, c) && count < 3)
                {
                    result = State.Letter2;
                    count++;
                }
                else
                    result = State.Invalid;
            }
            else if (currentstate == State.Digit)
            {
                if (Valid(DigitOrHEs, c) && count < 4)
                {
                    count++;
                }
                else
                    result = State.Invalid;
            }
            else if (currentstate == State.Sonderkennzeichen)
            {
                if (Valid(Digits, c))
                {
                    result = State.SDash;
                    count = 0;
                }
                else
                    result = State.Invalid;
            }
            else if (currentstate == State.SDash && count < 4)
            {
                if (Valid(Digits, c))
                {
                    result = State.Digit;
                    count++;
                }
                else
                    result = State.Invalid;
            }
            else
                result = State.Invalid;
            return result;
        }

        private bool Valid(string[] reference, string c)
        {
            // Durchlaufe jedes Element in der Referenz
            foreach (string element in reference)
            {
                // Überprüfe, ob das Zeichen in diesem Element enthalten ist
                if (element.Contains(c))
                {
                    // Falls gefunden, gib true zurück
                    return true;
                }
            }
            // Falls nicht gefunden, gib false zurück
            return false;
        }
        private bool Valid(string reference, string c)
        {
            // Überprüfe, ob das Zeichen in der Referenz enthalten ist
            return reference.Contains(c);
        }
    }
}
