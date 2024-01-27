using System.Runtime.InteropServices;

namespace DropMultipleFilesComAsyncDragImageWinForms.Com
{
    [ComImport]
    [Guid("00000121-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDropSource
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        DropSourceQueryContinueDragResult QueryContinueDrag(
            [In][MarshalAs(UnmanagedType.Bool)] bool fEscapePressed,
            [In][MarshalAs(UnmanagedType.I4)] int grfKeyState);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        DropSourceGiveFeedbackResult GiveFeedback(
            [In][MarshalAs(UnmanagedType.I4)] DragDropEffects dwEffect);
    }
}
