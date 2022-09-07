using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;


namespace Image_Stitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            string a = System.AppDomain.CurrentDomain.BaseDirectory;
            Debug.WriteLine(a);
            InitializeComponent();


        }

        

        class input_image
        {
            public string ID { get; set;}
            public string Path { get; set;}

        }

        private void btn_load_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_path.Text))
            {
                DateTime d = new DateTime();
                d = DateTime.Now;
                Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]" + ": No path has been selected.");
                Status_Box.AppendText(Environment.NewLine);
            }
            else
            {
                // Clear the ListView
                list_image.Items.Clear();

                // Get only file paths with image extension
                var files = Directory.EnumerateFiles(txt_path.Text, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".tiff"));
                string[] images = (from string c in files select c.ToString()).ToArray();

                // Load the images list to ListView
                int j = 1;
                foreach (string img in images)
                {

                    list_image.Items.Add(new input_image() { ID = j.ToString(), Path = img.ToString() });
                    j++;
                }

                // Show number of images in info textbox
                txt_info.Text = "Total_image: " + (j - 1).ToString();

                if (j == 0)
                {
                    error msg = new error();
                    msg.ShowDialog();
                }
            }
        }

        private void btn_open_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            txt_path.Text = dialog.SelectedPath;

        }

        private void list_image_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
