namespace FilmManagementAPI.Models
{
    public class UserFavoriteFilm
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int FilmId { get; set; }
        public Film Film { get; set; } = null!;
    }

}
