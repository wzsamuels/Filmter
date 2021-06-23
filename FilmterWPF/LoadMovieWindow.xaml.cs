using DataStructures.Hashing;
using DataStructures.List;
using FilmterWPF.Data;
using FilmterWPF.IO;
using System;
using System.ComponentModel;
using System.Windows;

namespace FilmterWPF
{
    /// <summary>
    /// Interaction logic for LoadMovieWindow.xaml
    /// </summary>
    public partial class LoadMovieWindow : Window
    {
        private readonly string path;
        private BackgroundWorker worker;

        public SinglyLinkedList<BasicMovie> MovieList { get; set; }
        public SeparateChainingHashMap<string, BasicMovie> MovieMap { get; set; }
        public int EntryCount { get; set; }
        public int EntryTotal { get; set; }

        public LoadMovieWindow(string path)
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
            entryCountText.Text = e.UserState.ToString();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = FileReader.ReadBasicMovieFile(path, e, worker);
        }

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
