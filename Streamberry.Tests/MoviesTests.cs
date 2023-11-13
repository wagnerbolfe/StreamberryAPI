using Microsoft.EntityFrameworkCore;
using Streamberry.API.Data;
using Streamberry.API.Entities;
using Streamberry.API.Services;

namespace Streamberry.Tests
{
    public class MovieRepositoryTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly IMovieRepository _repository;

        public MovieRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "MovieRepositoryTestDatabase")
                .Options;

            _context = new DataContext(options);
            _repository = new MovieRepository(_context);
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task GetMoviesAsync_ShouldReturnListOfMovies()
        {
            // Arrange
            await SeedDatabaseWithMovies();

            // Act
            var movies = await _repository.GetMoviesAsync();

            // Assert
            Assert.NotNull(movies);
            Assert.NotEmpty(movies);
            Assert.IsType<List<Movie>>(movies);
        }

        [Fact]
        public async Task GetMovieAsync_ShouldReturnMovieById()
        {
            // Arrange
            await SeedDatabaseWithMovies();

            // Act
            var movie = await _repository.GetMovieAsync(1);

            // Assert
            Assert.NotNull(movie);
            Assert.Equal(1, movie.Id);
        }

        [Fact]
        public void CreateMovie_ShouldAddMovieToDatabase()
        {
            // Arrange
            var movie = new Movie
            {
                Title = "Test Movie",
                Genre = "Test Genre",
                ReleaseDate = DateTime.Now,
                Streamings = new List<Streaming> { new() { StreamingName = "Stream 2" } },
                Ratings = new List<UserRating> { new() { Comment = "Muito Bom", Rating = 3 } },
            };

            // Act
            _repository.CreateMovie(movie);

            // Assert
            Assert.Equal("Test Movie", movie.Title);
            Assert.Equal("Test Genre", movie.Genre);
        }

        [Fact]
        public async Task UpdateMovie_ShouldUpdateExistingMovie()
        {
            // Arrange
            await SeedDatabaseWithMovies();

            var updatedMovie = new Movie {
                Id = 1,
                Title = "Updated Movie", 
                Genre = "Updated Genre", 
                ReleaseDate = DateTime.Now,
                Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 2" } },
                Ratings = new List<UserRating> { new UserRating { Comment = "Muito Bom", Rating = 3 } },
            };

            // Act
            _repository.UpdateMovie(1, updatedMovie);
            await _repository.SaveChangesAsync();

            // Assert
            var movie = await _repository.GetMovieAsync(1);

            Assert.Equal("Updated Movie", movie.Title);
            Assert.Equal("Updated Genre", movie.Genre);
        }

        [Fact]
        public async Task DeleteMovie_ShouldRemoveMovieFromDatabase()
        {
            // Arrange
            await SeedDatabaseWithMovies();

            // Act
            _repository.DeleteMovie(1);
            await _repository.SaveChangesAsync();

            // Assert
            var movie = await _context.Movies.FindAsync(1);
            Assert.Null(movie);
        }

        private async Task SeedDatabaseWithMovies()
        {
            var movies = new List<Movie>
            {
                new Movie
                {

                    Title = "Movie 1",
                    Genre = "Genre 1",
                    ReleaseDate = DateTime.Now,
                    Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 1" } },
                    Ratings = new List<UserRating> { new UserRating { Comment = "Muito Bom", Rating = 3 } },
                },
                new Movie
                {

                    Title = "Movie 2",
                    Genre = "Genre 2",
                    ReleaseDate = DateTime.Now,
                    Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 2" } },
                    Ratings = new List<UserRating> { new UserRating { Comment = "Muito Bom", Rating = 3 } },
                },
                new Movie
                {

                    Title = "Movie 3",
                    Genre = "Genre 3",
                    ReleaseDate = DateTime.Now,
                    Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 3" } },
                    Ratings = new List<UserRating> { new UserRating { Comment = "Muito Bom", Rating = 3 } },
                }
            };

            await _context.Movies.AddRangeAsync(movies);
            await _context.SaveChangesAsync();
        }

    }
}