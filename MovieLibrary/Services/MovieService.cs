using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieLibrary.DataStructures;
using MovieLibrary.Models;
using System.IO;
using System.Text.Json;


// MovieService.cs
// This is the BRAIN of the application
// All features live here: Add, Search, Sort, Borrow, Return, Export, Import
// It uses all 3 data structures we built:
//   - MovieLinkedList  (stores all movies in order)
//   - MovieHashtable   (finds movies fast by ID)
//   - BorrowQueue      (manages waiting lists)



namespace MovieLibrary.Services
{
    public class MovieService
    {
        // THE 3 DATA STRUCTURES
        // These 3 work together to power the whole app

        // Linked List - stores all movies in order (Week 2)
        private MovieLinkedList movieList = new MovieLinkedList();

        // Hashtable - finds a movie by ID instantly (Week 3)
        private MovieHashtable movieTable = new MovieHashtable();

        // Queue - manages waiting lists for borrowed movies (Week 3)
        private BorrowQueue borrowQueue = new BorrowQueue();

        // ADD MOVIE 
        // Adds a movie to BOTH the linked list AND the hashtable
        // Both must stay in sync at all times
        public void AddMovie(Movie movie)
        {
            // Insert into hashtable first - throws error if ID is duplicate
            movieTable.Insert(movie);

            // Also add to linked list for ordered storage
            movieList.Add(movie);
        }

        // REMOVE MOVIE 
        // Removes a movie from all 3 data structures
        public bool RemoveMovie(string movieID)
        {
            movieTable.Remove(movieID);
            borrowQueue.ClearQueue(movieID);
            return movieList.Remove(movieID);
        }

        // GET ALL MOVIES 
        // Returns all movies as a plain List for the UI to display
        public List<Movie> GetAllMovies()
        {
            return movieList.ToList();
        }

        //  FIND BY ID
        // Uses the hashtable for instant lookup
        public Movie FindByID(string movieID)
        {
            return movieTable.GetByID(movieID);
        }

        // LINEAR SEARCH 
        // Week 4: checks every single movie one by one
        // Works on unsorted lists
        // Slow for large lists but simple and always works
        public List<Movie> LinearSearchByTitle(string title)
        {
            List<Movie> results = new List<Movie>();
            List<Movie> allMovies = movieList.ToList();

            // Go through EVERY movie and check if the title matches
            for (int i = 0; i < allMovies.Count; i++)
            {
                // IndexOf returns -1 if not found, >= 0 if found
                // OrdinalIgnoreCase means capitals don't matter
                // so "inception" matches "Inception"
                bool titleMatches = allMovies[i].Title.IndexOf(
                    title, StringComparison.OrdinalIgnoreCase) >= 0;

                if (titleMatches)
                {
                    results.Add(allMovies[i]);
                }
            }

            return results;
        }

        //  BINARY SEARCH 
        // Week 4: much faster than linear search for large lists
        // BUT the list MUST be sorted first
        // Works by splitting the list in half each time
        public Movie BinarySearchByID(string movieID)
        {
            // Step 1: sort the list by ID so binary search works
            List<Movie> sorted = movieList.ToList()
                .OrderBy(m => m.MovieID).ToList();

            int left = 0;
            int right = sorted.Count - 1;

            // Step 2: keep splitting in half until found or not found
            while (left <= right)
            {
                // Find the middle position
                int mid = (left + right) / 2;

                int comparison = string.Compare(
                    sorted[mid].MovieID,
                    movieID,
                    StringComparison.OrdinalIgnoreCase);

                if (comparison == 0)
                {
                    return sorted[mid]; // FOUND
                }
                else if (comparison < 0)
                {
                    left = mid + 1;  // search RIGHT half
                }
                else
                {
                    right = mid - 1; // search LEFT half
                }
            }

            return null; // not found
        }

