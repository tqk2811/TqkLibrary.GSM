﻿namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCOPS : CommandRequest
    {
        internal CommandRequestCOPS(IGsmClient gsmClient) : base(gsmClient, "COPS")
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<COPS_ReadResponse> ReadAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.ReadAsync(cancellationToken).ConfigureAwait(false);
            var cops = result.GetCommandResponse(Command);
            if (result.IsSuccess &&
                cops != null &&
                cops.Arguments.Count() > 0)
            {
                if (int.TryParse(cops.Arguments.FirstOrDefault(), out int mode))
                {
                    COPS_ReadResponse response = new COPS_ReadResponse();
                    response.Mode = (COPS_Mode)mode;
                    if (int.TryParse(cops.Arguments.Skip(1).FirstOrDefault(), out int format))
                        response.Format = (COPS_Format)format;
                    response.Operator = cops.Arguments.Skip(2).FirstOrDefault()?.Trim('"');
                    return response;
                }
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public class COPS_ReadResponse
        {
            /// <summary>
            /// 
            /// </summary>
            public COPS_Mode Mode { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            public COPS_Format? Format { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            public string Operator { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"Mode: {Mode}, Format: {Format}, Operator: {Operator}";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public enum COPS_Mode
        {
            /// <summary>
            /// automatic choice (the parameter &lt;oper&gt; will be ignored) (default)
            /// </summary>
            Automatic = 0,
            /// <summary>
            /// manual choice (&lt;oper&gt; field shall be present)
            /// </summary>
            Manual = 1,
            /// <summary>
            /// deregister from GSM network; the MODULE is kept unregistered until a +COPS with &lt;mode&gt;=0, 1 or 4 is issued
            /// </summary>
            Deregister = 2,
            /// <summary>
            /// set only &lt;format&gt; parameter (the parameter &lt;oper&gt; will be ignored)
            /// </summary>
            SetOnly = 3,
            /// <summary>
            /// manual/automatic (&lt;oper&gt; field shall be present); if manual selection fails, automatic mode (&lt;mode&gt;=0) is entered
            /// </summary>
            ManualOrAutomatic = 5
        }
        /// <summary>
        /// 
        /// </summary>
        public enum COPS_Format
        {
            /// <summary>
            /// alphanumeric long form (max length 16 digits)
            /// </summary>
            AlphanumericLongForm = 0,
            /// <summary>
            /// numeric 5 digits [country code (3) + network code (2)] 
            /// </summary>
            Numeric5Digits = 1,
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCOPSExtension
    {
        /// <summary>
        /// Operator Selection
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCOPS COPS(this IGsmClient gsmClient) => new CommandRequestCOPS(gsmClient);
    }
}
