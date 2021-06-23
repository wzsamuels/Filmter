using FilmterWPF.Data;
using System;
using System.Windows;
using System.Collections.ObjectModel;

namespace FilmterWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>     

    public partial class MainWindow : Window
    {
        public ObservableCollection<BasicMovie> MovieCollection { get; set; }
        private readonly string path = "../../../basicdata.tsv";

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// After the window's content is loaded, populate the data grid with movie entries.     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressBar pb = new(path);

            bool? result = pb.ShowDialog();

            if (pb.MovieList != null)
            {

                MovieCollection = new(pb.MovieList);
                dataGrid.DataContext = MovieCollection;
            }
            else
            {
                _ = MessageBox.Show("Error loading movies.");
            }

        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            FilterInfo filterInfo = new("", "", "", "");

            if(!String.IsNullOrEmpty(titleTextBox.Text))
            {
                filterInfo.title = titleTextBox.Text;
            }
            if (!String.IsNullOrEmpty(yearTextBox.Text))
            {
                filterInfo.year = yearTextBox.Text;
            }
            if (!String.IsNullOrEmpty(runTimeTextBox.Text))
            {
                filterInfo.runTime = runTimeTextBox.Text;
            }
            if (!String.IsNullOrEmpty(genreTextBox.Text))
            {
                filterInfo.genre = genreTextBox.Text;
            }

            // Initialize sortInfo with some default values
            SortInfo sortInfo = new(SortBy.Title, SortingAlgorithm.MergeSort, true, DataType.LinearHash)
            {
                ascending = (bool)ascendingRadio.IsChecked
            };

            if (linearHashRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.LinearHash;
            }
            else if(searchTableRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.SearchTableMap;
            }
            else if(separateChainingRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.SeparateChaining;
            }
            else if(unorderedArrayRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.UnorderedArrayMap;
            }

            // Checking sorting algorithm
            if(bubbleRadio.IsChecked == true)
            {
                sortInfo.sortingAlgorithm = SortingAlgorithm.BubbleSort;
            }
            else if(mergeRadio.IsChecked == true)
            {
                sortInfo.sortingAlgorithm = SortingAlgorithm.MergeSort;
            }
            else if(insertionRadio.IsChecked == true)
            {
                sortInfo.sortingAlgorithm = SortingAlgorithm.InsertionSort;
            }
            else if(quickRadio.IsChecked == true)
            {
                sortInfo.sortingAlgorithm = SortingAlgorithm.QuickSort;
            }
            else if(selectionRadio.IsChecked == true)
            {
                sortInfo.sortingAlgorithm = SortingAlgorithm.SelectionSort;
            }

            // Check sort by
            if(yearRadio.IsChecked == true)
            {
                sortInfo.sortBy = SortBy.Year;
            }
            else if(titleRadio.IsChecked == true)
            {
                sortInfo.sortBy = SortBy.Title;
            }
            else if(runTimeRadio.IsChecked == true)
            {
                sortInfo.sortBy = SortBy.RunTimeMinutes;
            }
            else
            {
                sortInfo.sortBy = SortBy.Genres;
            }

            SortProgress window = new(sortInfo, filterInfo, MovieCollection);
            if (window.ShowDialog() == true)
            {
                    dataGrid.DataContext = window.MovieList;
            }
        }

        public struct FilterInfo
        {
            public string title;
            public string year;
            public string runTime;
            public string genre;

            public FilterInfo(string title, string year, string runTime, string genre)
            {
                this.title = title;
                this.year = year;
                this.runTime = runTime;
                this.genre = genre;
            }
        }

        public struct SortInfo
        {
            public SortBy sortBy;
            public SortingAlgorithm sortingAlgorithm;
            public bool ascending;
            public DataType dataType;

            public SortInfo(SortBy sortBy, SortingAlgorithm sortingAlgorithm, bool ascending, DataType dataType)
            {
                this.sortBy = sortBy;
                this.sortingAlgorithm = sortingAlgorithm;
                this.ascending = ascending;
                this.dataType = dataType;
            }
        }

        public enum SortBy
        {
            Year,
            Title,
            RunTimeMinutes,
            Genres
        }

        public enum DataType
        {
            AVLTreeMap,
            LinearHash,
            SearchTableMap,
            SeparateChaining,
            SkipListMap,
            RedBlackTreeMap,
            UnorderedArrayMap,
            UnorderedLinkedMap
        }

        public enum SortingAlgorithm
        {
            BubbleSort,
            InsertionSort,
            MergeSort,
            QuickSort,
            SelectionSort
        }
    }
}