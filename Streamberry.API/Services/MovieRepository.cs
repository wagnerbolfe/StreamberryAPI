using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Streamberry.API.Data;
using Streamberry.API.Entities;

namespace Streamberry.API.Services
{
    public class MovieRepository : IMovieRepository
    {
        private readonly DataContext _context;

        public MovieRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync()
        {
            var movies = await _context.Movies
                .Include(x => x.Streamings)
                .Include(x => x.Ratings)
                .ToListAsync();

            return movies;
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync(string title, string search, int pageNumber, int pageSize)
        {
            var collection = _context.Movies
                .Include(x => x.Streamings)
                .Include(x => x.Ratings) as IQueryable<Movie>;

            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.Trim();
                collection = collection.Where(c => c.Title == title);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                collection = collection
                    .Where(a => a.Title.Contains(search)
                    || (a.Genre != null && a.Genre.Contains(search)));
            }

            return await collection.OrderBy(c => c.Genre)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Movie> GetMovieAsync(int id)
        {
            var movie = await _context.Movies
                .Include(x => x.Streamings)
                .Include(x => x.Ratings)
                .FirstOrDefaultAsync(m => m.Id == id);

            return movie;
        }

        public void CreateMovie(Movie movie)
        {
            if (movie is null)
            {
                throw new ArgumentNullException(nameof(movie));
            }

            _context.Movies.Add(movie);
        }

        public void UpdateMovie(int id, Movie movie)
        {
            var movieExist = _context.Movies
                .Include(s => s.Streamings)
                .Include(r => r.Ratings)
                .FirstOrDefault(d => d.Id == id);

            if (movieExist != null)
            {
                movieExist.Title = movie.Title;
                movieExist.Genre = movie.Genre;
                movieExist.ReleaseDate = movie.ReleaseDate;

                foreach (var existingRating in movieExist.Ratings.ToList())
                {
                    if (!movie.Ratings.Any(c => c.Id == existingRating.Id))
                        _context.UserRatings.Remove(existingRating);
                }

                foreach (var ratingModel in movie.Ratings)
                {
                    var existingChild = movieExist.Ratings
                        .Where(c => c.Id == ratingModel.Id && c.Id != default)
                        .SingleOrDefault();

                    if (existingChild != null)
                        _context.Entry(existingChild).CurrentValues.SetValues(ratingModel);
                    else
                    {
                        var newRating = new UserRating
                        {
                            Rating = ratingModel.Rating,
                            Comment = ratingModel.Comment,
                        };
                        movieExist.Ratings.Add(newRating);
                    }
                }

                foreach (var existingStream in movieExist.Streamings.ToList())
                {
                    if (!movie.Streamings.Any(c => c.Id == existingStream.Id))
                        _context.Streamings.Remove(existingStream);
                }

                foreach (var streamModel in movie.Streamings)
                {
                    var existingChild = movieExist.Streamings
                        .Where(c => c.Id == streamModel.Id && c.Id != default)
                        .SingleOrDefault();

                    if (existingChild != null)
                        _context.Entry(existingChild).CurrentValues.SetValues(streamModel);
                    else
                    {
                        var newStream = new Streaming
                        {
                            StreamingName = streamModel.StreamingName
                        };
                        movieExist.Streamings.Add(newStream);
                    }
                }

            }
        }

        public void DeleteMovie(int id)
        {
            var movieExist = _context.Movies.FirstOrDefault(d => d.Id == id);

            _context.Movies.Remove(movieExist);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
