using System.Runtime.InteropServices;

namespace DropMultipleFilesComAsyncIconWpf.Com
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ShDragImage
    {
        public Win32Size sizeDragImage;
        public Win32Point ptOffset;
        public IntPtr hbmpDragImage;
        public int crColorKey;
    }
}