using Microsoft.AspNetCore.Mvc;
using FilmManagementAPI.Models;
using FilmManagementAPI.Data;
using Microsoft.EntityFrameworkCore;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpPost("login")]
    public IActionResult Login(string email, string password)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == email && u.Password == password);
        if (user == null) return Unauthorized("Invalid credentials");
        return Ok(user);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(User user)
    {
        _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }
    [HttpPost("{userId}/favorites/{filmId}")]
    public async Task<IActionResult> AddToFavorites(int userId, int filmId)
    {
        // Kullanıcıyı bul
        var user = await _context.Users.Include(u => u.FavoriteFilms).ThenInclude(uf => uf.Film).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(new { message = "Kullanıcı bulunamadı." });
        }

        // Film var mı kontrol et
        var film = await _context.Films.FindAsync(filmId);
        if (film == null)
        {
            return NotFound(new { message = "Film bulunamadı." });
        }

        // Favorilere zaten eklenmiş mi kontrol et
        if (user.FavoriteFilms.Any(f => f.FilmId == filmId))
        {
            return BadRequest(new { message = "Film zaten favorilerde." });
        }

        // Favorilere ekle
        user.FavoriteFilms.Add(new UserFavoriteFilm { UserId = userId, FilmId = filmId });
        await _context.SaveChangesAsync();

        return Ok(new { message = "Film favorilere eklendi." });
    }
    [HttpDelete("{userId}/favorites/{filmId}")]
    public async Task<IActionResult> RemoveFromFavorites(int userId, int filmId)
    {
        // Kullanıcıyı bul
        var user = await _context.Users.Include(u => u.FavoriteFilms).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(new { message = "Kullanıcı bulunamadı." });
        }

        // Favori listesinde filmi bul
        var favoriteFilm = user.FavoriteFilms.FirstOrDefault(f => f.FilmId == filmId);
        if (favoriteFilm == null)
        {
            return NotFound(new { message = "Film favorilerde bulunamadı." });
        }

        // Favorilerden çıkar
        user.FavoriteFilms.Remove(favoriteFilm);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Film favorilerden çıkarıldı." });
    }
    [HttpGet("{userId}/favorites")]
    public async Task<IActionResult> GetFavorites(int userId)
    {
        // Kullanıcıyı bul
        var user = await _context.Users.Include(u => u.FavoriteFilms).ThenInclude(uf => uf.Film).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(new { message = "Kullanıcı bulunamadı." });
        }

        // Favori filmleri listele
        var favoriteFilms = user.FavoriteFilms.Select(f => new
        {
            f.Film.Id,
            f.Film.Title,
            f.Film.Director,
            f.Film.Genre,
            f.Film.ReleaseYear
        });

        return Ok(favoriteFilms);
    }


}
