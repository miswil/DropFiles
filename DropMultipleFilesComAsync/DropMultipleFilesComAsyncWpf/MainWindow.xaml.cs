using DropMultipleFilesComAsyncWpf.Com;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DropMultipleFilesComAsyncWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDropSource
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (!e.LeftButton.HasFlag(MouseButtonState.Pressed))
            {
                return;
            }
            using var data = this.CreateDataObject();
            var result = await DragDropEx.DoDragDropAsync(this, data, DragDropEffects.Copy);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var data = this.CreateDataObject();
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
            data.SetFileContentFetch(new []
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
            return data;
        }

        DropSourceGiveFeedbackResult IDropSource.GiveFeedback(DragDropEffects dwEffect)
        {
            return DropSourceGiveFeedbackResult.UseDefaultCursors;
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
    }
}