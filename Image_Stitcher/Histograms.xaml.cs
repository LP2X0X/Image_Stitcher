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
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;
using System.IO;
using System.ComponentModel;

namespace Image_Stitcher
{
    /// <summary>
    /// Interaction logic for Histograms.xaml
    /// </summary>
    public partial class Histograms : Window
    {
        public Histograms()
        {
            InitializeComponent();
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "histogram\\grayscale_histogram.png"))
            {
                // Load image
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                // Add this for "another process" error
                image.CacheOption = BitmapCacheOption.OnLoad;
                // Add this to fix image won't change after first load
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "histogram\\grayscale_histogram.png", UriKind.Relative);
                image.EndInit();
                grayscale_hist.Source = image;
            }
        }
        void Histogram_Closing(object sender, CancelEventArgs e)
        {
            // Delete previous histogram
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            grayscale_hist.Source = null;
            File.Delete(System.AppDomain.CurrentDomain.BaseDirectory + "histogram\\grayscale_histogram.png");

        }
    }

}
