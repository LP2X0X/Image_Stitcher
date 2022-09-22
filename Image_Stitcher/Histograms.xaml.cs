﻿using System;
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
using System.IO;
using System.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Image_Stitcher
{
    /// <summary>
    /// Interaction logic for Histograms.xaml
    /// </summary>
    public partial class Histograms : Window
    {
        public string hist_data;
        public int min_hist { get; set; }
        public int max_hist { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        public Histograms()
        {
            InitializeComponent();
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "HISTOGRAM",
                    Values = new ChartValues<int> {}
                }
            };

            Labels = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50",
                "51", "52", "53", "54", "55", "56", "57", "58", "59", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "100",
                "101", "102", "103", "104", "105", "106", "107", "108", "109", "110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139", "140", "141", "142", "143", "144", "145", "146", "147", "148", "149", "150",
                "151", "152", "153", "154", "155", "156", "157", "158", "159", "160", "161", "162", "163", "164", "165", "166", "167", "168", "169", "170", "171", "172", "173", "174", "175", "176", "177", "178", "179", "180", "181", "182", "183", "184", "185", "186", "187", "188", "189", "190", "191", "192", "193", "194", "195", "196", "197", "198", "199", "200",
                "201", "202", "203", "204", "205", "206", "207", "208", "209", "210", "211", "212", "213", "214", "215", "216", "217", "218", "219", "220", "221", "222", "223", "224", "225", "226", "227", "228", "229", "230", "231", "232", "233", "234", "235", "236", "237", "238", "239", "240", "241", "242", "243", "244", "245", "246", "247", "248", "249", "250", "251", "252", "253", "254", "255"};

            // Get histogram data from MainWindow
            hist_data = ((MainWindow)Application.Current.MainWindow).data;
            string[] values = hist_data.Split(',');

            // Convert value from string to int and add to chart
            foreach(var x in values)
            {
                int y = Convert.ToInt32(x);
                SeriesCollection[0].Values.Add(y);
            }

            min_hist = ((MainWindow)Application.Current.MainWindow).min_h;
            max_hist = ((MainWindow)Application.Current.MainWindow).max_h;

            Formatter = value => value.ToString("N");
            DataContext = this;
        }
        void Histogram_Closing(object sender, CancelEventArgs e)
        {
            //// Delete previous histogram
            //System.GC.Collect();
            //System.GC.WaitForPendingFinalizers();
            //grayscale_hist.Source = null;
            //File.Delete(System.AppDomain.CurrentDomain.BaseDirectory + "histogram\\grayscale_histogram.png");

        }

        private void Axis_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }

}
