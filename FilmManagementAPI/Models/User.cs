using System.Text.Json.Serialization;

namespace FilmManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";

        [JsonIgnore]
        public ICollection<UserFavoriteFilm> FavoriteFilms { get; set; } = new List<UserFavoriteFilm>();

        [JsonIgnore]
        public ICollection<UserWatchlist> Watchlist { get; set; } = new List<UserWatchlist>();
    }



}


