using System.Runtime.InteropServices;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace DropMutipleFilesComAsyncWinForms.Com
{
    static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint GlobalAlloc(
            [MarshalAs(UnmanagedType.U4)][In] AllocFlag uFlags,
            [In] uint dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint GlobalFree(nint hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint GlobalLock(nint hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalUnlock(nint hMem);

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
    }
}
