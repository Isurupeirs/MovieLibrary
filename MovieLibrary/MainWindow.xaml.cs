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

using Microsoft.Win32;
using MovieLibrary.Models;
using MovieLibrary.Services;

// MainWindow.xaml.cs
// This file controls what happens when each button is clicked
// It connects the UI to our MovieService backend



namespace MovieLibrary
{
    public partial class MainWindow : Window
    {
        // MovieService does all the backend work
        // search, sort, borrow, return all go through here
        private MovieService _service = new MovieService();

        // Constructor - runs when the window first opens
        public MainWindow()
        {
            InitializeComponent();

            // Load example movies so the grid is not empty
            LoadSampleData();

            // Show all movies in the grid
            RefreshGrid(_service.GetAllMovies());
        }

        // SAMPLE DATA 
        // 10 example movies loaded when the app starts
        private void LoadSampleData()
        {
            try
            {
                _service.AddMovie(new Movie("M1", "The Shawshank Redemption", "Frank Darabont", "Drama", 1994));
                _service.AddMovie(new Movie("M2", "Jurassic Park", "Steven Spielberg", "Sci-Fi", 1993));
                _service.AddMovie(new Movie("M3", "The Godfather", "Francis Ford Coppola", "Crime", 1972));
                _service.AddMovie(new Movie("M4", "Knives Out", "Rian Johnson", "Comedy", 2019));
                _service.AddMovie(new Movie("M5", "The Dark Knight", "Christopher Nolan", "Action", 2008));
                _service.AddMovie(new Movie("M6", "Avatar", "James Cameron", "Sci-Fi", 2009));
                _service.AddMovie(new Movie("M7", "Avengers Endgame", "Anthony and Joe Russo", "Sci-Fi", 2019));
                _service.AddMovie(new Movie("M8", "Captain America The Winter Soldier", "Anthony and Joe Russo", "Action", 2014));
                _service.AddMovie(new Movie("M9", "Iron Man", "Jon Favreau", "Action", 2008));
                _service.AddMovie(new Movie("M10", "Inception", "Christopher Nolan", "Sci-Fi", 2010));
            }
            catch
            {
                // ignore if something goes wrong loading sample data
            }
        }

        // HELPER: Refresh the DataGrid 
        // Clears the grid and reloads it with a new list of movies
        private void RefreshGrid(List<Movie> movies)
        {
            // Setting to null first forces the grid to fully redraw
            MovieDataGrid.ItemsSource = null;
            MovieDataGrid.ItemsSource = movies;

            // Update the movie count label
            TxtMovieCount.Text = "Movies: " + movies.Count;
        }

        // HELPER: Show a status message at the bottom 
        private void SetStatus(string message)
        {
            TxtStatus.Text = message;
        }

