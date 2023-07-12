using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
        internal ConnectDataEvent(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <summary>
        /// Must dispose after used
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
            if (isTaked) throw new InvalidOperationException($"Stream was taked");
            isTaked = true;
            onConnectStream?.Invoke();
            return new ConnectStream(_stream, UnTake);
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



        class ConnectStream : Stream
        {
            readonly Stream _stream;
            readonly Action _onDispose;
            public ConnectStream(Stream stream, Action onDispose)
            {
                this._stream = stream ?? throw new ArgumentNullException(nameof(stream));
                this._onDispose = onDispose ?? throw new ArgumentNullException(nameof(stream));
            }
            public override bool CanRead => _stream.CanRead;
            public override bool CanSeek => _stream.CanSeek;
            public override bool CanWrite => _stream.CanWrite;
            public override long Length => _stream.Length;
            public override long Position { get => _stream.Position; set => _stream.Position = value; }
            public override void Flush() => _stream.Flush();
            public override int Read(byte[] buffer, int offset, int count)
            {
                //must handle escape
                return _stream.Read(buffer, offset, count);
            }
            public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);
            public override void SetLength(long value) => _stream.SetLength(value);
            public override void Write(byte[] buffer, int offset, int count)
            {
                //must handle escape
                _stream.Write(buffer, offset, count);
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

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return _stream.ReadAsync(buffer, offset, count, cancellationToken);
            }
            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return _stream.WriteAsync(buffer, offset, count, cancellationToken);
            }
            public override void WriteByte(byte value)
            {
                _stream.WriteByte(value);
            }
            public override int ReadByte()
            {
                return _stream.ReadByte();
            }
            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                return _stream.FlushAsync(cancellationToken);
            }
        }
    }
}
