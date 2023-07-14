using System;

namespace TqkLibrary.GSM
{
    /// <summary>
    /// 
    /// </summary>
    public class GsmException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public GsmException()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public GsmException(string message) : base(message)
        {

        }
    }
}