using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieLibrary.Models;

// MovieHashtable.cs
// A Hashtable lets us find a movie by ID very quickly
// Instead of checking every movie one by one, it jumps straight to it
// We use Dictionary which is C#'s built-in Hashtable
// Week 3 lecture: Hash Tables



namespace MovieLibrary.DataStructures
{
    public class MovieHashtable
    {
        // Dictionary stores KEY and VALUE pairs
        // KEY   = MovieID  (string)  e.g. "M001"
        // VALUE = Movie object
        // When we give it a key, it finds the value instantly
        private Dictionary<string, Movie> table = new Dictionary<string, Movie>();

        //INSERT 
        // Adds a movie to the hashtable
        // Throws an error if the Movie ID already exists (no duplicates)
        public void Insert(Movie movie)
        {
            // Check if this ID already exists
            if (table.ContainsKey(movie.MovieID))
            {
                throw new InvalidOperationException(
                    "Movie ID '" + movie.MovieID + "' already exists. Please use a different ID.");
            }

            // Store the movie using its ID as the key
            table[movie.MovieID] = movie;
        }

        //  GETBYID 
        // Finds and returns a movie by its ID
        // Returns null if the movie is not found
        public Movie GetByID(string movieID)
        {
            if (table.ContainsKey(movieID))
            {
                return table[movieID]; // found it - return the movie
            }
            else
            {
                return null; // not found
            }
        }

        // REMOVE 
        // Removes a movie from the hashtable by its ID
        // Returns true if removed, false if not found
        public bool Remove(string movieID)
        {
            return table.Remove(movieID);
        }

        // CONTAINS 
        // Checks if a Movie ID already exists in the table
        // Returns true or false
        public bool Contains(string movieID)
        {
            return table.ContainsKey(movieID);
        }

        // COUNT 
        // How many movies are stored in the hashtable
        public int Count
        {
            get { return table.Count; }
        }
    }
}
