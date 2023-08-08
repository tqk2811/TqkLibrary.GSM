using System.IO;
using System.Threading;

namespace TqkLibrary.GSM
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectDataEvent
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly Stream _stream;
        /// <summary>
        /// 
        /// </summary>
        protected readonly Action<string> _logCallback;
        internal ConnectDataEvent(Stream stream, Action<string> logCallback)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _logCallback = logCallback ?? throw new ArgumentNullException(nameof(logCallback));
        }

        /// <summary>
        /// Must dispose after used
        /// </summary>
        /// <returns></returns>
        public DataStream GetStream()
        {
            if (isTaked) throw new InvalidOperationException($"Stream was taked");
            isTaked = true;
            onConnectStream?.Invoke();
            return new DataStream(_stream, UnTake, _logCallback);
        }

        void UnTake()
        {
            isDisposed = true;
            onDisposeStream?.Invoke();
        }
        bool isTaked = false;
        event Action onConnectStream;

        bool isDisposed = false;
        event Action onDisposeStream;



        internal async Task<bool> WaitSomeOneTakeItAsync(int timeout)
        {
            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(timeout);
            return await WaitSomeOneTakeItAsync(cancellationTokenSource.Token);
        }
        internal async Task<bool> WaitSomeOneTakeItAsync(CancellationToken cancellationToken = default)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var register = cancellationToken.Register(() => tcs.TrySetResult(false));
            Action action = () => tcs.TrySetResult(true);
            try
            {
                onConnectStream += action;
                if (isTaked) return isTaked;
                return await tcs.Task;
            }
            finally
            {
                onConnectStream -= action;
            }
        }


        internal async Task<bool> WaitStreamDisposeAsync(int timeout)
        {
            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(timeout);
            return await WaitStreamDisposeAsync(cancellationTokenSource.Token);
        }
        internal async Task<bool> WaitStreamDisposeAsync(CancellationToken cancellationToken = default)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var register = cancellationToken.Register(() => tcs.TrySetResult(false));
            Action action = () => tcs.TrySetResult(true);
            try
            {
                onDisposeStream += action;
                if (isDisposed) return isDisposed;
                return await tcs.Task;
            }
            finally
            {
                onDisposeStream -= action;
            }
        }



#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public class DataStream : Stream
        {
            readonly Action<string> _logCallback;
            readonly Stream _stream;
            readonly Action _onDispose;
            bool isWriteLog = true;
            internal DataStream(Stream stream, Action onDispose, Action<string> logCallback)
            {
                _stream = stream ?? throw new ArgumentNullException(nameof(stream));
                _onDispose = onDispose ?? throw new ArgumentNullException(nameof(stream));
                _logCallback = logCallback ?? throw new ArgumentNullException(nameof(logCallback));
            }


            #region override
            public override bool CanRead => _stream.CanRead;
            public override bool CanSeek => _stream.CanSeek;
            public override bool CanWrite => _stream.CanWrite;
            public override long Length => _stream.Length;
            public override long Position { get => _stream.Position; set => _stream.Position = value; }
            public override void Flush() => _stream.Flush();
            public override int Read(byte[] buffer, int offset, int count)
            {
                //must handle escape
                int byte_read = _stream.Read(buffer, offset, count);
                if (isWriteLog) _logCallback?.Invoke($"DataStream_Read({byte_read})");
                return byte_read;
            }
            public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);
            public override void SetLength(long value) => _stream.SetLength(value);
            public override void Write(byte[] buffer, int offset, int count)
            {
                //must handle escape
                _stream.Write(buffer, offset, count);
                if (isWriteLog) _logCallback?.Invoke($"DataStream_Write({count})");
            }
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                _onDispose.Invoke();
            }

            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                return _stream.BeginRead(buffer, offset, count, callback, state);
            }
            public override int EndRead(IAsyncResult asyncResult)
            {
                return _stream.EndRead(asyncResult);
            }
            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                return _stream.BeginWrite(buffer, offset, count, callback, state);
            }
            public override void EndWrite(IAsyncResult asyncResult)
            {
                _stream.EndWrite(asyncResult);
            }

            public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
            {
                return _stream.CopyToAsync(destination, bufferSize, cancellationToken);
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                int byte_read = await _stream.ReadAsync(buffer, offset, count, cancellationToken);
                if (isWriteLog) _logCallback?.Invoke($"DataStream_ReadAsync({byte_read})");
                return byte_read;
            }
            public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                await _stream.WriteAsync(buffer, offset, count, cancellationToken);
                if (isWriteLog) _logCallback?.Invoke($"DataStream_WriteAsync({count})");
            }
            public override void WriteByte(byte value)
            {
                _stream.WriteByte(value);
                if (isWriteLog) _logCallback?.Invoke($"DataStream_Write(1)");
            }
            public override int ReadByte()
            {
                int val = _stream.ReadByte();
                if (isWriteLog) _logCallback?.Invoke($"DataStream_Read(1)");
                return val;
            }
            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                return _stream.FlushAsync(cancellationToken);
            }
            #endregion


            public async Task<byte[]> DownloadAsync(int size, CancellationToken cancellationToken = default)
            {
                try
                {
                    byte[] buffer = new byte[size];
                    int offset = 0;
                    isWriteLog = false;
                    while (offset < buffer.Length)
                    {
                        int byteRead = await this.ReadAsync(buffer, offset, buffer.Length - offset, cancellationToken);
                        offset += byteRead;
                        _logCallback?.Invoke($"DataStream_ReadAsync({byteRead},{offset}/{size})");
                    }
                    return buffer;
                }
                finally
                {
                    isWriteLog = true;
                }
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
