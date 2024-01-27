using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace DropMultipleFilesComAsyncDragImageWinForms.Com
{
    internal static class DragDropEx
    {
        public static Task<DragDropEffects> DoDragDropAsync(
            this Control dropSource,
            IComDataObject data,
            DragDropEffects allowedEffects,
            Bitmap dragImage,
            Point cursorOffset,
            bool useDefaultDragImage)
        {
            if (data is not MyDataObject myData)
            {
                throw new ArgumentException($"{nameof(data)} must be {nameof(IDataObjectAsyncCapability)}.");
            }
            ((IDataObjectAsyncCapability)myData).SetAsyncMode(true);

            var dwEffect = DragDropEffects.None;
            var tcs = new TaskCompletionSource<DragDropEffects>();
            myData.AsyncOperationEnd += (_, _) => tcs.SetResult(dwEffect);
            dwEffect = dropSource.DoDragDrop(data, allowedEffects, dragImage, cursorOffset, useDefaultDragImage);
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
