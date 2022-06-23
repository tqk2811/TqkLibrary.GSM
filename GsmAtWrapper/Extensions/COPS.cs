﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsmAtWrapper.Extensions
{
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

    public static partial class GsmExtensions
    {
        public static async Task<COPS_ReadResponse> ReadOperatorSelection(this GsmClient gsmClient)
        {
            var result = await gsmClient.Read("COPS").ConfigureAwait(false);
            if (result.IsSuccess && 
                result.CommandResponses.ContainsKey("COPS") && 
                result.CommandResponses["COPS"].Count() > 0)
            {
                if(int.TryParse(result.CommandResponses["COPS"].FirstOrDefault(),out int mode))
                {
                    COPS_ReadResponse response = new COPS_ReadResponse();
                    response.Mode = (COPS_Mode)mode;
                    if(int.TryParse(result.CommandResponses["COPS"].Skip(1).FirstOrDefault(),out int format)) 
                        response.Format = (COPS_Format)format;
                    response.Operator = result.CommandResponses["COPS"].Skip(2).FirstOrDefault()?.Trim('"');
                    return response;
                }
            }
            return null;
        }
    }

    public class COPS_ReadResponse
    {
        public COPS_Mode Mode { get; internal set; }
        public COPS_Format? Format { get; internal set; }
        public string Operator { get; internal set; }

        public override string ToString()
        {
            return $"Mode: {Mode}, Format: {Format}, Operator: {Operator}";
        }
    }
}
