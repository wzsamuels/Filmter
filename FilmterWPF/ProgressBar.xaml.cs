using DataStructures.Hashing;
using DataStructures.List;
using FilmterWPF.Data;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace FilmterWPF
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : Window
    {
        private readonly string path;
        private BackgroundWorker worker;

        public SinglyLinkedList<BasicMovie> MovieList { get; set; }
        public SeparateChainingHashMap<string, BasicMovie> MovieMap { get; set; }

        public ProgressBar(string path)
        {
            this.path = path;
            MovieMap = new();
            MovieList = null;

            InitializeComponent();

            OkButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
        }

        /// <summary>
        /// As soon as the window's content is rendered, begin running the background worker task
        /// to read the movie data file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            worker = new();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Work_WorkerCompleted;

            worker.RunWorkerAsync();
        }

        private void Work_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                MovieList = (SinglyLinkedList<BasicMovie>)e.Result;
                DialogResult = true;
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// Helper method to determine the total number of entries in the movie data file.
        /// Used for the percentage complete on the progress bar.
        /// </summary>
        /// <returns>The number of entries in the data file.</returns>
        private int TotalEntriesHelper()
        {
            using StreamReader r = new(path);
            int i = 0;
            while (r.ReadLine() != null) { i++; }
            return i;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = ReadBasicMovieFile(e);
        }

        public SinglyLinkedList<BasicMovie> ReadBasicMovieFile(DoWorkEventArgs e)
        {
            bool firstLine = true;
            SinglyLinkedList<BasicMovie> movieList = new();

            int totalLines = TotalEntriesHelper();
            //int totalLines = 100000;
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
                            //year = null;
                            continue;
                        }

                        int? runTimeMinutes;
                        if (int.TryParse(values[4], out temp))
                        {
                            runTimeMinutes = temp;
                        }
                        else
                        {
                            //runTimeMinutes = null;
                            continue;
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

        /// <summary>
        /// Handled the OKButton being clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
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
