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
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using System.Collections.Generic;
using System.Windows.Controls.DataVisualization.Charting;
using System.ComponentModel;

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

        }
        void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        class input_image
        {
            public string ID { get; set;}
            public string Path { get; set;}

        }

        private void run_cmd(string cmd, string args)
        {
            // Run python script get_image_type.py to return type of image
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = cmd;
            start.Arguments = args;
            // Don't show cmd
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadLine();
                    if (result == "3")
                    {
                        // If image has 3 channel
                        txt_info.AppendText("The images is in color format (RGB).");
                        txt_info.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        // If image has 1 channel
                        txt_info.AppendText("The images is in gray scale format.");
                        txt_info.AppendText(Environment.NewLine);
                    }
                }
            }
        }

        private void cmd_histogram(string cmd, string args)
        {
            // Run python script histogram.py to return chart image
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = cmd;
            start.Arguments = args;
            // Don't show cmd
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                using (StreamReader reader = process.StandardOutput)
                {

                }
            }
        }

        private string ValidFileName(string filename)
        {
            string msg = "";

            if (list_image.Items.Count == 0)
            {
                // If no images have been loaded
                msg = "[Error] You need to load a folder which contains images first.";
            }
            else if (list_image.Items.Count != 0) {
                // Check if file name contain invalid character
                List<string> Pattern = new List<string> { "^", "<", ">", ";", "|", "'", "/", ",", "\\", ":", "=", "?", "\"", "*" };
                for (int i = 0; i < Pattern.Count; i++)
                {
                    if (filename.Contains(Pattern[i]))
                    {
                        msg = "[Error] File name can not contain invalid character " + "\"" + Pattern[i] + "\".";
                        break;
                    }
                }

                // Check if file name is invalid string
                List<string> Parts = new List<string> { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
                for (int i = 0; i < Parts.Count; i++)
                {
                    if (filename == Parts[i])
                    {
                        msg = "[Error] File name can not be " + "\"" + Parts[i] + "\"."; ;
                        break;
                    }
                }

                // Check if file name is empty
                if (filename == "")
                {
                    msg = "[Error] File name must not be blank.";
                }

                // If there is no error
            
                if (msg == "")
                {
                    msg = "[Error] File name is valid.";
                }
            }
            return msg;
        }


        private void btn_load_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_path.Text))
            {
                // If there is no path in Path Textbox
                DateTime d = new DateTime();
                d = DateTime.Now;
                Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]" + ": [Error] No path has been selected.");
                Status_Box.AppendText(Environment.NewLine);
            }
            else
            {
                // Clear the ListView and Info Textbox
                list_image.Items.Clear();
                txt_info.Text = "";

                // Get only file paths with image extension
                var files = Directory.EnumerateFiles(txt_path.Text, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".tiff") || s.EndsWith(".jpeg"));
                string[] images = (from string c in files select c.ToString()).ToArray();

                
                if (images.Length == 0)
                {
                    // If there is no image in chosen folder
                    DateTime d = new DateTime();
                    d = DateTime.Now;
                    Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]" + ": [Error] There is no images in the chosen folder.");
                    Status_Box.AppendText(Environment.NewLine);
                }

                else if (images.Length == 1)
                {
                    // If there is only one image in chosen folder
                    DateTime d = new DateTime();
                    d = DateTime.Now;
                    Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]" + ": [Error] There is only one image in the chosen folder.");
                    Status_Box.AppendText(Environment.NewLine);
                }

                else
                {
                    // If there is image in chosen folder
                    DateTime d = new DateTime();
                    d = DateTime.Now;
                    Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]" + ": Image loaded successfully.");
                    Status_Box.AppendText(Environment.NewLine);

                    // Load the images to ListView
                    int j = 1;
                    foreach (string img in images)
                    {

                        list_image.Items.Add(new input_image() { ID = j.ToString(), Path = img.ToString() });
                        j++;
                    }

                    // Show number of images in Info Textbox
                    txt_info.AppendText("Total images: " + (j - 1).ToString() + ".");
                    txt_info.AppendText(Environment.NewLine);

                    // Show type of image in Info Textbox
                    string path = System.AppDomain.CurrentDomain.BaseDirectory + "get_image_type.py" + " \"{0}\" " + "&";
                    run_cmd("C:\\Program Files\\Python39\\python.exe", string.Format(path, txt_path.Text));
                    
                    
                }
            }
        }

        private void btn_open_Click(object sender, RoutedEventArgs e)
        {
            // Open Explorer to choose folder
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            txt_path.Text = dialog.SelectedPath;

        }

        private void btn_select_Click(object sender, RoutedEventArgs e)
        {
            // Show update about output filename
            DateTime d = new DateTime();
            d = DateTime.Now;
            Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]: " + ValidFileName(txt_name.Text));
            Status_Box.AppendText(Environment.NewLine);
        }

        private void Status_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            Status_Box.ScrollToEnd();
        }

        private void btn_histogram_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "histogram\\grayscale_histogram.png"))
            {
                // Show histograms window
                Histograms histograms = new Histograms();
                histograms.Show();
            }
            else
            {
                // Show error, the histogram is not created
                DateTime d = new DateTime();
                d = DateTime.Now;
                Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]: " + " Please wait for the histograms to be created or reselect the image.");
                Status_Box.AppendText(Environment.NewLine);
            }
            
        }

        private void btn_choose_Click(object sender, RoutedEventArgs e)
        {

            // Open Explorer to choose image
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = ".jpg";
            dialog.Filter = "JPG Files (*.jpg)|*.jpg|*.jpeg|PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|TIFF Files (*.tiff)|*.tiff";
            dialog.ShowDialog();
            txt_path_histogram.Text = dialog.FileName;
            
            // Get histogram image
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "histogram\\" + "histogram.py" + " \"{0}\" ";
            cmd_histogram("C:\\Program Files\\Python39\\python.exe", string.Format(path, txt_path_histogram.Text));

            // Show update about histogram
            DateTime d = new DateTime();
            d = DateTime.Now;
            while (true)
            {
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "histogram\\grayscale_histogram.png"))
                {
                    Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]: " + " Histograms created successfully.");
                    Status_Box.AppendText(Environment.NewLine);
                    break;
                }
            }
            
        }
    }
    
}
