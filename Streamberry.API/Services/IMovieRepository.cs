using Streamberry.API.Entities;

namespace Streamberry.API.Services
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetMoviesAsync();
        Task<(IEnumerable<Movie>, PaginationMetadata)> GetMoviesAsync(string title, string search, int pageNumber, int pageSize);
        Task<IEnumerable<Movie>> GetStreamingsByMovie(string title);
        Task<IEnumerable<Movie>> GetMoviesByYear(string year);
        Task<IEnumerable<Movie>> GetMoviesByRating();
        Task<Movie> GetMovieAsync(int id);
        void CreateMovie(Movie movie);
        void UpdateMovie(int id, Movie movie);
        void DeleteMovie(int id);
        Task<bool> SaveChangesAsync();
    }
}
