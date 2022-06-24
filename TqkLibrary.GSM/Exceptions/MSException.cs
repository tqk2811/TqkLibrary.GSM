namespace TqkLibrary.GSM
{
    public class MSException : GsmException
    {
        public int Code { get; }
        string _message;
        public MSException(string message) : base(message)
        {

        }

        public MSException(int code, string message) : base(message)
        {
            this.Code = code;
            if (string.IsNullOrEmpty(message)) _message = $"error code {code}";
        }

        public override string Message
        {
            get { return string.IsNullOrEmpty(_message) ? base.Message : _message; }
        }
    }
}