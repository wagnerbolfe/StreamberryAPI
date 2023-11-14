using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamberry.API.Data;
using Streamberry.API.Entities;
using Streamberry.API.Services;

namespace Streamberry.Tests
{
    public class MovieServiceTests
    {
        private DbContextOptions<DataContext> CreateInMemoryDatabaseOptions(string dbName)
        {
            return new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
        }

        [Fact]
        public async Task GetMoviesAsync_ReturnsAllMovies()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions("GetMoviesAsync_ReturnsAllMovies");
            using (var context = new DataContext(options))
            {
                var repository = new MovieRepository(context);
                await context.Movies.AddRangeAsync(new[]
                {
                    new Movie
                    {
                        Title = "Filme Teste1",
                        Genre = "Action",
                        ReleaseDate = "12/2020",
                        Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 1" } },
                        Ratings = new List<UserRating> { new UserRating { Comment = "Comment 1", Rating = 3 } },
                    },
                    new Movie
                    {
                        Title = "Filme Teste 2",
                        Genre = "Action",
                        ReleaseDate = "12/2021",
                        Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 2" } },
                        Ratings = new List<UserRating> { new UserRating { Comment = "Comment 2", Rating = 3 } },
                    },
                    new Movie
                    {
                        Title = "Filme Teste 3",
                        Genre = "Action",
                        ReleaseDate = "12/2022",
                        Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 3" } },
                        Ratings = new List<UserRating> { new UserRating { Comment = "Comment 3", Rating = 3 } },
                    }

                });
                await context.SaveChangesAsync();

                // Act
                var movies = await repository.GetMoviesAsync();

                // Assert
                Assert.NotNull(movies);
                Assert.Equal(3, movies.Count());
            }
        }

        [Fact]
        public async Task GetMovieAsync_ReturnsCorrectMovie()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions("GetMovieAsync_ReturnsCorrectMovie");
            using (var context = new DataContext(options))
            {
                var repository = new MovieRepository(context);
                await context.Movies.AddRangeAsync(new[]
                {
                    new Movie
                    {
                        Title = "Filme Teste1",
                        Genre = "Action",
                        ReleaseDate = "12/2020",
                        Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 1" } },
                        Ratings = new List<UserRating> { new UserRating { Comment = "Comment 1", Rating = 3 } },
                    },
                    new Movie
                    {
                        Title = "Filme Teste 2",
                        Genre = "Action",
                        ReleaseDate = "12/2021",
                        Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 2" } },
                        Ratings = new List<UserRating> { new UserRating { Comment = "Comment 2", Rating = 3 } },
                    }
                });
                await context.SaveChangesAsync();

                // Act
                var movie = await repository.GetMovieAsync(2);

                // Assert
                Assert.NotNull(movie);
                Assert.Equal("Filme Teste 2", movie.Title);
            }
        }

        [Fact]
        public async Task UpdateMovie_ShouldUpdateExistingMovie()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions("UpdateMovie_ShouldUpdateExistingMovie");
            using (var context = new DataContext(options))
            {
                var repository = new MovieRepository(context);

                await SeedDatabaseWithMovies();

                var createdMovie = new Movie
                {
                    Id = 2,
                    Title = "Filme Teste",
                    Genre = "Action",
                    ReleaseDate = "12/2023",
                    Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream" } },
                    Ratings = new List<UserRating> { new UserRating { Comment = "Comment", Rating = 3 } },
                };

                var updatedMovie = new Movie
                {
                    Id = 2,
                    Title = "Filme Teste Atualizado",
                    Genre = "Action",
                    ReleaseDate = "12/2021",
                    Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream Atualizado" } },
                    Ratings = new List<UserRating> { new UserRating { Comment = "Comment Atualizado", Rating = 3 } },
                };

                // Act
                repository.CreateMovie(createdMovie);
                await context.SaveChangesAsync();

                repository.UpdateMovie(2, updatedMovie);
                await context.SaveChangesAsync();

                var movie = await repository.GetMovieAsync(2);

                // Assert
                Assert.NotNull(movie);
                Assert.Equal(updatedMovie.Title, movie.Title);
                Assert.Equal(updatedMovie.Genre, movie.Genre);
            }
        }

        [Fact]
        public void CreateMovie_ShouldAddMovieToDatabase()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions("CreateMovie_ShouldAddMovieToDatabase");
            using (var context = new DataContext(options))
            {
                var repository = new MovieRepository(context);
                var movie = new Movie
                {
                    Title = "Filme Teste 2",
                    Genre = "Action",
                    ReleaseDate = "12/2021",
                    Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 2" } },
                    Ratings = new List<UserRating> { new UserRating { Comment = "Comment 2", Rating = 3 } },
                };

                // Act
                repository.CreateMovie(movie);
                _ = repository.SaveChangesAsync();

                // Assert
                Assert.Contains(movie, context.Movies);
            }
        }

        [Fact]
        public void DeleteMovie_RemovesMovie()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions("DeleteMovie_RemovesMovie");
            using (var context = new DataContext(options))
            {
                var repository = new MovieRepository(context);
                context.Movies.Add(
                    new Movie
                    {
                        Id = 1,
                        Title = "Filme Teste 2",
                        Genre = "Action",
                        ReleaseDate = "12/2021",
                        Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 2" } },
                        Ratings = new List<UserRating> { new UserRating { Comment = "Comment 2", Rating = 3 } },
                    });
                context.SaveChanges();

                // Act
                repository.DeleteMovie(1);
                _ = repository.SaveChangesAsync();

                // Assert
                Assert.Empty(context.Movies);
            }
        }

        private async Task SeedDatabaseWithMovies()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions("SeedDatabaseWithMovies");
            using (var context = new DataContext(options))
            {
                var repository = new MovieRepository(context);
                var movies = new List<Movie>
                {
                    new Movie
                    {
                        Id = 1,
                        Title = "Filme Teste 1",
                        Genre = "Action",
                        ReleaseDate = "12/2021",
                        Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 1" } },
                        Ratings = new List<UserRating> { new UserRating { Comment = "Comment 1", Rating = 1 } },
                    },
                    new Movie
                    {
                        Id = 2,
                        Title = "Filme Teste 2",
                        Genre = "Adventure",
                        ReleaseDate = "12/2022",
                        Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 2" } },
                        Ratings = new List<UserRating> { new UserRating { Comment = "Comment 2", Rating = 2 } },
                    },
                    new Movie
                    {
                        Id = 3,
                        Title = "Filme Teste 3",
                        Genre = "Suspense",
                        ReleaseDate = "12/2023",
                        Streamings = new List<Streaming> { new Streaming { StreamingName = "Stream 3" } },
                        Ratings = new List<UserRating> { new UserRating { Comment = "Comment 3", Rating = 3 } },
                    }
                };

                await context.Movies.AddRangeAsync(movies);
                await context.SaveChangesAsync();
            }
        }
    }
}
