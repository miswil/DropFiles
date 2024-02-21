using System.IO.Pipes;
using System.Text;

namespace DropDirectlyWinForms
{
    public partial class Form1 : Form
    {
        private Task waitDropTask;
        public Form1()
        {
            InitializeComponent();
            this.waitDropTask = Task.Run(() => this.WaitForDrop());
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) { return; }
            this.StartDrag();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) { return; }
            this.StartDrag();
        }

        private void StartDrag()
        {
            var data = new DataObject();
            var specialDirPath = Path.Combine(Path.GetTempPath(), "DropSource");
            Directory.CreateDirectory(specialDirPath);
            data.SetData(DataFormats.FileDrop, new[] { specialDirPath });
            this.DoDragDrop(data, DragDropEffects.Copy);
        }

        private async Task WaitForDrop()
        {
            while (true)
            {
                using var pipe = new NamedPipeServerStream(
                    "com.example.dropdirectly",
                    PipeDirection.In,
                    1,
                    PipeTransmissionMode.Byte);
                await pipe.WaitForConnectionAsync();
                var reader = new StreamReader(pipe, Encoding.Unicode);
                var dest = Path.GetDirectoryName(await reader.ReadToEndAsync());
                this.Invoke(() => MessageBox.Show(this, $"Dropped to {dest}"));
            }
        }
    }
}
