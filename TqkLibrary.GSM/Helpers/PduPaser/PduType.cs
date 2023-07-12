namespace TqkLibrary.GSM.Helpers.PduPaser
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public enum PduType : byte
    {
        SmsDeliver = 0b00000000,
        SmsSubmitReport = 0b00000001,
        SmsStatusReport = 0b00000010,
        SmsSubmit = 0b00000011,
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
