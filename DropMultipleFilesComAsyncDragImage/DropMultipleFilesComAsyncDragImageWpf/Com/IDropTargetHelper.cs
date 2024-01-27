using System.Runtime.InteropServices;
using System.Windows;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace DropMultipleFilesComAsyncDragImageWpf.Com
{
    [ComVisible(true)]
    [ComImport]
    [Guid("4657278B-411B-11D2-839A-00C04FD918D0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDropTargetHelper
    {
        void DragEnter(
            [In] IntPtr hwndTarget,
            [In][MarshalAs(UnmanagedType.Interface)] IComDataObject dataObject,
            [In] ref Win32Point pt,
            [In][MarshalAs(UnmanagedType.I4)] DragDropEffects effect);
        void DragLeave();
        void DragOver(
            [In] ref Win32Point pt,
            [In][MarshalAs(UnmanagedType.I4)] DragDropEffects effect);
        void Drop(
            [In, MarshalAs(UnmanagedType.Interface)] IComDataObject dataObject,
            [In] ref Win32Point pt,
            [In][MarshalAs(UnmanagedType.I4)] DragDropEffects effect);
        void Show(
            [In][MarshalAs(UnmanagedType.Bool)] bool show);
    }
}