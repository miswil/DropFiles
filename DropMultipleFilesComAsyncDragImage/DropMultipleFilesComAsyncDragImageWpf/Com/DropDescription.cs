using System.Runtime.InteropServices;

namespace DropMultipleFilesComAsyncDragImageWpf.Com
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct DROPDESCRIPTION
    {
        [MarshalAs(UnmanagedType.I4)]
        public DropImageType types;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szMessage;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string? szInsert;
    }
}
