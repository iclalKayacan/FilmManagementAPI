namespace FilmManagementAPI.DTOs
{
    public class FilmResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public double? AverageRating { get; set; }
    }
}
