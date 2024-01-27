using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace DropMultipleFilesComAsyncDragImageWpf.Com
{
    internal sealed class ReadStream : IStream, IDisposable
    {
        private readonly Stream proxiedStream;

        public ReadStream(Stream stream)
        {
            this.proxiedStream = stream;
        }

        public void Read(byte[] buffer, int bufferSize, nint bytesReadPtr)
        {
            int bytesRead = proxiedStream.Read(buffer, 0, bufferSize);
            if (bytesReadPtr != nint.Zero)
            {
                Marshal.WriteInt32(bytesReadPtr, bytesRead);
            }
        }

        public void CopyTo(IStream pstm, long cb, nint pcbRead, nint pcbWritten)
        {
            var buf = new byte[8096];
            var remain = cb;
            var totalRead = 0;
            while (remain > 0)
            {
                var nRead = this.proxiedStream.Read(buf, 0, buf.Length);
                pstm.Write(buf, nRead, pcbWritten);
                totalRead += nRead;
                remain -= nRead;
                if (nRead < buf.Length)
                {
                    break;
                }
            }
            if (pcbRead != nint.Zero)
            {
                Marshal.WriteInt32(pcbRead, totalRead);
            }
        }

        public void Stat(out STATSTG streamStats, int grfStatFlag)
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
        public void Clone(out IStream ppstm)
        {
            throw new NotImplementedException();
        }

        public void Commit(int grfCommitFlags)
        {
            throw new NotImplementedException();
        }

        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotImplementedException();
        }

        public void Revert()
        {
            throw new NotImplementedException();
        }

        public void Seek(long dlibMove, int dwOrigin, nint plibNewPosition)
        {
            throw new NotImplementedException();
        }

        public void SetSize(long libNewSize)
        {
            throw new NotImplementedException();
        }

        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] pv, int cb, nint pcbWritten)
        {
            throw new NotImplementedException();
        }
        #endregion NotSupported
    }
}
