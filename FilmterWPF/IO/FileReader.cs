using FilmterWPF.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures.List;
using System.Windows;

namespace FilmterWPF.IO
{
    public static class FileReader
    {
        private static int TotalLines(string filePath)
        {
            using (StreamReader r = new(filePath))
            {
                int i = 0;
                while (r.ReadLine() != null) { i++; }
                return i;
            }
        }

        public static SinglyLinkedList<Movie> ReadMovieFile(string path)
        {
            SinglyLinkedList<Movie> movieList = new();
            bool firstLine = true;

            using (StreamReader reader = File.OpenText(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
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
                        if(Int32.TryParse(values[5], out int temp))
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
                    }
                }
            }

            return movieList;
        }
    }
}
