using Streamberry.API.Entities;

namespace Streamberry.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DataContext context)
        {
            if (context.Movies.Any()) return;

            var movies = new List<Movie>
            {
                new Movie
                {
                    Title = "Piratas do Caribe - Pérola Negra",
                    Genre = "Action",
                    Streamings = new List<Streaming>
                    {
                        new() { StreamingName = "Netflix" },
                        new() { StreamingName = "Disney+" },
                        new() { StreamingName = "Paramount+" },
                    },
                    Ratings = new List<UserRating>
                    {
                        new() { Rating = 1, Comment = "Não Gostei Muito!"},
                        new() { Rating = 5, Comment = "Muito Bom!"}
                    },
                    ReleaseDate = DateTime.Now.AddMonths(-200)
        },
                new Movie
                {
                    Title = "Piratas do Caribe - O Baú da Morte",
                    Genre = "Adventure",
                    Streamings = new List<Streaming>
                    {
                        new() { StreamingName = "Netflix" },
                        new() { StreamingName = "Disney+" },
                        new() { StreamingName = "Paramount+" },
                    },
                    Ratings = new List<UserRating>
                    {
                        new() { Rating = 2, Comment = "Não Gostei Muito!"},
                        new() { Rating = 5, Comment = "Muito Bom!"}
                    },
                    ReleaseDate = DateTime.Now.AddMonths(-160)
                },
                new Movie
                {
                    Title = "Piratas do Caribe - No Fim do Mundo",
                    Genre = "Adventure",
                    Streamings = new List<Streaming>
                    {
                        new() { StreamingName = "Netflix" },
                        new() { StreamingName = "Disney+" },
                        new() { StreamingName = "Paramount+" },
                    },
                    Ratings = new List<UserRating>
                    {
                        new() { Rating = 2, Comment = "Não Gostei Muito!"},
                        new() { Rating = 4, Comment = "Muito Bom!"}
                    },
                    ReleaseDate = DateTime.Now.AddMonths(-100)
                },
                new Movie
                {
                    Title = "Piratas do Caribe - Navegando em Aguas Misteriosas",
                    Genre = "Action",
                    ReleaseDate = DateTime.Now.AddMonths(-75),
                    Streamings = new List<Streaming>
                    {
                        new() { StreamingName = "Hulu" },
                        new() { StreamingName = "Disney+" },
                    }
                },
                new Movie
                {
                    Title = "Piratas do Caribe - A Vingança de Salazar",
                    Genre = "Action",
                    ReleaseDate = DateTime.Now.AddMonths(-50),
                    Streamings = new List<Streaming>
                    {
                        new() { StreamingName = "Netflix" },
                        new() { StreamingName = "GloboPlay" },
                    },
                    Ratings = new List<UserRating>
                    {
                        new() { Rating = 3, Comment = "Muito Bom!"},
                        new() { Rating = 2, Comment = "Não Gostei Muito!"},
                    }
                },
            };

            context.Movies.AddRange(movies);

            context.SaveChanges();
        }
    }
}
