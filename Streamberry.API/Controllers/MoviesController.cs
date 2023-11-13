using Microsoft.AspNetCore.Mvc;
using Streamberry.API.Entities;
using Streamberry.API.Services;

namespace Streamberry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ILogger<MoviesController> _logger;
        const int maxMoviesPageSize = 10;

        public MoviesController(IMovieRepository movieRepository, ILogger<MoviesController> logger)
        {
            _movieRepository = movieRepository ?? 
                throw new ArgumentNullException(nameof(movieRepository));
            _logger = logger ?? 
                throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies([FromQuery]string title, string search, int pageNumber = 1, int pageSize = 5)
        {
            if (pageSize > maxMoviesPageSize) pageSize = maxMoviesPageSize;

            var movies = await _movieRepository.GetMoviesAsync(title, search, pageNumber, pageSize);

            return Ok(movies);
        }

        [HttpGet("{id}", Name = "GetMovie")]
        public async Task<IActionResult> GetMovie(int id)
        {
            var movie = await _movieRepository.GetMovieAsync(id);

            if (movie is null) return NotFound();

            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody]Movie movie)
        {
            _movieRepository.CreateMovie(movie);

            await _movieRepository.SaveChangesAsync();

            return CreatedAtRoute("GetMovie", new { id = movie.Id }, movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] Movie movie)
        {
            _movieRepository.UpdateMovie(id, movie);

            await _movieRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            _movieRepository.DeleteMovie(id);

            await _movieRepository.SaveChangesAsync();

            return Ok();
        }
    }
}
