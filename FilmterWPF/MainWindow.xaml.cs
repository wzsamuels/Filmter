using FilmterWPF.Data;
using FilmterWPF.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataStructures.List;
using DataStructures.Hashing;
using DataStructures.Map;
using System.Collections.ObjectModel;
using DataStructures.Sorter;

namespace FilmterWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>     

    public partial class MainWindow : Window
    {
        public ObservableCollection<BasicMovie> MovieCollection { get; set; }
        private string path = "../../../basicdata.tsv";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressBar pb = new(path);

            bool? result = pb.ShowDialog();

            if (result == true)
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
            SortInfo sortInfo;
            sortInfo.ascending = (bool)ascendingRadio.IsChecked;

            if(linearHashRadio.IsChecked == true)
            {
                sortInfo.dataType = DataType.LinearHash;
            }
            else
            {
                sortInfo.dataType = 0;
            }

            if(bubbleRadio.IsChecked == true)
            {
                sortInfo.sortingAlgorithm = SortingAlgorithm.BubbleSort;
            }
            else 
            {
                sortInfo.sortingAlgorithm = 0;
            }

            if(yearRadio.IsChecked == true)
            {
                sortInfo.sortBy = SortBy.Year;
            }
            else
            {
                sortInfo.sortBy = 0;
            }

            SortProgress window = new(sortInfo, MovieCollection);

            bool? result = window.ShowDialog();

            if(result == true)
            {
                
                    dataGrid.DataContext = window.MovieList;
            }
            else 
            {
                dataGrid.DataContext = null;
            }
            
        }

        public struct SortInfo
        {
            public SortBy sortBy;
            public SortingAlgorithm sortingAlgorithm;
            public bool ascending;
            public DataType dataType;
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
            LinearHash
        }

        public enum SortingAlgorithm
        {
            BubbleSort
        }
    }
}
