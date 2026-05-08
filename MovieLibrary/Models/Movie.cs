using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Movie.cs
// This class represents ONE movie in our library
// It is like a form with 6 fields for each movie
// Week 2 lecture: Classes, Properties, Constructors

namespace MovieLibrary.Models
{
    public class Movie
    {
        //  PROPERTIES 
        // These are the 6 pieces of information we store for each movie

        public string MovieID { get; set; }       // e.g. "M001"
        public string Title { get; set; }         // e.g. "Inception"
        public string Director { get; set; }      // e.g. "Christopher Nolan"
        public string Genre { get; set; }         // e.g. "Sci-Fi"
        public int ReleaseYear { get; set; }      // e.g. 2010
        public bool IsAvailable { get; set; }     // true = available, false = borrowed

        // CALCULATED PROPERTY 
        // This property does not store a value
        // It automatically works out the text based on IsAvailable
        public string AvailabilityStatus
        {
            get
            {
                if (IsAvailable == true)
                {
                    return "Available";
                }
                else
                {
                    return "Borrowed";
                }
            }
        }

        // CONSTRUCTOR 
        // A constructor is a special method that runs when we CREATE a movie
        // Example of creating a movie:
        // Movie m = new Movie("M001", "Inception", "Nolan", "Sci-Fi", 2010);
        public Movie(string movieID, string title, string director,
                     string genre, int releaseYear)
        {
            MovieID = movieID;
            Title = title;
            Director = director;
            Genre = genre;
            ReleaseYear = releaseYear;
            IsAvailable = true;  // every new movie starts as Available
        }
    }
}
