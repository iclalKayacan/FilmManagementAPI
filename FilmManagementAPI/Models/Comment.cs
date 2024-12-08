namespace FilmManagementAPI.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int FilmId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
