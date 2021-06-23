using FilmterWPF.Data;
using System;
using System.ComponentModel;
using System.Windows;
using DataStructures.Map;
using DataStructures.Sorter;
using DataStructures.Hashing;
using static FilmterWPF.MainWindow;
using System.Collections.ObjectModel;
using DataStructures.SearchTree;

namespace FilmterWPF
{
    /// <summary>
    /// Interaction logic for SortMovieWindow.xaml
    /// </summary>
    public partial class SortMovieWindow : Window
    {
        private BackgroundWorker worker;
        private SortInfo sortInfo;
        private FilterInfo filterInfo;
        public ObservableCollection<BasicMovie> MovieList;
        public IMap<string, BasicMovie> MovieMap { get; set; }
        private readonly ObservableCollection<BasicMovie> moviesToSort;
        private TimeSpan sortTimeSpan;
        private TimeSpan buildMapTimeSpan;
        public string SortTime { get; set; }

        public SortMovieWindow(SortInfo sortInfo, FilterInfo filterInfo, ObservableCollection<BasicMovie> moviesToSort)
        {
            InitializeComponent();

            OkButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
            this.sortInfo = sortInfo;
            this.filterInfo = filterInfo;
            this.moviesToSort = moviesToSort;           
        }

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
                Dispatcher.Invoke(new Action(() =>
                {
                    pbStatus.IsIndeterminate = false;
                    pbStatus.Value = 100;
                    LoadingText.Text = $"Building the map took: {buildMapTimeSpan.TotalSeconds:F5} seconds.\n" +
                    $"Sorting took: {sortTimeSpan.TotalSeconds:F5} seconds.\n";
                    
                }));

                MovieList = (ObservableCollection<BasicMovie>)e.Result;
                SortTime = sortTimeSpan.TotalSeconds.ToString();
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = SortAndFilter(e);
        }

        public ObservableCollection<BasicMovie> SortAndFilter(DoWorkEventArgs e)
        {
            AbstractComparisonSorter<BasicMovie> sorter = null;                    
            IMap<string, BasicMovie> movieMap = null;

            if (sortInfo.dataType == DataType.AVLTreeMap)
            {
                movieMap = new AVLTreeMap<string, BasicMovie>();
            }
            else if (sortInfo.dataType == DataType.BinaryTree)
            {
                movieMap = new BinarySearchTreeMap<string, BasicMovie>();
            }
            else if (sortInfo.dataType == DataType.LinearHash)
            {
                movieMap = new LinearProbingHashMap<string, BasicMovie>();
            }
            else if(sortInfo.dataType == DataType.RedBlackTree)
            {
                movieMap = new RedBlackTreeMap<string, BasicMovie>();
            }
            else if (sortInfo.dataType == DataType.SeparateChaining)
            {
                movieMap = new SeparateChainingHashMap<string, BasicMovie>();
            }
            else if (sortInfo.dataType == DataType.SearchTableMap)
            {
                movieMap = new SearchTableMap<string, BasicMovie>();
            }
            else if (sortInfo.dataType == DataType.SkipListMap)
            {
                movieMap = new SkipListMap<string, BasicMovie>();
            }
            else if (sortInfo.dataType == DataType.SplayTree)
            {
                movieMap = new SplayTreeMap<string, BasicMovie>();
            }
            else if (sortInfo.dataType == DataType.UnorderedArrayMap)
            {
                movieMap = new UnorderedArrayMap<string, BasicMovie>();
            }
            else if (sortInfo.dataType == DataType.UnorderedLinkedMap)
            {
                movieMap = new UnorderedLinkedMap<string, BasicMovie>();
            }

            // Check for sorting algorithm

            if (sortInfo.sortingAlgorithm == SortingAlgorithm.BubbleSort)
            {
                sorter = new BubbleSorter<BasicMovie>();
            }
            else if(sortInfo.sortingAlgorithm == SortingAlgorithm.MergeSort)
            {
                sorter = new MergeSorter<BasicMovie>();
            }
            else if (sortInfo.sortingAlgorithm == SortingAlgorithm.InsertionSort)
            {
                sorter = new InsertionSorter<BasicMovie>();
            }
            else if (sortInfo.sortingAlgorithm == SortingAlgorithm.QuickSort)
            {
                sorter = new QuickSorter<BasicMovie>();
            }
            else if (sortInfo.sortingAlgorithm == SortingAlgorithm.SelectionSort)
            {
                sorter = new SelectionSorter<BasicMovie>();
            }

            // Check for sort by
            if (sortInfo.sortBy == SortBy.Year)
            {
                if (sortInfo.ascending)
                {
                    sorter.SetComparator(BasicMovie.SortYearAscending());
                }
                else
                {
                    sorter.SetComparator(BasicMovie.SortYearDescending());
                }
            }
            else if(sortInfo.sortBy == SortBy.Title)
            {
                if(sortInfo.ascending)
                {
                    sorter.SetComparator(BasicMovie.SortTitleAscending());
                }
                else
                {
                    sorter.SetComparator(BasicMovie.SortTitleDescending());
                }
            }
            else if(sortInfo.sortBy == SortBy.RunTimeMinutes)
            {
                if(sortInfo.ascending)
                {
                    sorter.SetComparator(BasicMovie.SortRunTimeAscending());
                }
                else
                {
                    sorter.SetComparator(BasicMovie.SortRunTimeDescending());
                }
            }
            else
            {
                if(sortInfo.ascending)
                {
                    sorter.SetComparator(BasicMovie.SortGenresAscending());
                }
                else
                {
                    sorter.SetComparator(BasicMovie.SortGenresDescending());
                }
            }

            // Build map using data type
            Dispatcher.Invoke(new Action(() =>
            {
                LoadingText.Text = "Building map...";
            }));

            float currentCount = 0;
            float totalCount = moviesToSort.Count;
            DateTime beginMapTime = DateTime.Now;            

            // Check each movie entry for filter criteria 
            foreach (BasicMovie movie in moviesToSort)
            {
                bool titleMatch = true;
                bool yearMatch = true;
                bool runTimeMatch = true;
                bool genreMatch = true;
                if(!string.IsNullOrEmpty(filterInfo.title) && !filterInfo.title.Equals(movie.Title)) 
                {
                    titleMatch = false;
                }
                if (!string.IsNullOrEmpty(filterInfo.year) && !filterInfo.year.Equals(movie.Year.ToString()))
                {
                    yearMatch = false;
                }
                if (!string.IsNullOrEmpty(filterInfo.runTime) && !filterInfo.runTime.Equals(movie.RunTimeMinutes.ToString()))
                {
                    runTimeMatch = false;
                }
                if (!string.IsNullOrEmpty(filterInfo.genre) && !filterInfo.genre.Equals(movie.Genres))
                {
                    genreMatch = false;
                }

                if(titleMatch && yearMatch && runTimeMatch && genreMatch)
                {
                    _ = movieMap.Put(movie.Id, movie);
                }

                currentCount++;
                worker.ReportProgress((int)(currentCount / totalCount * 100));
            }
            DateTime endMapTime = DateTime.Now;
            buildMapTimeSpan = endMapTime - beginMapTime;

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
                pbStatus.Value = 0;
                pbStatus.IsIndeterminate = true;
                LoadingText.Text = "Sorting...";
            }));
       

            // Sort the movies 
            DateTime beginSortTime = DateTime.Now;
            sorter.Sort(movieArray);
            DateTime endSortTime = DateTime.Now;

            sortTimeSpan = endSortTime - beginSortTime;

            return new ObservableCollection<BasicMovie>(movieArray);
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}