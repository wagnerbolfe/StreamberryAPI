using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Streamberry.API.Entities
{
    public class UserRating
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "O comentário deve ter 100 caracteres")]
        public string Comment { get; set; }

        [Range(1, 5, ErrorMessage = "A avaliação deve ser de 1 até 5")]
        public int Rating { get; set; }

        //EF
        [JsonIgnore]
        public int MovieId { get; set; }

        [JsonIgnore]
        public Movie Movie { get; set; }
    }
}
