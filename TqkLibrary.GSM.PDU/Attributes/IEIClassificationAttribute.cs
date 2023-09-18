/*
Original https://github.com/wi1dcard/sms-decoder
Edit: tqk2811
 */

using TqkLibrary.GSM.PDU.Enums;

namespace TqkLibrary.GSM.PDU.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class IEIClassificationAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classification"></param>
        public IEIClassificationAttribute(IEIClassification classification)
        {
            Classification = classification;
        }
        /// <summary>
        /// 
        /// </summary>
        public IEIClassification Classification { get; }
    }
}
