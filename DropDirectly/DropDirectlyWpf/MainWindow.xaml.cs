using System.Collections.Specialized;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace DropDirectlyWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Task waitDropTask;
        public MainWindow()
        {
            InitializeComponent();
            this.waitDropTask = Task.Run(() => this.WaitForDrop());
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (!e.LeftButton.HasFlag(MouseButtonState.Pressed))
            {
                return;
            }
            var data = new DataObject();
            var specialDirPath = Path.Combine(Path.GetTempPath(), "DropSource");
            Directory.CreateDirectory(specialDirPath);
            data.SetData(DataFormats.FileDrop, new[] { specialDirPath });
            DragDrop.DoDragDrop((Border)sender, data, DragDropEffects.Copy);
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
                await this.Dispatcher.BeginInvoke(() => MessageBox.Show(this, $"Dropped to {dest}"));
            }
        }
    }
}