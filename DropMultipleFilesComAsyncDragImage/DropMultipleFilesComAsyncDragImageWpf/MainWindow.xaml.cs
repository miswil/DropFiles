using DropMultipleFilesComAsyncDragImageWpf.Com;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace DropMultipleFilesComAsyncDragImageWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDropSource
    {
        private IDropTargetHelper helper = (IDropTargetHelper)new DragDropHelper();
        private MyDataObject? currentData;

        public MainWindow()
        {
            InitializeComponent();
            this.helper.Show(true);
        }

        private async void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (!e.LeftButton.HasFlag(MouseButtonState.Pressed))
            {
                return;
            }
            using var data = this.CreateDataObject();
            this.currentData = data;
            try
            {
                var result = await DragDropEx.DoDragDropAsync(
                    this,
                    data,
                    DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Link);
            }
            finally
            {
                this.currentData = null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var data = this.CreateDataObject();
            ((IDataObjectAsyncCapability)data).SetAsyncMode(true);
            Clipboard.SetDataObject(data);
        }

        private MyDataObject CreateDataObject()
        {
            var originalContents = new MemoryStream(Encoding.UTF8.GetBytes(this.text.Text));
            var upeerContents = new MemoryStream(Encoding.UTF8.GetBytes(this.text.Text.ToUpper()));
            var lowerContents = new MemoryStream(Encoding.UTF8.GetBytes(this.text.Text.ToLower()));
            var fileSize = originalContents.Length;
            var data = new MyDataObject();
            data.SetFileGroupDescriptorFetch(async () =>
            {
                await Task.Delay(1000);
                return new IDroppedObjectInfo[]
                {
                    new DroppedDirectoryInfo("Files by drag & drop"),
                    new DroppedFileInfo("Files by drag & drop\\original.txt", Size: fileSize),
                    new DroppedFileInfo("Files by drag & drop\\upper.txt", Size: fileSize),
                    new DroppedFileInfo("Files by drag & drop\\lower.txt", Size: fileSize),
                };
            });
            data.SetFileContentFetch(new[]
            {
                null,
                async () =>
                {
                    await Task.Delay(5000);
                    return (Stream)originalContents;
                },
                async () =>
                {
                    await Task.Delay(5000);
                    return (Stream)upeerContents;
                },
                async () =>
                {
                    await Task.Delay(5000);
                    return (Stream)lowerContents;
                },
            });
            var dragImage = new UserControl1();
            dragImage.ForceRender(dragImage.Width, dragImage.Height);
            data.SetDragImage(dragImage, true);
            return data;
        }

        DropSourceGiveFeedbackResult IDropSource.GiveFeedback(DragDropEffects dwEffect)
        {
            if (this.currentData!.IsShowingLayered() && 
                this.currentData!.IsShowingText())
            {
                this.currentData!.UpdateDragImage();
                Mouse.SetCursor(Cursors.Arrow);
                return DropSourceGiveFeedbackResult.Ok;
            }
            else
            {
                return DropSourceGiveFeedbackResult.UseDefaultCursors;
            }
        }

        DropSourceQueryContinueDragResult IDropSource.QueryContinueDrag(
            bool fEscapePressed, DragDropKeyStates grfKeyState)
        {
            if (fEscapePressed)
            {
                return DropSourceQueryContinueDragResult.Cancel;
            }
            if (!grfKeyState.HasFlag(DragDropKeyStates.LeftMouseButton) &&
                !grfKeyState.HasFlag(DragDropKeyStates.RightMouseButton))
            {
                return DropSourceQueryContinueDragResult.Drop;
            }
            return DropSourceQueryContinueDragResult.Ok;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data is IComDataObject comData)
            {
                var point = e.GetPosition(this);
                var dPoint = new Win32Point { x = (int)point.X, y = (int)point.Y };
                try
                {
                    helper.Drop(comData, ref dPoint, e.Effects);
                }
                catch { }
            }
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.KeyStates.HasFlag(DragDropKeyStates.ShiftKey))
            {
                e.Effects = DragDropEffects.Move;
            }
            else if (e.KeyStates.HasFlag(DragDropKeyStates.AltKey))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.Copy;
            }
            if (e.Data is IComDataObject comData)
            {
                var point = e.GetPosition(this);
                var dPoint = new Win32Point { x = (int)point.X, y = (int)point.Y };
                var handle = new WindowInteropHelper(this).Handle;
                helper.DragEnter(handle, comData, ref dPoint, e.Effects);
                switch (e.Effects)
                {
                    case DragDropEffects.Copy:
                        comData.SetDropDescription(
                            DropImageType.DROPIMAGE_COPY,
                            "%1にコピー",
                            "ここ");
                        break;
                    case DragDropEffects.Link:
                        comData.SetDropDescription(
                            DropImageType.DROPIMAGE_LINK,
                            "%1にリンク",
                            "ここ");
                        break;
                    case DragDropEffects.Move:
                        comData.SetDropDescription(
                            DropImageType.DROPIMAGE_MOVE,
                            "%1に移動",
                            "ここ");
                        break;
                    default:
                        break;
                }
            }
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            if (e.KeyStates.HasFlag(DragDropKeyStates.ShiftKey))
            {
                e.Effects = DragDropEffects.Move;
            }
            else if (e.KeyStates.HasFlag(DragDropKeyStates.AltKey))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.Copy;
            }
            if (e.Data is IComDataObject comData)
            {
                var point = e.GetPosition(this);
                var dPoint = new Win32Point { x = (int)point.X, y = (int)point.Y };
                helper.DragOver(ref dPoint, e.Effects);
                switch (e.Effects)
                {
                    case DragDropEffects.Copy:
                        comData.SetDropDescription(
                            DropImageType.DROPIMAGE_COPY,
                            "%1にコピー",
                            "ここ");
                        break;
                    case DragDropEffects.Link:
                        comData.SetDropDescription(
                            DropImageType.DROPIMAGE_LINK,
                            "%1にリンク",
                            "ここ");
                        break;
                    case DragDropEffects.Move:
                        comData.SetDropDescription(
                            DropImageType.DROPIMAGE_MOVE,
                            "%1に移動",
                            "ここ");
                        break;
                    default:
                        break;
                }
            }
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            if (e.Data is IComDataObject comData)
            {
                helper.DragLeave();
            }
        }
    }
}