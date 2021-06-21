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

namespace FilmterWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SinglyLinkedList<Movie> movies;
        private string path = "../../../data.tsv";
        private IMap<string, Movie> movieMap;
        private SkipListMap<string, Movie> skipMap;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressBar pb = new(path);

            bool? result = pb.ShowDialog();
            
            if(result == true)
            {
                movieMap = pb.MovieMap;

                ObservableCollection<Movie> data = new(pb.MovieList);
                dataGrid.DataContext = data;


            }
            
        }
    }
}
