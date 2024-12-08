using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FilmManagementAPI.Data;
using FilmManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmManagementAPI.Controllers
{

    [Route("api/films")]

    [ApiController]
    public class FilmController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FilmController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/films
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Film>>> GetFilms()
        {
            return await _context.Films.ToListAsync();
        }

        // GET: api/films/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Film>> GetFilm(int id)
        {
            var film = await _context.Films.FindAsync(id);

            if (film == null)
            {
                return NotFound();
            }

            return film;
        }

        // POST: api/films
        [HttpPost]
        public async Task<ActionResult<Film>> PostFilm(Film film)
        {
            _context.Films.Add(film);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFilm), new { id = film.Id }, film);
        }

        // PUT: api/films/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFilm(int id, Film film)
        {
            if (id != film.Id)
            {
                return BadRequest();
            }

            _context.Entry(film).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Films.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/films/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFilm(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }

            _context.Films.Remove(film);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Film>>> FilterFilms(string? genre, string? director, int? releaseYear)
        {
            // Veritabanında sorguyu başlatıyoruz
            var query = _context.Films.AsQueryable();

            // Tür filtrelemesi
            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(f => f.Genre.ToLower() == genre.ToLower());
            }

            // Yönetmen filtrelemesi
            if (!string.IsNullOrEmpty(director))
            {
                query = query.Where(f => f.Director.ToLower() == director.ToLower());
            }

            // Yayın yılı filtrelemesi
            if (releaseYear.HasValue)
            {
                query = query.Where(f => f.ReleaseYear == releaseYear.Value);
            }

            // Sorguyu çalıştır ve sonuçları döndür
            var films = await query.ToListAsync();

            if (!films.Any())
            {
                return NotFound(new { message = "Filtreye uygun film bulunamadı." });
            }

            return Ok(films);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Film>>> SearchFilms(string title)
        {
            // Film adında geçen kelimelere göre arama yapıyoruz (case-insensitive)
            var films = await _context.Films
                .Where(f => f.Title.ToLower().Contains(title.ToLower()))
                .ToListAsync();

            // Eğer film bulunamazsa hata mesajı döndür
            if (!films.Any())
            {
                return NotFound(new { message = $"'{title}' kelimesini içeren film bulunamadı." });
            }

            return Ok(films);
        }




    }

}
