using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Streamberry.API.Entities
{
    public class Streaming
    {
        public int Id { get; set; }

        [Required]
        public string StreamingName { get; set; }

        //EF
        [JsonIgnore]
        public int MovieId { get; set; }

        [JsonIgnore]
        public Movie Movie { get; set; }

    }
}
