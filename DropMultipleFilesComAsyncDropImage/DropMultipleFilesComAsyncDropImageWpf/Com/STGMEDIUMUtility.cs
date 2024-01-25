using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace DropMultipleFilesComAsyncIconWpf.Com
{
    internal static class STGMEDIUMUtility
    {
        public static int CopyTo(this ref readonly STGMEDIUM src, TYMED tymed, ref STGMEDIUM dest)
        {
            if (src.tymed != tymed)
            {
                return NativeMethods.DV_E_FORMATETC;
            }
            switch (src.tymed)
            {
                case TYMED.TYMED_HGLOBAL:
                    IntPtr srcHandle = src.unionmember;
                    IntPtr destHandle = dest.unionmember;
                    var srcSize = NativeMethods.GlobalSize(srcHandle);
                    if (destHandle == IntPtr.Zero)
                    {
                        destHandle = 
                            NativeMethods.GlobalAlloc(
                                AllocFlag.GMEM_MOVEABLE,
                                srcSize);
                    }
                    else
                    {
                        var destSize = NativeMethods.GlobalSize(destHandle);
                        if (srcSize > destSize)
                        {
                            return NativeMethods.STG_E_MEDIUMFULL;
                        }
                    }
                    try
                    {
                        var srcPtr = NativeMethods.GlobalLock(srcHandle);
                        try
                        {
                            var destPtr = NativeMethods.GlobalLock(destHandle);
                            try
                            {
                                var buf = new byte[Math.Min(srcSize, int.MaxValue)];
                                var remain = srcSize;
                                while (remain > 0)
                                {
                                    var moved = (int)Math.Min(buf.Length, remain);
                                    Marshal.Copy(srcPtr, buf, 0, moved);
                                    Marshal.Copy(buf, 0, destPtr, moved);
                                    remain -= moved;
                                }
                            }
                            finally
                            {
                                NativeMethods.GlobalUnlock(destPtr);
                            }
                        }
                        finally
                        {
                            NativeMethods.GlobalUnlock(srcPtr);
                        }
                    }
                    catch
                    {
                        if (dest.unionmember == IntPtr.Zero)
                        {
                            NativeMethods.GlobalFree(destHandle);
                        }
                        throw;
                    }
                    dest.unionmember = destHandle;
                    dest.tymed = TYMED.TYMED_HGLOBAL;
                    return NativeMethods.S_OK;
                case TYMED.TYMED_ISTREAM:
                    var srcStream = (IStream)Marshal.GetObjectForIUnknown(src.unionmember);
                    IStream? destStream = null;
                    try
                    {
                        if (dest.unionmember == IntPtr.Zero)
                        {
                            destStream = NativeMethods.SHCreateMemStream(null, 0);
                        }
                        else
                        {
                            destStream = (IStream)Marshal.GetObjectForIUnknown(dest.unionmember);
                        }
                        srcStream.Seek(0, 0, IntPtr.Zero);
                        srcStream.CopyTo(destStream, long.MaxValue, IntPtr.Zero, IntPtr.Zero);

                        dest.unionmember = Marshal.GetIUnknownForObject(destStream);
                        dest.tymed = TYMED.TYMED_ISTREAM;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(srcStream);
                        if (destStream != null)
                        {
                            Marshal.ReleaseComObject(destStream);
                        }
                    }
                    return NativeMethods.S_OK;
                default:
                    return NativeMethods.DV_E_FORMATETC;
            }
        }
    }
}
