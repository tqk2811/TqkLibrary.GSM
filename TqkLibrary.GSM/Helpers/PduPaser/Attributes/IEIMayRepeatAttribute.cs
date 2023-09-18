/*
Original https://github.com/wi1dcard/sms-decoder
Edit: tqk2811
 */

namespace TqkLibrary.GSM.Helpers.PduPaser.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class IEIMayRepeatAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public IEIMayRepeatAttribute(bool mayRepeat)
        {
            MayRepeat = mayRepeat;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool MayRepeat { get; }
    }
}
