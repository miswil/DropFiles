using System.Text;

namespace DropMultipleFilesComWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void splitContainer1_Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) { return; }

            var data = this.CreateDataObject();
            this.splitContainer1.Panel1.DoDragDrop(data, DragDropEffects.Copy);
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) { return; }

            var data = this.CreateDataObject();
            this.splitContainer1.Panel1.DoDragDrop(data, DragDropEffects.Copy);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var data = this.CreateDataObject();
            Clipboard.SetDataObject(data);
        }

        private object CreateDataObject()
        {
            var originalContents = new MemoryStream(Encoding.UTF8.GetBytes(this.textBox1.Text));
            var upeerContents = new MemoryStream(Encoding.UTF8.GetBytes(this.textBox1.Text.ToUpper()));
            var lowerContents = new MemoryStream(Encoding.UTF8.GetBytes(this.textBox1.Text.ToLower()));
            var fileSize = originalContents.Length;
            var data = new MyDataObject();
            data.SetFileGroupDescriptor(new IDroppedObjectInfo[]
            {
                new DroppedDirectoryInfo("Files by drag & drop"),
                new DroppedFileInfo("Files by drag & drop\\original.txt", Size: fileSize),
                new DroppedFileInfo("Files by drag & drop\\upper.txt", Size: fileSize),
                new DroppedFileInfo("Files by drag & drop\\lower.txt", Size: fileSize),
            });
            data.SetFileContents(new Stream?[]
            {
                null, // directory
                originalContents,
                upeerContents,
                lowerContents,
            });
            return data;
        }
    }
}
