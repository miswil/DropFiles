using System.Runtime.InteropServices;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace DropMutipleFilesComAsyncWinForms.Com
{
    static class NativeMethods
    {
        [DllImport("kernel32.dll", EntryPoint = "GlobalAlloc", SetLastError = true)]
        private static extern IntPtr UnsafeGlobalAlloc(
            [MarshalAs(UnmanagedType.U4)][In] AllocFlag uFlags,
            [In] nint dwBytes);

        public static IntPtr GlobalAlloc(
            [MarshalAs(UnmanagedType.U4)][In] AllocFlag uFlags,
            [In] nint dwBytes)
        {
            var handle = NativeMethods.UnsafeGlobalAlloc(uFlags, dwBytes);
            if (handle == IntPtr.Zero)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }
            return handle;
        }

        [DllImport("kernel32.dll", EntryPoint = "GlobalFree", SetLastError = true)]
        private static extern IntPtr UnsafeGlobalFree(IntPtr hMem);

        public static void GlobalFree(IntPtr hMem)
        {
            var handle = NativeMethods.UnsafeGlobalFree(hMem);
            if (handle != IntPtr.Zero)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "GlobalLock", SetLastError = true)]
        private static extern IntPtr UnsafeGlobalLock(IntPtr hMem);
        public static IntPtr GlobalLock(IntPtr hMem)
        {
            var handle = NativeMethods.UnsafeGlobalLock(hMem);
            if (handle == IntPtr.Zero)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }
            return handle;
        }

        [DllImport("kernel32.dll", EntryPoint = "GlobalUnlock", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnsafeGlobalUnlock(IntPtr hMem);
        public static void GlobalUnlock(IntPtr hMem)
        {
            var stillLocked = NativeMethods.UnsafeGlobalUnlock(hMem);
            if (!stillLocked)
            {
                var lastError = Marshal.GetLastPInvokeError();
                if (lastError != NativeMethods.NO_ERROR)
                {
                    Marshal.ThrowExceptionForHR(lastError);
                }
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "GlobalSize", SetLastError = true)]
        public static extern nint UnsafeGlobalSize(IntPtr hMem);
        public static nint GlobalSize(IntPtr hMem)
        {
            var size = NativeMethods.UnsafeGlobalSize(hMem);
            if (size == 0)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }
            return size;
        }

        [DllImport("ole32.dll")]
        public static extern int DoDragDrop(
            IComDataObject pDataObject,
            IDropSource pDropSource,
            [MarshalAs(UnmanagedType.I4)] DragDropEffects dwOKEffect,
            [Out][MarshalAs(UnmanagedType.I4)] out DragDropEffects pdwEffect);

        public const int NO_ERROR = 0;
        /// <summary>
        /// Operation successful
        /// </summary>
        public const int S_OK = 0x00000000;
        /// <summary>
        /// Operation successful but returned no results
        /// </summary>
        public const int S_FALSE = 0x00000001;
        /// <summary>
        /// Unspecified failure
        /// </summary>
        public const int E_FAIL = unchecked((int)0x80004005);
        /// <summary>
        /// Not implemented
        /// </summary>
        public const int E_NOTIMPL = unchecked((int)0x80004001);
        /// <summary>
        /// This implementation doesn't take advises
        /// </summary>
        public const int OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003);
        /// <summary>
        /// Data has same FORMATETC.,
        /// </summary>
        public const int DATA_S_SAMEFORMATETC = 0x00040130;
        /// <summary>
        /// Invalid FORMATETC structure
        /// </summary>
        public const int DV_E_FORMATETC = unchecked((int)0x80040064);
        /// <summary>
        /// Invalid lindex
        /// </summary>
        public const int DV_E_LINDEX = unchecked((int)0x80040068);
        /// <summary>
        /// Invalid tymed
        /// </summary>
        public const int DV_E_TYMED = unchecked((int)0x80040069);
        /// <summary>
        /// Invalid aspect(s)
        /// </summary>
        public const int DV_E_DVASPECT = unchecked((int)0x8004006B);
        /// <summary>
        /// An error occurred when allocating the medium.
        /// </summary>
        public const int STG_E_MEDIUMFULL = unchecked((int)0x80030070);
    }
}
