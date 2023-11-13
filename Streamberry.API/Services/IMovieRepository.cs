using Streamberry.API.Entities;

namespace Streamberry.API.Services
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetMoviesAsync();
        Task<IEnumerable<Movie>> GetMoviesAsync(string title, string search, int pageNumber, int pageSize);
        Task<Movie> GetMovieAsync(int id);
        void CreateMovie(Movie movie);
        void UpdateMovie(int id, Movie movie);
        void DeleteMovie(int id);
        Task<bool> SaveChangesAsync();
    }
}
