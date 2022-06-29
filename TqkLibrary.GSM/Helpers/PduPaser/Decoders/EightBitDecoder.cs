using System;

namespace TqkLibrary.GSM.Helpers.PduPaser.Decoders
{
    public class EightBitDecoder : IDecoder
    {
        public string Decode(byte[] raw, int padding = 0) =>
            throw new Exception("Cannot decode User-defined coding!");

    }
}
