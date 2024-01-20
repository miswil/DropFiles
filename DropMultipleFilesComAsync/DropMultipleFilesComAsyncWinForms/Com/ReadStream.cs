using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace DropMutipleFilesComAsyncWinForms.Com
{
    internal sealed class ReadStream : IStream, IDisposable
    {
        private readonly Stream proxiedStream;

        public ReadStream(Stream stream)
        {
            this.proxiedStream = stream;
        }

        void IStream.Read(byte[] buffer, int bufferSize, nint bytesReadPtr)
        {
            int bytesRead = proxiedStream.Read(buffer, 0, bufferSize);
            if (bytesReadPtr != nint.Zero)
            {
                Marshal.WriteInt32(bytesReadPtr, bytesRead);
            }
        }

        void IStream.Stat(out STATSTG streamStats, int grfStatFlag)
        {
            streamStats = new STATSTG
            {
                type = (int)STGTY.STGTY_STREAM,
                grfMode = 0
            };
            try
            {
                streamStats.cbSize = proxiedStream.Length;
            }
            catch (NotSupportedException) { }
            switch (proxiedStream.CanRead, proxiedStream.CanWrite)
            {
                case (true, true):
                    streamStats.grfMode |= (int)StgmConstants.STGM_READWRITE;
                    break;
                case (true, false):
                    streamStats.grfMode |= (int)StgmConstants.STGM_READ;
                    break;
                case (false, true):
                    streamStats.grfMode |= (int)StgmConstants.STGM_WRITE;
                    break;
                default:
                    throw new IOException();
            }
        }

        public void Dispose()
        {
            this.proxiedStream.Dispose();
            GC.SuppressFinalize(this);
        }

        #region NotSupported
        void IStream.Seek(long offset, int origin, nint newPositionPtr)
        {
            throw new NotSupportedException();
        }

        void IStream.SetSize(long libNewSize)
        {
            throw new NotSupportedException();
        }

        void IStream.Write(byte[] buffer, int bufferSize, nint bytesWrittenPtr)
        {
            throw new NotSupportedException();
        }

        void IStream.Clone(out IStream streamCopy)
        {
            streamCopy = null;
            throw new NotSupportedException();
        }

        void IStream.CopyTo(IStream targetStream, long bufferSize, nint buffer, nint bytesWrittenPtr)
        {
            throw new NotSupportedException();
        }

        void IStream.Commit(int flags)
        {
            throw new NotSupportedException();
        }

        void IStream.LockRegion(long offset, long byteCount, int lockType)
        {
            throw new NotSupportedException();
        }

        void IStream.Revert()
        {
            throw new NotSupportedException();
        }

        void IStream.UnlockRegion(long offset, long byteCount, int lockType)
        {
            throw new NotSupportedException();
        }
        #endregion NotSupported
    }
}
