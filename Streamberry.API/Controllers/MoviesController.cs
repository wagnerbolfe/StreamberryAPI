using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamberry.API.Entities;
using Streamberry.API.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

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

            var (movies, paginationMetadata) = await _movieRepository.GetMoviesAsync(title, search, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var moviesGrouped = movies
                .Select((value, index) => new
                {
                    movie = new
                    {
                        value.Id,
                        value.Title,
                        value.Genre,
                        value.ReleaseDate,
                        value.Streamings,
                        value.Ratings
                    }
                })
                .GroupBy(x => x.movie.Genre)
                .ToDictionary(wrap => wrap.Key);

            return Ok(moviesGrouped);
        }

        [HttpGet("streamings")]
        public async Task<IActionResult> GetStreamingsByMovie([FromQuery] string title)
        {
            var movies = await _movieRepository.GetStreamingsByMovie(title);

            var moviesGrouped = movies
                .Select((value, index) => new
                {
                    movie = new
                    {
                        value.Id,
                        value.Title,
                        value.Genre,
                        StreamingsCount = value.Streamings.Count,
                    }
                })
                .GroupBy(x => x.movie.Genre)
                .ToDictionary(wrap => wrap.Key);

            return Ok(moviesGrouped);
        }

        [HttpGet("ratingaverage")]
        public async Task<IActionResult> GetRatingAverageByMovie([FromQuery] string title)
        {
            var movies = await _movieRepository.GetStreamingsByMovie(title);

            var moviesGrouped = movies
                .Select((value, index) => new
                {
                    movie = new
                    {
                        value.Id,
                        value.Title,
                        value.Genre,
                        Average = value.Ratings.Select(x => x.Rating).Average()
        }
                })
                .GroupBy(x => x.movie.Genre)
                .ToDictionary(wrap => wrap.Key);

            return Ok(moviesGrouped);
        }

        [HttpGet("moviesinyear")]
        public async Task<IActionResult> GetMoviesByYear([FromQuery] string year)
        {
            var movies = await _movieRepository.GetMoviesByYear(year);

            var moviesGrouped = movies
                .Select((value, index) => new
                {
                    movie = new
                    {
                        value.Id,
                        value.Title,
                        value.Genre,
                        value.ReleaseDate,
                    }
                })
                .GroupBy(x => x.movie.ReleaseDate)
                .ToDictionary(wrap => wrap.Key);

            return Ok(moviesGrouped);
        }

        [HttpGet("moviesbyrating")]
        public async Task<IActionResult> GetMoviesByRating([FromQuery] int rating)
        {
            var movies = await _movieRepository.GetMoviesByRating();

            var moviesGrouped = movies
                .Select((value, index) => new
                {
                    movie = new
                    {
                        value.Id,
                        value.Title,
                        value.Genre,
                        value.ReleaseDate,
                        Rating = value.Ratings.Where(x => x.Rating == rating)
                    }
                })
                .GroupBy(x => x.movie.Rating);

            return Ok(moviesGrouped);
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
            var movieResult = await _movieRepository.GetMoviesAsync();

            var movieCheck = movieResult.Any(c => c.Title == movie.Title);

            if (movieCheck) return BadRequest("O título do filme já existe");

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
