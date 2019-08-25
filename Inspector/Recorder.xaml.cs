using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Inspector
{
    /// <summary>
    /// Recorder.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Recorder : Window
    {
        private RecorderController recorderController;
        public Recorder(ListBox recordList)
        {
            InitializeComponent();
            recorderController = new RecorderController(recordList);
            recorderController.Install();
        }

        private void RecordStart_Click(object sender, RoutedEventArgs e)
        {
            recorderController.Start();
        }

        private void RecordStop_Click(object sender, RoutedEventArgs e)
        {
            recorderController.Stop();
        }

        private void Try_Click(object sender, RoutedEventArgs e)
        {
            recorderController.StartRecorded();
        }

        private void RecordClear_Click(object sender, RoutedEventArgs e)
        {
            recorderController.Clear();

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = recorderController.ElementQueue.ToArray();

            using (StreamWriter outputFile = new StreamWriter(@"C:\Users\Chaemin Lim\Documents\New_Xml_File.xml"))
            {
                foreach (string line in lines)
                {
                    outputFile.WriteLine(line);
                }
            }

        }
    }
}
