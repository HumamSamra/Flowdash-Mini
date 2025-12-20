namespace Flowdash_Mini.Models
{
    public class SmtpResult
    {
        public string Message { get; set; } = string.Empty;
        public bool Succeeded { get; set; } = true;

        public SmtpResult(string msg, bool succeeded = true)
        {
            Succeeded = succeeded;
            Message = msg;
        }
    }
}
