/*
Original https://github.com/wi1dcard/sms-decoder
Edit: tqk2811
 */

namespace TqkLibrary.GSM.Helpers.PduPaser.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class IEILengthAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public IEILengthAttribute(int length)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        public IEILengthAttribute(int min, int max)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        public int? Length { get; }
        /// <summary>
        /// 
        /// </summary>
        public int? Min { get; }
        /// <summary>
        /// 
        /// </summary>
        public int? Max { get; }
    }
}
