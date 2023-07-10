using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.GSM.Helpers.PduPaser.Encrypts;

namespace TqkLibrary.GSM.Extensions
{
    //https://www.manualslib.com/manual/1511434/Quectel-Ec25.html?page=88#manual
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestQSPN : CommandRequest
    {
        internal CommandRequestQSPN(GsmClient gsmClient) : base(gsmClient, "QSPN")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<NameOfRegisteredNetwork> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.ExecuteAsync(cancellationToken);
            var qspn = result.GetCommandResponse(Command);
            if (qspn != null &&
                qspn.Arguments.Count() == 5 &&
                int.TryParse(qspn.Arguments.Skip(3).First(), out int val))
            {
                Alphabet alphabet = (Alphabet)val;
                string FNN = qspn.Arguments.First().Trim('"');
                string SNN = qspn.Arguments.Skip(1).First().Trim('"');
                string SPN = qspn.Arguments.Skip(2).First().Trim('"');
                string RPLMN = qspn.Arguments.Skip(4).First().Trim('"');
                NameOfRegisteredNetwork nameOfRegisteredNetwork = new NameOfRegisteredNetwork();
                switch (alphabet)
                {
                    case Alphabet.GSM7Bit://7bit string
                        {
                            nameOfRegisteredNetwork.FullNetworkName = SevenBitDecoder.ReplaceBasicCharacterSet(FNN);
                            nameOfRegisteredNetwork.ShortNetworkName = SevenBitDecoder.ReplaceBasicCharacterSet(SNN);
                            nameOfRegisteredNetwork.RPLMN = RPLMN;
                            nameOfRegisteredNetwork.ServiceProviderName = SPN;
                            break;
                        }

                    case Alphabet.UCS2:
                        {
                            IDecoder decoder = new UnicodeDecoder();
                            var buff_fnn = FNN.HexStringToByteArray();
                            nameOfRegisteredNetwork.FullNetworkName = decoder.Decode(buff_fnn, buff_fnn.Length);
                            var buff_snn = FNN.HexStringToByteArray();
                            nameOfRegisteredNetwork.ShortNetworkName = decoder.Decode(buff_snn, buff_snn.Length);
                            nameOfRegisteredNetwork.RPLMN = RPLMN;
                            nameOfRegisteredNetwork.ServiceProviderName = SPN;
                            break;
                        }

                    default:
                        return null;
                }
                return nameOfRegisteredNetwork;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum Alphabet
        {
            /// <summary>
            /// &lt;FNN&gt; and &lt;SNN&gt; will be shown in GSM 7 bit default alphabet string.
            /// </summary>
            GSM7Bit,
            /// <summary>
            /// &lt;FNN&gt; and &lt;SNN&gt; will be shown in UCS2 hexadecimal string.
            /// </summary>
            UCS2
        }
        /// <summary>
        /// 
        /// </summary>
        public class NameOfRegisteredNetwork
        {
            /// <summary>
            /// 
            /// </summary>
            public string ServiceProviderName { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            public string RPLMN { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            public string FullNetworkName { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            public string ShortNetworkName { get; internal set; }
        }
    }

    /// <summary>
    /// </summary>
    public static class CommandRequestQSPNExtension
    {
        /// <summary>
        /// Display the Name of Registered Network
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestQSPN QSPN(this GsmClient gsmClient) => new CommandRequestQSPN(gsmClient);
    }
}
