using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmterWPF.Data
{
    public class Movie
    {
        public string Id { get; set; }
        public string TitleType { get; set; }
        public string PrimaryTitle { get; set; }
        public string OriginalTitle { get; set; }
        public bool IsAdult { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public int? RuntimeMinutes { get; set; }
        public string Genres { get; set; }

        public Movie(string id, string titleType, string primaryTitle, string originalTitle, bool isAdult, int? startYear, int? endYear, int? runtimeMinutes, string genres)
        {
            Id = id;
            TitleType = titleType;
            PrimaryTitle = primaryTitle;
            OriginalTitle = originalTitle;
            IsAdult = isAdult;
            StartYear = startYear;
            EndYear = endYear;
            RuntimeMinutes = runtimeMinutes;
            Genres = genres;
        }
    }

    public class BasicMovie : IComparable<BasicMovie>
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public int? RunTimeMinutes { get; set; }
        public string Genres { get; set; }

        public BasicMovie(string id, string title, int? year, int? runTimeMinutes, string genres)
        {
            Id = id;
            Title = title;
            Year = year;
            RunTimeMinutes = runTimeMinutes;
            Genres = genres;
        }

        private class SortYearAscendingHelper : IComparer<BasicMovie>
        {

            public int Compare(BasicMovie x, BasicMovie y)
            {
                BasicMovie movie1 = (BasicMovie)x;
                BasicMovie movie2 = (BasicMovie)y;

                if(movie1.Year < movie2.Year)
                {
                    return -1;
                }
                if(movie1.Year > movie2.Year)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static IComparer<BasicMovie> SortYearAscending()
        {
            return new SortYearAscendingHelper();
        }

        public int CompareTo(object obj)
        {
            BasicMovie other = (BasicMovie)obj;
            return string.Compare(this.Title, other.Title);
        }

        public int CompareTo(BasicMovie other)
        {
            return string.Compare(this.Title, other.Title);
        }
    }
}
