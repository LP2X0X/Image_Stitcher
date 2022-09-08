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

            InitializeComponent();


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
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadLine();
                    Debug.WriteLine(result);
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

        private void btn_load_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_path.Text))
            {
                // If there is no path in Path Textbox
                DateTime d = new DateTime();
                d = DateTime.Now;
                Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]" + ": No path has been selected.");
                Status_Box.AppendText(Environment.NewLine);
            }
            else
            {
                // Clear the ListView and Info Textbox
                list_image.Items.Clear();
                txt_info.Text = "";

                // Get only file paths with image extension
                var files = Directory.EnumerateFiles(txt_path.Text, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".tiff"));
                string[] images = (from string c in files select c.ToString()).ToArray();

                
                if (images.Length == 0)
                {
                    // If there is no image in chosen folder
                    DateTime d = new DateTime();
                    d = DateTime.Now;
                    Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]" + ": There is no images in the chosen folder.");
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
                    string path = System.AppDomain.CurrentDomain.BaseDirectory + "get_image_type.py" + " \"{0}\"";
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

    }
}
