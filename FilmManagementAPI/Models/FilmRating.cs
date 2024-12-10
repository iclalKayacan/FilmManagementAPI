namespace FilmManagementAPI.Models
{
    public class FilmRating
    {
        public int Id { get; set; } 
        public int FilmId { get; set; } 
        public Film Film { get; set; } = null!; 
        public int Rating { get; set; } 
    }

}
