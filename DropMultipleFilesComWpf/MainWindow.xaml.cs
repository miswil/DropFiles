using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DropMultipleFilesComWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (!e.LeftButton.HasFlag(MouseButtonState.Pressed))
            {
                return;
            }
            var data = this.CreateDataObject();
            DragDrop.DoDragDrop((Border)sender, data, DragDropEffects.Copy);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var data = this.CreateDataObject();
            Clipboard.SetDataObject(data);
        }

        private object CreateDataObject()
        {
            var originalContents = new MemoryStream(Encoding.UTF8.GetBytes(this.text.Text));
            var upeerContents = new MemoryStream(Encoding.UTF8.GetBytes(this.text.Text.ToUpper()));
            var lowerContents = new MemoryStream(Encoding.UTF8.GetBytes(this.text.Text.ToLower()));
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