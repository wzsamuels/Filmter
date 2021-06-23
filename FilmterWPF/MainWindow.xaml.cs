using FilmterWPF.Data;
using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FilmterWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>     

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Private fields
        private readonly string path = "../../../basicdata.tsv";
        private string _movieEntryCount = "0";
        private string _movieDisplayCount = "0";
        private string _lastSortTime = "0";
        #endregion

        #region Properties
        public ObservableCollection<BasicMovie> MovieCollection { get; set; }
        
        public string MovieEntryCount
        {
            get => _movieEntryCount;
            set
            {
                _movieEntryCount = value;
                OnPropertyChanged("MovieEntryCount");
            }
        }

        public string MovieDisplayCount
        {
            get => _movieDisplayCount;
            set
            {
                _movieDisplayCount = value;
                OnPropertyChanged("MovieDisplayCount");
            }
        }

        public string LastSortTime
        {
            get => _lastSortTime;
            set
            {
                _lastSortTime = value;
                OnPropertyChanged("LastSortTime");
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this; //So variables can bind to UI
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string strPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(strPropertyName));
        }

        /// <summary>
        /// After the window's content is loaded, populate the data grid with movie entries.     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMovieWindow pb = new(path);

            bool? result = pb.ShowDialog();

            if (pb.MovieList != null)
            {

                MovieCollection = new(pb.MovieList);
                dataGrid.DataContext = MovieCollection;
                MovieEntryCount = MovieCollection.Count.ToString();
                MovieDisplayCount = MovieCollection.Count.ToString();
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

            if (avlTreeRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.AVLTreeMap;
            }
            else if (binaryTreeRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.BinaryTree;
            }
            else if (linearHashRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.LinearHash;
            }
            else if (redBlackRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.RedBlackTree;
            }
            else if(searchTableRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.SearchTableMap;
            }
            else if(separateChainingRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.SeparateChaining;
            }
            else if (skipListRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.SkipListMap;
            }
            else if (splayTreeRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.SplayTree;
            }
            else if(unorderedArrayRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.UnorderedArrayMap;
            }
            else if (unorderedLinkedRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.UnorderedLinkedMap;
            }

            // Checking sorting algorithm
            if (bubbleRadio.IsChecked == true)
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

            SortMovieWindow window = new(sortInfo, filterInfo, MovieCollection);
            if (window.ShowDialog() == true)
            {
                dataGrid.DataContext = window.MovieList;
                MovieDisplayCount = window.MovieList.Count.ToString();
                LastSortTime = window.SortTime;
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

        /// <summary>
        /// SortBy 
        /// </summary>
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
            BinaryTree,
            LinearHash,
            RedBlackTree,
            SearchTableMap,
            SeparateChaining,
            SkipListMap,
            SplayTree,
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

        private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit?", "Exit",
                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Close();
            }
        }
    }
}