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

namespace Image_Stitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            

            InitializeComponent();

            //list_image.Items.Add(new input_image() { ID=})
        }

        

        class input_image
        {
            public string ID { get; set;}
            public string Path { get; set;}

        }

        private void btn_load_Click(object sender, RoutedEventArgs e)
        {
            list_image.Items.Clear();
            String[] images = Directory.GetFiles(txt_path.Text);
            
            //load the list image list
            int j = 0;
            foreach (string img in images)
            {

                list_image.Items.Add(new input_image() { ID = j.ToString(), Path = img.ToString() });             
                j++;
            }
            txt_info.Text = "Total_image: " + j.ToString();
            if (j == 0)
            {
                error msg = new error();
                msg.ShowDialog();
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
