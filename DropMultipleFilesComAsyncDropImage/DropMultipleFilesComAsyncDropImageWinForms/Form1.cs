using DropMultipleFilesComAsyncIconWinForms.Com;
using System.Text;

namespace DropMultipleFilesComAsyncIconWinForms
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
            await this.StartDragAsync();
        }

        private async void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) { return; }
            await this.StartDragAsync();
        }

        private async Task StartDragAsync()
        {
            using var data = this.CreateDataObject();
            var w = this.splitContainer1.Panel1.Width;
            var h = this.splitContainer1.Panel1.Height;
            var bitmap = new Bitmap(w, h);
            this.splitContainer1.Panel1.DrawToBitmap(bitmap, new Rectangle(0, 0, w, h));
            var result = await this.DoDragDropAsync(
                data,
                DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Link,
                dragImage: bitmap,
                cursorOffset: default,
                useDefaultDragImage: false);
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

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            // Set the effect based upon the KeyState.
            if ((e.KeyState & (8 + 32)) == (8 + 32) &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // KeyState 8 + 32 = CTRL + ALT
                e.Effect = DragDropEffects.Link;
                e.DropImageType = DropImageType.Link;
                e.Message = "ここにリンク";
            }
            else if ((e.KeyState & 32) == 32 &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // ALT KeyState for link.
                e.Effect = DragDropEffects.Link;
                e.DropImageType = DropImageType.Link;
                e.Message = "ここにリンク";
            }
            else if ((e.KeyState & 4) == 4 &&
                (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // SHIFT KeyState for move.
                e.Effect = DragDropEffects.Move;
                e.DropImageType = DropImageType.Move;
                e.Message = "ここに移動";
            }
            else if ((e.KeyState & 8) == 8 &&
                (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                // CTRL KeyState for copy.
                e.Effect = DragDropEffects.Copy;
                e.DropImageType = DropImageType.Copy;
                e.Message = "ここにコピー";
            }
            else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // By default, the drop action should be move, if allowed.
                e.Effect = DragDropEffects.Move;
                e.DropImageType = DropImageType.Move;
                e.Message = "ここに移動";
            }
            else
            {
                e.Effect = DragDropEffects.None;
                e.DropImageType = DropImageType.None;
            }
        }

        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            // Set the effect based upon the KeyState.
            if ((e.KeyState & (8 + 32)) == (8 + 32) &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // KeyState 8 + 32 = CTRL + ALT
                e.Effect = DragDropEffects.Link;
                e.DropImageType = DropImageType.Link;
                e.Message = "ここにリンク";
            }
            else if ((e.KeyState & 32) == 32 &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // ALT KeyState for link.
                e.Effect = DragDropEffects.Link;
                e.DropImageType = DropImageType.Link;
                e.Message = "ここにリンク";
            }
            else if ((e.KeyState & 4) == 4 &&
                (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // SHIFT KeyState for move.
                e.Effect = DragDropEffects.Move;
                e.DropImageType = DropImageType.Move;
                e.Message = "ここに移動";
            }
            else if ((e.KeyState & 8) == 8 &&
                (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                // CTRL KeyState for copy.
                e.Effect = DragDropEffects.Copy;
                e.DropImageType = DropImageType.Copy;
                e.Message = "ここにコピー";
            }
            else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // By default, the drop action should be move, if allowed.
                e.Effect = DragDropEffects.Move;
                e.DropImageType = DropImageType.Move;
                e.Message = "ここに移動";
            }
            else
            {
                e.Effect = DragDropEffects.None;
                e.DropImageType = DropImageType.None;
            }
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
