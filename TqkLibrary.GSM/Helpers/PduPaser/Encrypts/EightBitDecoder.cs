using System;

namespace TqkLibrary.GSM.Helpers.PduPaser.Encrypts
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EightBitDecoder : IDecoder, IEncoder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="dataLength"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual string Decode(byte[] raw, int dataLength, int padding = 0) =>
            throw new Exception("Cannot decode User-defined coding!");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual byte[] Encode(string str)
        {
            throw new NotImplementedException();
        }
    }
}
