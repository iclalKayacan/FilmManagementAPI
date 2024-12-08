using FilmManagementAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace FilmManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Film> Films { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<UserFavoriteFilm> UserFavoriteFilms { get; set; } = null!;
        public DbSet<UserWatchlist> UserWatchlist { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User-FavoriteFilm Relationship
            modelBuilder.Entity<UserFavoriteFilm>()
                .HasKey(uf => new { uf.UserId, uf.FilmId });

            modelBuilder.Entity<UserFavoriteFilm>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.FavoriteFilms)
                .HasForeignKey(uf => uf.UserId);

            modelBuilder.Entity<UserFavoriteFilm>()
                .HasOne(uf => uf.Film)
                .WithMany()
                .HasForeignKey(uf => uf.FilmId);

            // User-Watchlist Relationship
            modelBuilder.Entity<UserWatchlist>()
                .HasKey(uw => new { uw.UserId, uw.FilmId });

            modelBuilder.Entity<UserWatchlist>()
                .HasOne(uw => uw.User)
                .WithMany(u => u.Watchlist)
                .HasForeignKey(uw => uw.UserId);

            modelBuilder.Entity<UserWatchlist>()
                .HasOne(uw => uw.Film)
                .WithMany()
                .HasForeignKey(uw => uw.FilmId);
        }
    }



}
