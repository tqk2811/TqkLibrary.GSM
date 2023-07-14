namespace TqkLibrary.GSM.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGsmClient : IBaseGsmClient
    {
        /// <summary>
        /// 
        /// </summary>
        int CommandTimeout { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GsmCommandResult> SendCommandAsync(string command, CancellationToken cancellationToken = default);
    }
}
