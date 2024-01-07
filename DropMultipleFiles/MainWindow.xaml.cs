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

namespace DropMultipleFiles
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
            var originalContents = new MemoryStream(Encoding.UTF8.GetBytes(this.text.Text));
            var upeerContents = new MemoryStream(Encoding.UTF8.GetBytes(this.text.Text.ToUpper()));
            var lowerContents = new MemoryStream(Encoding.UTF8.GetBytes(this.text.Text.ToLower()));
            var fileSize = originalContents.Length;
            var data = new MyDataObject();
            data.SetFileGroupDescriptor(new DropFileInfo[]
            {
                new DropFileInfo("original.txt", Size: fileSize),
                new DropFileInfo("upper.txt", Size: fileSize),
                new DropFileInfo("lower.txt", Size: fileSize),
            });
            data.SetFileContents(new Stream[]
            {
                originalContents,
                upeerContents,
                lowerContents,
            });
            DragDrop.DoDragDrop((Border)sender, data, DragDropEffects.Copy);
        }
    }
}