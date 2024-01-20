using DropMutipleFilesComAsyncWinForms.Com;
using System.Text;

namespace DropMutipleFilesComAsyncWinForms
{
    public partial class Form1 : Form, IDropSource
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void splitContainer1_Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) { return; }

            using var data = this.CreateDataObject();
            var result = await DragDropEx.DoDragDropAsync(this, data, DragDropEffects.Copy);
        }

        private async void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) { return; }

            using var data = this.CreateDataObject();
            var result = await DragDropEx.DoDragDropAsync(this, data, DragDropEffects.Copy);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var data = this.CreateDataObject();
            Clipboard.SetDataObject(data);
        }

        private MyDataObject CreateDataObject()
        {
            var originalContents = new MemoryStream(Encoding.UTF8.GetBytes(this.textBox1.Text));
            var upeerContents = new MemoryStream(Encoding.UTF8.GetBytes(this.textBox1.Text.ToUpper()));
            var lowerContents = new MemoryStream(Encoding.UTF8.GetBytes(this.textBox1.Text.ToLower()));
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
            return data;
        }

        DropSourceGiveFeedbackResult IDropSource.GiveFeedback(DragDropEffects dwEffect)
        {
            return DropSourceGiveFeedbackResult.UseDefaultCursors;
        }

        DropSourceQueryContinueDragResult IDropSource.QueryContinueDrag(
            bool fEscapePressed, int grfKeyState)
        {
            if (fEscapePressed)
            {
                return DropSourceQueryContinueDragResult.Cancel;
            }
            if (((grfKeyState & 1) != 1) &&
                ((grfKeyState & 2) != 2))
            {
                return DropSourceQueryContinueDragResult.Drop;
            }
            return DropSourceQueryContinueDragResult.Ok;
        }
    }
}
