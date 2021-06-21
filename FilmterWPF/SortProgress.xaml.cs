using FilmterWPF.Data;
using System;
using System.ComponentModel;
using System.Windows;
using DataStructures.Map;
using DataStructures.Sorter;
using DataStructures.Hashing;
using static FilmterWPF.MainWindow;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace FilmterWPF
{
    /// <summary>
    /// Interaction logic for SortProgress.xaml
    /// </summary>
    public partial class SortProgress : Window
    {
        private BackgroundWorker worker;
        private SortInfo sortInfo;
        public ObservableCollection<BasicMovie> MovieList;
        public IMap<string, BasicMovie> MovieMap { get; set; }
        private ObservableCollection<BasicMovie> movieCollection;

        public SortProgress(SortInfo sortInfo, ObservableCollection<BasicMovie> movieCollection)
        {
            InitializeComponent();
            OkButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
            this.sortInfo = sortInfo;
            this.movieCollection = movieCollection;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            worker = new();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
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
                MovieList = (ObservableCollection<BasicMovie>)e.Result;
            }
        }     

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = SortAndFilter(e);
        }

        public ObservableCollection<BasicMovie> SortAndFilter(DoWorkEventArgs e)
        {
            //int totalSteps = (int)((float)currentLine / (float)totalLines * 100);

            AbstractComparisonSorter<BasicMovie> sorter;
            
            IMap<string, BasicMovie> movieMap = null;


            // Build map using data type
            Dispatcher.Invoke(new Action(() =>
            {
                LoadingText.Text = "Building map...";
            }));

            if (sortInfo.dataType == DataType.LinearHash)
            {
                movieMap = new LinearProbingHashMap<string, BasicMovie>();
            }

            foreach (BasicMovie movie in movieCollection)
            {
                _ = movieMap.Put(movie.Id, movie);
            }


            // Check for sorting algorithm
            if (sortInfo.sortingAlgorithm == SortingAlgorithm.BubbleSort)
            {
                sorter = new BubbleSorter<BasicMovie>();

            }
            else
            {
                sorter = null;
            }

            // Check for filtering

            // Check for sort by
            if (sortInfo.sortBy == SortBy.Year)
            {
                if (sortInfo.ascending)
                {

                    sorter.SetComparator(BasicMovie.SortYearAscending());
                }
                else
                {

                }
            }

            Dispatcher.Invoke(new Action(() =>
            {
                LoadingText.Text = "Breaking down map...";
            }));

            BasicMovie[] movieArray = new BasicMovie[movieMap.Size()];

            int index = 0;
            foreach (BasicMovie movie in movieMap.Values())
            {
                movieArray[index] = movie;
                index++;
            }

            Dispatcher.Invoke(new Action(() =>
            {
                LoadingText.Text = "Sorting...";
            }));

            sorter.Sort(movieArray);

            return new ObservableCollection<BasicMovie>(movieArray);
        }

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