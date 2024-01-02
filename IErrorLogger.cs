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
