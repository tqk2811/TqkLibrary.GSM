namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestQFLST : CommandRequest
    {
        internal CommandRequestQFLST(IGsmClient gsmClient) : base(gsmClient, "QFLST")
        {

        }

        /// <summary>
        /// </summary>
        /// <param name="namePattern">
        /// “*” All the files in UFS<br></br>
        /// “RAM:*” All the files in RAM<br></br>
        /// “SD:*” All the files in SD<br></br>
        /// &#8220;&lt;filename&gt;&#8221; The specified file &lt;filename&gt; in UFS<br></br>
        /// &#8220;RAM:&lt;filename&gt;&#8221; The specified file &lt;filename&gt; in RAM<br></br>
        /// &#8220;SD:&lt;filename&gt;&#8221; The specified file &lt;filename&gt; in SD
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<IReadOnlyList<FileInfo>> WriteAsync(string namePattern, CancellationToken cancellationToken = default)
        {
            var result = await base.WriteAsync(cancellationToken, namePattern.ToAtString());
            var qflst = result.GetCommandResponses(Command);
            List<FileInfo> list = new List<FileInfo>();
            if (result.IsSuccess)
            {
                foreach (var item in qflst)
                {
                    if (item.Arguments.Count() >= 2)
                    {
                        string path = item.Arguments.First();
                        string size = item.Arguments.Skip(1).First();

                        if (long.TryParse(size, out long s))
                        {
                            list.Add(new FileInfo()
                            {
                                Path = path.Trim('"'),
                                FileSize = s,
                            });
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<GsmCommandResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => base.ExecuteAsync(cancellationToken);


        /// <summary>
        /// 
        /// </summary>
        public class FileInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public string Path { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            public long FileSize { get; internal set; }
        }
    }

    /// <summary>
    /// </summary>
    public static class CommandRequestQFLSTExtension
    {
        /// <summary>
        /// List Files
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestQFLST QFLST(this IGsmClient gsmClient) => new CommandRequestQFLST(gsmClient);
    }
}
