using System.Runtime.InteropServices;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace DropMultipleFilesComAsyncDragImageWpf.Com
{
    [ComVisible(true)]
    [ComImport]
    [Guid("83E07D0D-0C5F-4163-BF1A-60B274051E40")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDragSourceHelper2
    {
        void InitializeFromBitmap(
            [In][MarshalAs(UnmanagedType.Struct)] ref ShDragImage dragImage,
            [In][MarshalAs(UnmanagedType.Interface)] IComDataObject dataObject);

        void InitializeFromWindow(
            [In] IntPtr hwnd,
            [In] ref Win32Point pt,
            [In][MarshalAs(UnmanagedType.Interface)] IComDataObject dataObject);

        void SetFlags(
            [In][MarshalAs(UnmanagedType.I4)] DSH_FLAGS dwFlags);
    }
}