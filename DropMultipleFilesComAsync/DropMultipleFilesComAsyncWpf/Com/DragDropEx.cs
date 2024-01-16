using System.Windows;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace DropMultipleFilesComAsyncWpf.Com
{
    internal static class DragDropEx
    {
        public static DragDropEffects DoDragDrop(IDropSource dropSource, IComDataObject data, DragDropEffects allowedEffects)
        {
            NativeMethods.DoDragDrop(data, dropSource, allowedEffects, out var dwEffect);
            return dwEffect;
        }
    }
}
