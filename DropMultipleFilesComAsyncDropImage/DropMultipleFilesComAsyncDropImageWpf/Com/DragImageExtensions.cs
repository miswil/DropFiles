using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace DropMultipleFilesComAsyncIconWpf.Com
{
    internal static class DragImageExtensions
    {
        public static void SetDragImage(
            this IComDataObject dataObject,
            UIElement ui)
        {
            var bmp = ui.ToBitmap();
            var hbitmap = bmp.GetHbitmap();
            try
            {
                var dragImage = new ShDragImage
                {
                    hbmpDragImage = hbitmap,
                    ptOffset = new Win32Point { x = 0, y = bmp.Height },
                    sizeDragImage = new Win32Size { cx = bmp.Width, cy = bmp.Height },
                    crColorKey = Color.White.ToArgb()
                };
                var helper = (IDragSourceHelper2)new DragDropHelper();
                helper.SetFlags(DSH_FLAGS.DSH_ALLOWDROPDESCRIPTIONTEXT);
                helper.InitializeFromBitmap(ref dragImage, dataObject);
            }
            catch
            {
                NativeMethods.DeleteObject(hbitmap);
            }
        }

        public static void SetDropDescription(
            this IComDataObject dataObject,
            DropImageType dropImageType,
            string message,
            string? insert = null)
        {
            var dd = new DROPDESCRIPTION
            {
                types = dropImageType,
                szMessage = message,
                szInsert = insert
            };
            var size = Marshal.SizeOf<DROPDESCRIPTION>();
            var hMem = NativeMethods.GlobalAlloc(AllocFlag.GMEM_MOVEABLE, size);
            try
            {
                var format = new FORMATETC
                {
                    cfFormat = (short)DataFormats.GetDataFormat("DropDescription").Id,
                    lindex = -1,
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    tymed = TYMED.TYMED_HGLOBAL,
                    ptd = IntPtr.Zero,
                };
                var ptr = NativeMethods.GlobalLock(hMem);
                try
                {
                    Marshal.StructureToPtr(dd, ptr, false);
                }
                finally
                {
                    NativeMethods.GlobalUnlock(hMem);
                }
                var medium = new STGMEDIUM
                {
                    tymed = TYMED.TYMED_HGLOBAL,
                    unionmember = hMem
                };
                dataObject.SetData(ref format, ref medium, true);
            }
            catch
            {
                NativeMethods.GlobalFree(hMem);
                throw;
            }
        }

        public static void UpdateDragImage(
            this IComDataObject comDataObject)
        {
            var format = new FORMATETC
            {
                tymed = TYMED.TYMED_HGLOBAL,
                lindex = -1,
                dwAspect = DVASPECT.DVASPECT_CONTENT,
                cfFormat = (short)DataFormats.GetDataFormat("DragWindow").Id,
                ptd = IntPtr.Zero,
            };

            if (NativeMethods.S_OK != comDataObject.QueryGetData(ref format))
            {
                return;
            }

            comDataObject.GetData(ref format, out var medium);
            try
            {
                IntPtr hMem = medium.unionmember;
                var pDragWindow = NativeMethods.GlobalLock(hMem);
                try
                {
                    var hwnd = Marshal.PtrToStructure<IntPtr>(pDragWindow);
                    NativeMethods.PostMessage(hwnd, NativeMethods.DDWM_UPDATEWINDOW, IntPtr.Zero, IntPtr.Zero);
                    var error = Marshal.GetLastWin32Error();
                }
                finally
                {
                    NativeMethods.GlobalUnlock(hMem);
                }
            }
            finally
            {
                NativeMethods.ReleaseStgMedium(ref medium);
            }
        }

        public static bool IsShowingLayered(
            this IComDataObject comDataObject)
        {
            var format = new FORMATETC
            {
                cfFormat = (short)DataFormats.GetDataFormat("IsShowingLayered").Id,
                dwAspect = DVASPECT.DVASPECT_CONTENT,
                lindex = -1,
                tymed = TYMED.TYMED_HGLOBAL,
            };
            if (NativeMethods.S_OK != comDataObject.QueryGetData(ref format))
            {
                return false;
            }
            comDataObject.GetData(ref format, out var medium);
            try
            {
                var hMem = medium.unionmember;
                var ptr = NativeMethods.GlobalLock(hMem);
                try
                {
                    var isShowingLayered = Marshal.PtrToStructure<int>(ptr);
                    return isShowingLayered != 0;
                }
                finally
                {
                    NativeMethods.GlobalUnlock(hMem);
                }
            }
            finally
            {
                NativeMethods.ReleaseStgMedium(ref medium);
            }
        }
    }
}
