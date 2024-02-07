using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace DropMultipleFilesComAsyncDragImageWpf.Com
{
    internal static class STGMEDIUMUtility
    {
        public static int CopyTo(this ref readonly STGMEDIUM src, short cfFormat, ref STGMEDIUM dest)
        {
            if (dest.tymed != TYMED.TYMED_NULL && src.tymed != dest.tymed)
            {
                return NativeMethods.DV_E_TYMED;
            }
            switch (src.tymed)
            {
                case TYMED.TYMED_ISTREAM:
                    dest.unionmember = src.unionmember;
                    dest.tymed = TYMED.TYMED_ISTREAM;
                    Marshal.AddRef(src.unionmember);
                    return NativeMethods.S_OK;
                case TYMED.TYMED_ISTORAGE:
                    dest.unionmember = src.unionmember;
                    dest.tymed = TYMED.TYMED_ISTORAGE;
                    Marshal.AddRef(src.unionmember);
                    return NativeMethods.S_OK;
                default:
                    dest.unionmember =
                        NativeMethods.OleDuplicateData(
                            src.unionmember,
                            cfFormat,
                            AllocFlag.GMEM_MOVEABLE);
                    dest.tymed = src.tymed;
                    return NativeMethods.S_OK;
            }
        }
    }
}
