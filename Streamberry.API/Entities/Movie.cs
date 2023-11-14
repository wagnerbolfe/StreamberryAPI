using System.ComponentModel.DataAnnotations;

namespace Streamberry.API.Entities
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "O nome do filme deve ter 100 caracteres")]
        public string Title { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "O genero do filme deve ter 50 caracteres")]
        public string Genre { get; set; }

        [Required]
        public string ReleaseDate { get; set; }

        public List<Streaming> Streamings { get; set; } 
        public List<UserRating> Ratings { get; set; }
    }
}
