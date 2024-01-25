using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;

namespace DropMultipleFilesComAsyncIconWpf.Com
{
    [ComImport]
    [Guid("3D8B0590-F691-11d2-8EA9-006097DF5BD4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDataObjectAsyncCapability
    {
        void SetAsyncMode([In][MarshalAs(UnmanagedType.Bool)] bool fDoOpAsync);
        void GetAsyncMode([Out][MarshalAs(UnmanagedType.Bool)] out bool pfIsOpAsync);
        void StartOperation([In] IBindCtx pbcReserved);
        void InOperation([Out][MarshalAs(UnmanagedType.Bool)] out bool pfInAsyncOp);
        void EndOperation([In] int hResult, [In] IBindCtx pbcReserved, [In][MarshalAs(UnmanagedType.U4)] DragDropEffects dwEffects);
    }
}
