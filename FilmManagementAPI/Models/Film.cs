﻿namespace FilmManagementAPI.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }


    }

}