        // BUBBLE SORT
        // Week 2: compare two neighbours, swap if out of order
        // Repeat until no more swaps needed
        // Simple but slow for very large lists
        public List<Movie> BubbleSortByTitle()
        {
            Movie[] arr = movieList.ToList().ToArray();
            int n = arr.Length;

            // Outer loop: how many passes do we need
            for (int i = 0; i < n - 1; i++)
            {
                // Inner loop: compare each pair of neighbours
                for (int j = 0; j < n - i - 1; j++)
                {
                    // If left title is AFTER right title alphabetically
                    // then swap them
                    bool outOfOrder = string.Compare(
                        arr[j].Title,
                        arr[j + 1].Title,
                        StringComparison.OrdinalIgnoreCase) > 0;

                    if (outOfOrder)
                    {
                        // Swap using a temporary variable
                        Movie temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
            }

            // Convert sorted array back to a List
            List<Movie> sorted = new List<Movie>(arr);

            // Rebuild the linked list in sorted order
            movieList.RebuildFrom(sorted);

            return sorted;
        }

        // MERGE SORT
        // Week 2: Divide and Conquer
        // Split the list in half, sort each half, merge back together
        // Much faster than Bubble Sort for large lists
        public List<Movie> MergeSortByYear()
        {
            List<Movie> sorted = MergeSort(movieList.ToList());
            movieList.RebuildFrom(sorted);
            return sorted;
        }

        // Splits the list and sorts each half (calls itself recursively)
        private List<Movie> MergeSort(List<Movie> list)
        {
            // Base case: a list of 1 item is already sorted
            if (list.Count <= 1)
            {
                return list;
            }

            // Split into two halves
            int mid = list.Count / 2;
            List<Movie> left = MergeSort(list.GetRange(0, mid));
            List<Movie> right = MergeSort(list.GetRange(mid, list.Count - mid));

            // Merge the two sorted halves back together
            return Merge(left, right);
        }

        // Combines two sorted lists into one sorted list
        private List<Movie> Merge(List<Movie> left, List<Movie> right)
        {
            List<Movie> result = new List<Movie>();
            int i = 0; // pointer for left list
            int j = 0; // pointer for right list

            // Pick the smaller year from the front of each list
            while (i < left.Count && j < right.Count)
            {
                if (left[i].ReleaseYear <= right[j].ReleaseYear)
                {
                    result.Add(left[i]);
                    i++;
                }
                else
                {
                    result.Add(right[j]);
                    j++;
                }
            }

            // Add any leftover items from the left list
            while (i < left.Count)
            {
                result.Add(left[i]);
                i++;
            }

            // Add any leftover items from the right list
            while (j < right.Count)
            {
                result.Add(right[j]);
                j++;
            }

            return result;
        }

        // BORROW MOVIE
        // Week 3: If movie is available mark it as Borrowed
        //         If not available add user to the waiting Queue (FIFO)
        public string BorrowMovie(string movieID, string username)
        {
            // Validate username is not empty
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Please enter your name.");
            }

            // Find the movie using our hashtable (instant lookup)
            Movie movie = movieTable.GetByID(movieID);

            if (movie == null)
            {
                return "Movie not found. Please check the Movie ID.";
            }

            if (movie.IsAvailable == true)
            {
                // Movie is free - give it to this person
                movie.IsAvailable = false;
                return "'" + movie.Title + "' has been borrowed by " + username + ".";
            }
            else
            {
                // Movie is taken - add user to waiting queue (FIFO)
                borrowQueue.Enqueue(movieID, username);
                int position = borrowQueue.GetQueueCount(movieID);
                return "'" + movie.Title + "' is not available. "
                     + username + " added to waiting list at position "
                     + position + ".";
            }
        }

        // RETURN MOVIE 
        // Week 3: Mark movie as available
        //         If someone is waiting in queue give it to them
        public string ReturnMovie(string movieID)
        {
            Movie movie = movieTable.GetByID(movieID);

            if (movie == null)
            {
                return "Movie not found. Please check the Movie ID.";
            }

            if (movie.IsAvailable == true)
            {
                return "'" + movie.Title + "' was not borrowed.";
            }

            // Mark as returned
            movie.IsAvailable = true;

            // Check if anyone is waiting in the queue
            if (borrowQueue.HasWaiting(movieID))
            {
                // Give to next person in queue (FIFO - first in first out)
                string nextPerson = borrowQueue.Dequeue(movieID);
                movie.IsAvailable = false;
                return "'" + movie.Title + "' returned and automatically given to "
                     + nextPerson + ".";
            }

            return "'" + movie.Title + "' has been returned and is now available.";
        }

        // QUEUE HELPERS 
        public int GetQueueCount(string movieID)
        {
            return borrowQueue.GetQueueCount(movieID);
        }

        public List<string> GetWaitingList(string movieID)
        {
            return borrowQueue.GetWaitingList(movieID);
        }

        // EXPORT TO JSON 
        // Saves all movies to a .json file on the computer
        public void ExportToJson(string filePath)
        {
            List<Movie> allMovies = movieList.ToList();

            // Convert the list to JSON text format
            string json = JsonSerializer.Serialize(
                allMovies,
                new JsonSerializerOptions { WriteIndented = true });

            // Save the JSON text to the file
            File.WriteAllText(filePath, json);
        }

        // IMPORT FROM JSON 
        // Loads movies from a .json file back into the app
        public void ImportFromJson(string filePath)
        {
            // Read the JSON text from the file
            string json = File.ReadAllText(filePath);

            // Convert JSON text back into a List of Movie objects
            List<Movie> movies = JsonSerializer.Deserialize<List<Movie>>(json);

            if (movies == null)
            {
                throw new InvalidDataException("Could not read the file.");
            }

            // Clear existing data and reload from file
            movieList = new MovieLinkedList();
            movieTable = new MovieHashtable();

            foreach (Movie movie in movies)
            {
                AddMovie(movie);
            }
        }
    }
}