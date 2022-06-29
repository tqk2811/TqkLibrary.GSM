using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Helpers.PduPaser.Decoders
{
    public interface IDecoder
    {
        string Decode(byte[] raw, int padding = 0);

    }
}