        // ADD MOVIE 
        // Runs when the Add Movie button is clicked
        // Week 2: creating a new Movie object using a constructor
        private void BtnAddMovie_Click(object sender, RoutedEventArgs e)
        {
            // Check all fields are filled in
            if (string.IsNullOrWhiteSpace(TxtMovieID.Text) ||
                string.IsNullOrWhiteSpace(TxtTitle.Text) ||
                string.IsNullOrWhiteSpace(TxtDirector.Text) ||
                string.IsNullOrWhiteSpace(TxtGenre.Text) ||
                string.IsNullOrWhiteSpace(TxtYear.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check the year is a valid number
            int year;
            bool yearOk = int.TryParse(TxtYear.Text, out year);

            if (yearOk == false || year < 1888 || year > 2100)
            {
                MessageBox.Show("Please enter a valid year (1888-2100).",
                    "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Create the new Movie object
                Movie newMovie = new Movie(
                    TxtMovieID.Text.Trim(),
                    TxtTitle.Text.Trim(),
                    TxtDirector.Text.Trim(),
                    TxtGenre.Text.Trim(),
                    year);

                // Add it to the service
                _service.AddMovie(newMovie);

                // Refresh the grid to show the new movie
                RefreshGrid(_service.GetAllMovies());

                SetStatus("'" + newMovie.Title + "' added successfully.");

                // Clear all input boxes
                TxtMovieID.Clear();
                TxtTitle.Clear();
                TxtDirector.Clear();
                TxtGenre.Clear();
                TxtYear.Clear();
            }
            catch (InvalidOperationException ex)
            {
                // This happens if the Movie ID already exists
                MessageBox.Show(ex.Message, "Duplicate ID",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // LINEAR SEARCH 
        // Week 4: checks every movie one by one to find a title match
        private void BtnLinearSearch_Click(object sender, RoutedEventArgs e)
        {
            string query = TxtSearchTitle.Text.Trim();

            if (string.IsNullOrWhiteSpace(query))
            {
                SetStatus("Please type a title to search.");
                return;
            }

            List<Movie> results = _service.LinearSearchByTitle(query);
            RefreshGrid(results);

            if (results.Count > 0)
            {
                SetStatus("Linear Search: found " + results.Count +
                          " result(s) for '" + query + "'.");
            }
            else
            {
                SetStatus("Linear Search: no results found for '" + query + "'.");
            }
        }

        // BINARY SEARCH 
        // Week 4: sorts by ID first then splits in half to find the movie
        private void BtnBinarySearch_Click(object sender, RoutedEventArgs e)
        {
            string query = TxtSearchID.Text.Trim();

            if (string.IsNullOrWhiteSpace(query))
            {
                SetStatus("Please type a Movie ID to search.");
                return;
            }

            Movie result = _service.BinarySearchByID(query);

            if (result != null)
            {
                // Found - show just this one movie
                List<Movie> singleMovie = new List<Movie>();
                singleMovie.Add(result);
                RefreshGrid(singleMovie);
                SetStatus("Binary Search: found '" + result.Title +
                          "' with ID '" + query + "'.");
            }
            else
            {
                // Not found - show empty grid
                RefreshGrid(new List<Movie>());
                SetStatus("Binary Search: no movie found with ID '" + query + "'.");
            }
        }

        // SHOW ALL
        // Resets the grid to show every movie
        private void BtnShowAll_Click(object sender, RoutedEventArgs e)
        {
            RefreshGrid(_service.GetAllMovies());
            SetStatus("Showing all movies.");
        }

        // BUBBLE SORT 
        // Week 2: compares two neighbours and swaps if out of order
        private void BtnBubbleSort_Click(object sender, RoutedEventArgs e)
        {
            List<Movie> sorted = _service.BubbleSortByTitle();
            RefreshGrid(sorted);
            SetStatus("Bubble Sort applied: movies sorted by Title A to Z.");
        }

        // MERGE SORT 
        // Week 2: splits list in half, sorts each half, merges back
        private void BtnMergeSort_Click(object sender, RoutedEventArgs e)
        {
            List<Movie> sorted = _service.MergeSortByYear();
            RefreshGrid(sorted);
            SetStatus("Merge Sort applied: movies sorted by Release Year.");
        }

        // BORROW
        // Week 3: marks movie as borrowed or adds user to waiting queue
        private void BtnBorrow_Click(object sender, RoutedEventArgs e)
        {
            string movieID = TxtBorrowID.Text.Trim();
            string username = TxtBorrowerName.Text.Trim();

            if (string.IsNullOrWhiteSpace(movieID) ||
                string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter both a Movie ID and your name.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string result = _service.BorrowMovie(movieID, username);
                MessageBox.Show(result, "Borrow Result",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                SetStatus(result);
                RefreshGrid(_service.GetAllMovies());
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //  RETURN 
        // Week 3: marks movie as available and assigns to next in queue
        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            string movieID = TxtReturnID.Text.Trim();

            if (string.IsNullOrWhiteSpace(movieID))
            {
                MessageBox.Show("Please enter a Movie ID to return.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string result = _service.ReturnMovie(movieID);
            MessageBox.Show(result, "Return Result",
                MessageBoxButton.OK, MessageBoxImage.Information);
            SetStatus(result);
            RefreshGrid(_service.GetAllMovies());
        }

        // CHECK QUEUE 
        // Week 3: shows the FIFO waiting list for a movie
        private void BtnCheckQueue_Click(object sender, RoutedEventArgs e)
        {
            string movieID = TxtQueueID.Text.Trim();

            if (string.IsNullOrWhiteSpace(movieID))
            {
                TxtQueueResult.Text = "Please enter a Movie ID.";
                return;
            }

            List<string> queue = _service.GetWaitingList(movieID);

            if (queue.Count == 0)
            {
                TxtQueueResult.Text = "No one is waiting.";
            }
            else
            {
                string waitingText = queue.Count + " waiting:\n";

                for (int i = 0; i < queue.Count; i++)
                {
                    waitingText = waitingText + "  " +
                                  (i + 1) + ". " + queue[i] + "\n";
                }

                TxtQueueResult.Text = waitingText;
            }
        }

        //  ROW CLICK 
        // When user clicks a row, auto fills Movie ID into all boxes
        private void MovieDataGrid_SelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            if (MovieDataGrid.SelectedItem is Movie selected)
            {
                TxtBorrowID.Text = selected.MovieID;
                TxtReturnID.Text = selected.MovieID;
                TxtQueueID.Text = selected.MovieID;
                SetStatus("Selected: " + selected.Title +
                          " (" + selected.MovieID + ")");
            }
        }

        // EXPORT JSON
        // Saves all movies to a JSON file
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json";
            dialog.FileName = "movies.json";

            bool? userClickedSave = dialog.ShowDialog();

            if (userClickedSave == true)
            {
                try
                {
                    _service.ExportToJson(dialog.FileName);
                    SetStatus("Exported successfully to " + dialog.FileName);
                    MessageBox.Show("Movies exported successfully!", "Export Done",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Export failed: " + ex.Message, "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // IMPORT JSON 
        // Loads movies from a JSON file
        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json";

            bool? userClickedOpen = dialog.ShowDialog();

            if (userClickedOpen == true)
            {
                try
                {
                    _service.ImportFromJson(dialog.FileName);
                    RefreshGrid(_service.GetAllMovies());
                    SetStatus("Imported successfully from " + dialog.FileName);
                    MessageBox.Show("Movies imported successfully!", "Import Done",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Import failed: " + ex.Message, "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}