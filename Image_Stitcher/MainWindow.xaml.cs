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
using System.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;



namespace Image_Stitcher
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Create change property event and function
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property_name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
        }

        // Public variables for histogram
        public int Min_H;
        public int Max_H;
        public int Max_A;
        public int Min_A;
        public string data;

        // Min histogram range
        public int min_h
        {
            get { return Min_H; }
            set
            {
                Min_H = value;
                OnPropertyChanged("min_h");
            }
        }

        // Max histogram range
        public int max_h
        {
            get { return Max_H; }
            set
            {
                Max_H = value;
                OnPropertyChanged("max_h");
            }
        }

        // Max histogram Y Axis
        public int MaxAxisValue
        {
            get { return Max_A; }
            set
            {
                Max_A = value;
                OnPropertyChanged("MaxAxisValue");
            }
        }

        // Min histogram Y Axis
        public int MinAxisValue
        {
            get { return Min_A; }
            set
            {
                Min_A = value;
                OnPropertyChanged("MinAxisValue");
            }
        }

        public string date_time;
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }


        public MainWindow()
        {

            InitializeComponent();
            combobox_name.Items.Add("Barcode");
            SeriesCollection = new SeriesCollection();
            DateTime d = new DateTime();
            d = DateTime.Now;
            date_time = d.ToString("dd-MM-yyyy") + "_" + d.ToString("HH-mm-ss");
            combobox_name.Items.Add(date_time);

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

        private string cmd_histogram(string cmd, string args)
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
                    string result = reader.ReadLine();
                    return result;
                }
            }
        }

        private string ValidFileName(string filename)
        {
            string msg = "";

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
                msg = "File name is valid.";
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
                    string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\get_image_type.py" + " \"{0}\" ";
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

        private void btn_check_Click(object sender, RoutedEventArgs e)
        {
            // Show update about output filename
            DateTime d = new DateTime();
            d = DateTime.Now;
            Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]: " + ValidFileName(combobox_name.Text));
            Status_Box.AppendText(Environment.NewLine);
        }

        private void Status_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            Status_Box.ScrollToEnd();
        }

        private void btn_histogram_Click(object sender, RoutedEventArgs e)
        {

            if (txt_path_histogram.Text != "")
            {
                // Clear previous chart
                SeriesCollection.Clear();
                SeriesCollection.Add
            (
                new ColumnSeries
                {
                    Title = "HISTOGRAM",
                    Values = new ChartValues<int> { },
                    ColumnPadding = 0,
                }
            );

                Labels = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50",
                "51", "52", "53", "54", "55", "56", "57", "58", "59", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "100",
                "101", "102", "103", "104", "105", "106", "107", "108", "109", "110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139", "140", "141", "142", "143", "144", "145", "146", "147", "148", "149", "150",
                "151", "152", "153", "154", "155", "156", "157", "158", "159", "160", "161", "162", "163", "164", "165", "166", "167", "168", "169", "170", "171", "172", "173", "174", "175", "176", "177", "178", "179", "180", "181", "182", "183", "184", "185", "186", "187", "188", "189", "190", "191", "192", "193", "194", "195", "196", "197", "198", "199", "200",
                "201", "202", "203", "204", "205", "206", "207", "208", "209", "210", "211", "212", "213", "214", "215", "216", "217", "218", "219", "220", "221", "222", "223", "224", "225", "226", "227", "228", "229", "230", "231", "232", "233", "234", "235", "236", "237", "238", "239", "240", "241", "242", "243", "244", "245", "246", "247", "248", "249", "250", "251", "252", "253", "254", "255"};

                // Get histogram data from MainWindow
                string[] values = data.Split(',');
                int[] int_values = new int[values.Length];

                // Convert value from string to int and add to chart
                for ( int i = 0; i < values.Length; i++ )
                {
                    int y = Convert.ToInt32(values[i]);
                    int_values[i] = y;
                    SeriesCollection[0].Values.Add(y);
                }

                // Get min, max data by user
                min_h = Convert.ToInt32(select_min.Value);
                max_h = Convert.ToInt32(select_max.Value);


                // Get min, max value for Y Axis
                MaxAxisValue = int_values.Max();
                MinAxisValue = 0;

                Formatter = value => value.ToString("N");
                DataContext = this;
                
            }
            else
            {
                // Can not leave image path blank
                DateTime d = new DateTime();
                d = DateTime.Now;
                Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]: " + "[Error] Please select an image in order to calculate histogram.");
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

            if (txt_path_histogram.Text == "")
            {
                // Can not leave image path blank
                DateTime d = new DateTime();
                d = DateTime.Now;
                Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]: " + "[Error] Please select an image in order to calculate histogram.");
                Status_Box.AppendText(Environment.NewLine);
            }

            else
            {
                // Get histogram data and pass to 'data' variable
                string path = System.AppDomain.CurrentDomain.BaseDirectory + "histogram\\" + "histogram.py" + " \"{0}\" ";
                data = cmd_histogram("C:\\Program Files\\Python39\\python.exe", string.Format(path, txt_path_histogram.Text));
                data = data.Substring(1, data.Length - 2);

                while (true)
                {
                    if (data != "")
                    {
                        // Show update about histogram
                        DateTime d = new DateTime();
                        d = DateTime.Now;
                        Status_Box.AppendText("[" + d.ToString("dd/MM/yyyy") + " - " + d.ToString("HH:mm:ss") + "]: " + "Histograms data created successfully.");
                        Status_Box.AppendText(Environment.NewLine);
                        break;
                    }
                }
            }
        }

        private void select_zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TransformGroup transformGroup = (TransformGroup)main_image.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            int zoom = Convert.ToInt32(select_zoom.Value);
            transform.ScaleX = zoom;
            transform.ScaleY = zoom;
        }
    }
    
}
