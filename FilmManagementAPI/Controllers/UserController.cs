using Microsoft.AspNetCore.Mvc;
using FilmManagementAPI.Models;
using FilmManagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FilmManagementAPI.Services;
using FilmManagementAPI.DTOs;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    private readonly TokenService _tokenService;

    public UserController(ApplicationDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        user.Role = string.IsNullOrEmpty(user.Role) ? "User" : user.Role;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == request.Email && u.Password == request.Password);
        if (user == null)
        {
            return Unauthorized(new { message = "Geçersiz giriş bilgileri." });
        }

        var token = _tokenService.GenerateToken(user);
        return Ok(new { token });
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
        var user = await _context.Users.Include(u => u.FavoriteFilms).ThenInclude(uf => uf.Film).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(new { message = "Kullanıcı bulunamadı." });
        }

        var film = await _context.Films.FindAsync(filmId);
        if (film == null)
        {
            return NotFound(new { message = "Film bulunamadı." });
        }

        if (user.FavoriteFilms.Any(f => f.FilmId == filmId))
        {
            return BadRequest(new { message = "Film zaten favorilerde." });
        }

        user.FavoriteFilms.Add(new UserFavoriteFilm { UserId = userId, FilmId = filmId });
        await _context.SaveChangesAsync();

        return Ok(new { message = "Film favorilere eklendi." });
    }
    [HttpDelete("{userId}/favorites/{filmId}")]
    public async Task<IActionResult> RemoveFromFavorites(int userId, int filmId)
    {
        var user = await _context.Users.Include(u => u.FavoriteFilms).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(new { message = "Kullanıcı bulunamadı." });
        }

        var favoriteFilm = user.FavoriteFilms.FirstOrDefault(f => f.FilmId == filmId);
        if (favoriteFilm == null)
        {
            return NotFound(new { message = "Film favorilerde bulunamadı." });
        }

        user.FavoriteFilms.Remove(favoriteFilm);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Film favorilerden çıkarıldı." });
    }
    [HttpGet("{userId}/favorites")]
    public async Task<IActionResult> GetFavorites(int userId)
    {
        var user = await _context.Users.Include(u => u.FavoriteFilms).ThenInclude(uf => uf.Film).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(new { message = "Kullanıcı bulunamadı." });
        }

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
    [HttpPost("{userId}/watchlist/{filmId}")]
    public async Task<IActionResult> AddToWatchlist(int userId, int filmId)
    {
        var user = await _context.Users.Include(u => u.Watchlist).ThenInclude(uw => uw.Film).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(new { message = "Kullanıcı bulunamadı." });
        }

        var film = await _context.Films.FindAsync(filmId);
        if (film == null)
        {
            return NotFound(new { message = "Film bulunamadı." });
        }

        if (user.Watchlist.Any(w => w.FilmId == filmId))
        {
            return BadRequest(new { message = "Film zaten izleme listesinde." });
        }

        user.Watchlist.Add(new UserWatchlist { UserId = userId, FilmId = filmId });
        await _context.SaveChangesAsync();

        return Ok(new { message = "Film izleme listesine eklendi." });
    }
    [HttpDelete("{userId}/watchlist/{filmId}")]
    public async Task<IActionResult> RemoveFromWatchlist(int userId, int filmId)
    {
        var user = await _context.Users.Include(u => u.Watchlist).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(new { message = "Kullanıcı bulunamadı." });
        }

        var watchlistFilm = user.Watchlist.FirstOrDefault(w => w.FilmId == filmId);
        if (watchlistFilm == null)
        {
            return NotFound(new { message = "Film izleme listesinde bulunamadı." });
        }

        user.Watchlist.Remove(watchlistFilm);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Film izleme listesinden çıkarıldı." });
    }
    [HttpGet("{userId}/watchlist")]
    public async Task<IActionResult> GetWatchlist(int userId)
    {
        var user = await _context.Users.Include(u => u.Watchlist).ThenInclude(uw => uw.Film).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(new { message = "Kullanıcı bulunamadı." });
        }

        var watchlistFilms = user.Watchlist.Select(w => new
        {
            w.Film.Id,
            w.Film.Title,
            w.Film.Director,
            w.Film.Genre,
            w.Film.ReleaseYear
        });

        return Ok(watchlistFilms);
    }


}
