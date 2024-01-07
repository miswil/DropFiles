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

namespace DropSingleFile
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
            var fileContents = new MemoryStream(Encoding.UTF8.GetBytes(this.text.Text));
            var fileSize = fileContents.Length;
            var data = new DataObject();
            data.SetFileGroupDescriptor("example.txt", fileSize: fileSize);
            data.SetFileContents(fileContents);
            DragDrop.DoDragDrop((Border)sender, data, DragDropEffects.Copy);
        }
    }
}