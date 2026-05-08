using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieLibrary.Models;
using MovieLibrary.Services;
using Xunit;



// MovieManagementTests.cs
// Tests for adding and removing movies
// Testing lecture: [Fact] tests, Assert, boundary testing



namespace MovieLibrary.Tests
{
    public class MovieManagementTests
    {
        // Helper - creates a fresh service for each test
        private MovieService CreateService()
        {
            return new MovieService();
        }

        // Helper - creates a simple test movie
        private Movie SampleMovie(string id = "M1", string title = "Fast and Furious 7")
        {
            return new Movie(id, title, "Nolan", "Action", 2010);
        }

        // TEST 1 - Adding a valid movie should store it correctly
        [Fact]
        public void AddMovie_ValidMovie_IsStoredAndRetrievable()
        {
            // Arrange
            MovieService service = CreateService();
            Movie movie = SampleMovie();

            // Act
            service.AddMovie(movie);
            List<Movie> all = service.GetAllMovies();

            // Assert
            Assert.Single(all);
            Assert.Equal("M1", all[0].MovieID);
            Assert.Equal("Fast and Furious 7", all[0].Title);
        }

        // TEST 2 - Duplicate ID should throw an error
        [Fact]
        public void AddMovie_DuplicateID_ThrowsInvalidOperationException()
        {
            // Arrange
            MovieService service = CreateService();
            service.AddMovie(SampleMovie("M1", "Fast and Furious 7"));

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() =>
                service.AddMovie(SampleMovie("M1", "Interstellar")));
        }

        // TEST 3 - Adding multiple movies should store all of them
        [Fact]
        public void AddMovie_MultipleMovies_AllStored()
        {
            // Arrange
            MovieService service = CreateService();

            // Act
            service.AddMovie(SampleMovie("M1", "Fast and Furious 7"));
            service.AddMovie(SampleMovie("M2", "Jurassic World"));
            service.AddMovie(SampleMovie("M3", "Interstellar"));

            // Assert
            Assert.Equal(3, service.GetAllMovies().Count);
        }

        // TEST 4 - New movie should always start as Available
        [Fact]
        public void AddMovie_NewMovie_DefaultsToAvailable()
        {
            // Arrange
            MovieService service = CreateService();

            // Act
            service.AddMovie(SampleMovie());
            Movie movie = service.FindByID("M1");

            // Assert
            Assert.NotNull(movie);
            Assert.True(movie.IsAvailable);
            Assert.Equal("Available", movie.AvailabilityStatus);
        }

        // TEST 5 - Removing an existing movie should work
        [Fact]
        public void RemoveMovie_ExistingID_RemovesSuccessfully()
        {
            // Arrange
            MovieService service = CreateService();
            service.AddMovie(SampleMovie("M1"));

            // Act
            bool result = service.RemoveMovie("M1");

            // Assert
            Assert.True(result);
            Assert.Empty(service.GetAllMovies());
        }

        // TEST 6 - Removing a movie that does not exist returns false
        [Fact]
        public void RemoveMovie_NonExistentID_ReturnsFalse()
        {
            // Arrange
            MovieService service = CreateService();
            service.AddMovie(SampleMovie("M1"));

            // Act
            bool result = service.RemoveMovie("M999");

            // Assert
            Assert.False(result);
            Assert.Single(service.GetAllMovies());
        }

        // TEST 7 - Empty collection returns empty list not null
        [Fact]
        public void GetAllMovies_EmptyCollection_ReturnsEmptyList()
        {
            // Arrange
            MovieService service = CreateService();

            // Act
            List<Movie> result = service.GetAllMovies();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}