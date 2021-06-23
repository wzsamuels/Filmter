using FilmterWPF.Data;
using System;
using System.IO;
using DataStructures.List;
using System.ComponentModel;

namespace FilmterWPF.IO
{
    public static class FileReader
    {
        /// <summary>
        /// Helper method to determine the total number of entries in the movie data file.
        /// Used for the percentage complete on the progress bar.
        /// </summary>
        /// <returns>The number of entries in the data file.</returns>
        private static int TotalEntriesHelper(string filePath)
        {
            using StreamReader r = new(filePath);
            int i = 0;
            while (r.ReadLine() != null) { i++; }
            return i;
        }

        public static SinglyLinkedList<BasicMovie> ReadBasicMovieFile(string path, DoWorkEventArgs e, BackgroundWorker worker)
        {
            bool firstLine = true;
            SinglyLinkedList<BasicMovie> movieList = new();

            int totalLines = TotalEntriesHelper(path);
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

                        // Convert string year to int. Skip over entries with no value.
                        int? year;
                        if (Int32.TryParse(values[3], out int temp))
                        {
                            year = temp;
                        }
                        else
                        {
                            year = null;
                            //continue;
                        }

                        // Convert string run time minutes to int. Skip over entries with no value.
                        int? runTimeMinutes;
                        if (int.TryParse(values[4], out temp))
                        {
                            runTimeMinutes = temp;
                        }
                        else
                        {
                            runTimeMinutes = null;
                            //continue;
                        }

                        string genres = values[5];

                        BasicMovie record = new(id, title, year, runTimeMinutes, genres);

                        movieList.AddLast(record);

                        currentLine++;
                        int percentComplete = (int)((float)currentLine / (float)totalLines * 100);
                        worker.ReportProgress(percentComplete, $"Movies loaded: {currentLine} / {totalLines}");

                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;

                            return null;
                        }
                    }
                }
            }
            worker.ReportProgress(100, $"Movies loaded: {totalLines} / {totalLines}");
            return movieList;
        }

        public static SinglyLinkedList<Movie> ReadFullMovieFile(string path, DoWorkEventArgs e, BackgroundWorker worker)
        {
            SinglyLinkedList<Movie> movieList = new();
            bool firstLine = true;

            int totalLines = TotalEntriesHelper(path);
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
    }
}