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

        public static Task<DragDropEffects> DoDragDropAsync(IDropSource dropSource, IComDataObject data, DragDropEffects allowedEffects)
        {
            if (data is not MyDataObject myData)
            {
                throw new ArgumentException($"{nameof(data)} must be {nameof(IDataObjectAsyncCapability)}.");
            }
            ((IDataObjectAsyncCapability)myData).SetAsyncMode(true);

            var dwEffect = DragDropEffects.None;
            var tcs = new TaskCompletionSource<DragDropEffects>();
            myData.AsyncOperationEnd += (_, _) => tcs.SetResult(dwEffect);
            NativeMethods.DoDragDrop(data, dropSource, allowedEffects, out dwEffect);
            ((IDataObjectAsyncCapability)myData).InOperation(out var inOperation);
            if (!inOperation)
            {
                return Task.FromResult(dwEffect);
            }
            else
            {
                return tcs.Task;
            }
        }
    }
}
