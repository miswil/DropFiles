using System.Runtime.InteropServices;

namespace DropMultipleFilesComAsyncWpf.Com
{
    internal static class SafeNativeMethods
    {
        public static IntPtr GlobalAlloc(
            [MarshalAs(UnmanagedType.U4)][In] AllocFlag uFlags,
            [In] uint dwBytes)
        {
            var handle = NativeMethods.GlobalAlloc(uFlags, dwBytes);
            if (handle == IntPtr.Zero)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }
            return handle;
        }

        public static void GlobalFree(IntPtr hMem)
        {
            var handle = NativeMethods.GlobalFree(hMem);
            if (handle != IntPtr.Zero)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }
        }

        public static IntPtr GlobalLock(IntPtr hMem)
        {
            var handle = NativeMethods.GlobalLock(hMem);
            if (handle == IntPtr.Zero)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }
            return handle;
        }

        public static void GlobalUnlock(IntPtr hMem)
        {
            var stillLocked = NativeMethods.GlobalUnlock(hMem);
            if (!stillLocked)
            {
                var lastError = Marshal.GetLastPInvokeError();
                if (lastError != NativeMethods.NO_ERROR)
                {
                    Marshal.ThrowExceptionForHR(lastError);
                }
            }
        }

        public static nint GlobalSize(IntPtr hMem)
        {
            var size = NativeMethods.GlobalSize(hMem);
            if (size == 0)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }
            return size;
        }
    }
}
