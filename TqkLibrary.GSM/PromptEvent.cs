﻿namespace TqkLibrary.GSM
{
    /// <summary>
    /// 
    /// </summary>
    public class PromptEvent : ConnectDataEvent
    {
        internal PromptEvent(Stream stream, Action<string> logCallback) : base(stream, logCallback)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public bool Handled { get; set; } = false;

        /// <summary>
        /// To send the message issue Ctrl-Z char (0x1A hex).
        /// </summary>
        public void SendCtrlZ()
        {
            _stream.WriteByte(0x1a);
        }

        /// <summary>
        /// To exit without sending the message issue ESC char (0x1B hex).
        /// </summary>
        public void SendEsc()
        {
            _stream.WriteByte(0x1b);
        }
    }
}
