namespace TqkLibrary.GSM
{
    /// <summary>
    /// 
    /// </summary>
    public class MEException : GsmException
    {
        /// <summary>
        /// 
        /// </summary>
        public int Code { get; }
        string _message;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public MEException(string message) : base(message)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public MEException(int code, string message) : base(message)
        {
            Code = code;
            if (string.IsNullOrEmpty(message)) _message = $"error code {code}";
        }
        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get { return string.IsNullOrEmpty(_message) ? base.Message : _message; }
        }
    }
}