using System.Runtime.InteropServices;

namespace DropMutipleFilesComAsyncWinForms.Com
{
    internal static class SafeNativeMethods
    {
        public static nint GlobalAlloc(
            [MarshalAs(UnmanagedType.U4)][In] AllocFlag uFlags,
            [In] uint dwBytes)
        {
            var handle = NativeMethods.GlobalAlloc(uFlags, dwBytes);
            if (handle == nint.Zero)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }
            return handle;
        }

        public static void GlobalFree(nint hMem)
        {
            var handle = NativeMethods.GlobalFree(hMem);
            if (handle != nint.Zero)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }
        }

        public static nint GlobalLock(nint hMem)
        {
            var handle = NativeMethods.GlobalLock(hMem);
            if (handle == nint.Zero)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }
            return handle;
        }

        public static void GlobalUnlock(nint hMem)
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
    }
}
