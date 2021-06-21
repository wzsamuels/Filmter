using DataStructures.Hashing;
using DataStructures.List;
using FilmterWPF.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace FilmterWPF
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : Window
    {
        private string path;
        private BackgroundWorker worker;

        public SinglyLinkedList<BasicMovie> MovieList { get; set; }
        public SeparateChainingHashMap<string, BasicMovie> MovieMap { get; set; }

        public ProgressBar(string path)
        {
            this.path = path;
            MovieMap = new();

            InitializeComponent();

            OkButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            worker = new();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += work_WorkerCompleted;

            worker.RunWorkerAsync();
        }

        private void work_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _ = MessageBox.Show(e.Error.Message);
            }

            CancelButton.IsEnabled = false;
            OkButton.IsEnabled = true;

            if (e.Cancelled)
            {
                DialogResult = false;
            }
            else
            {
                DialogResult = true;
                MovieList = (SinglyLinkedList<BasicMovie>)e.Result;
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        private int TotalLines()
        {
            using StreamReader r = new(path);
            int i = 0;
            while (r.ReadLine() != null) { i++; }
            return i;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = ReadBasicMovieFile(e);
        }

        public SinglyLinkedList<BasicMovie> ReadBasicMovieFile(DoWorkEventArgs e)
        {
            bool firstLine = true;
            SinglyLinkedList<BasicMovie> movieList = new();

            //int totalLines = TotalLines();
            int totalLines = 10000;
            int currentLine = 0;

            using (StreamReader reader = File.OpenText(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null && currentLine < totalLines)
                {
                    if (firstLine)
                    {
                        firstLine = false;
                    }
                    else
                    {
                        string[] values = line.Split('\t');
                        string id = values[0];
                        string title = values[2];

                        int? year;
                        if (Int32.TryParse(values[3], out int temp))
                        {
                            year = temp;
                        }
                        else
                        {
                            year = null;
                        }

                        int? runTimeMinutes;
                        if (int.TryParse(values[4], out temp))
                        {
                            runTimeMinutes = temp;
                        }
                        else
                        {
                            runTimeMinutes = null;
                        }

                        string genres = values[5];

                        BasicMovie record = new(id, title, year, runTimeMinutes, genres);

                        movieList.AddLast(record);
                        MovieMap.Put(record.Id, record);

                        currentLine++;
                        int percentComplete = (int)((float)currentLine / (float)totalLines * 100);
                        worker.ReportProgress(percentComplete);

                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;

                            return null;
                        }
                    }
                }
            }
            worker.ReportProgress(100);
            return movieList;
        }

        /*
        public SinglyLinkedList<Movie> ReadFullMovieFile(DoWorkEventArgs e)
        {
            SinglyLinkedList<Movie> movieList = new();
            MovieMap = new();
            bool firstLine = true;

            int totalLines = TotalLines();
            //int totalLines = 10000;
            int currentLine = 0;            

            using (StreamReader reader = File.OpenText(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null && currentLine < totalLines)
                {
                    if (firstLine)
                    {
                        firstLine = false;
                    }
                    else
                    {
                        string[] values = line.Split('\t');
                        string id = values[0];
                        string titleType = values[1];
                        string primaryTitle = values[2];
                        string originalTitle = values[3];
                        bool isAdult = (values[4] == "1");
                        int? startYear;
                        if (Int32.TryParse(values[5], out int temp))
                        {
                            startYear = temp;
                        }
                        else
                        {
                            startYear = null;
                        }

                        int? endYear;
                        if (int.TryParse(values[6], out temp))
                        {
                            endYear = temp;
                        }
                        else
                        {
                            endYear = null;
                        }

                        int? runTimeMinutes;
                        if (int.TryParse(values[7], out temp))
                        {
                            runTimeMinutes = temp;
                        }
                        else
                        {
                            runTimeMinutes = null;
                        }

                        string genres = values[8];

                        Movie record = new(id, titleType, primaryTitle, originalTitle, isAdult, startYear, endYear, runTimeMinutes, genres);

                        movieList.AddLast(record);
                        //MovieMap.Put(record.Id, record);
                        
                        currentLine++;
                        int percentComplete = (int)((float)currentLine / (float)totalLines * 100);
                        worker.ReportProgress(percentComplete);

                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;

                            return null;
                        }
                    }
                }
            }
            worker.ReportProgress(100);

            using (StreamWriter writer = new("newdata.txt"))
            {
                foreach (Movie m in movieList)
                {
                    if (m.TitleType == "movie")
                    {
                        string line = m.Id + "\t" + m.TitleType + "\t" + m.PrimaryTitle + "\t" + m.StartYear + "\t" +
                            m.RuntimeMinutes + "\t" + m.Genres;

                        writer.WriteLine(line);
                    }
                }
            }

            return movieList;
        }
        */

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            CancelButton.IsEnabled = false;
            worker.CancelAsync();
            LoadingText.Text = "Cancelling...";
        }
    }
}
