using System.Text;

namespace DropSingleFileWinForms
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
            var fileContents = new MemoryStream(Encoding.UTF8.GetBytes(this.textBox1.Text));
            var fileSize = fileContents.Length;
            var data = new DataObject();
            data.SetFileGroupDescriptor("example.txt", fileSize: fileSize);
            data.SetFileContents(fileContents);
            return data;
        }
    }
}
