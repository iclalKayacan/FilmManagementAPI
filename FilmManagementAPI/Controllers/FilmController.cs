using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FilmManagementAPI.Data;
using FilmManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using FilmManagementAPI.DTOs;

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
        [HttpGet("{filmId}")]
        public async Task<ActionResult<Film>> GetFilm(int filmId)
        {
            var film = await _context.Films
                .Include(f => f.Ratings)
                .FirstOrDefaultAsync(f => f.Id == filmId);

            if (film == null)
            {
                return NotFound(new { message = "Film bulunamadı." });
            }

            var averageRating = film.Ratings.Any() ? film.Ratings.Average(r => r.Rating) : 0;

            return Ok(new
            {
                film.Id,
                film.Title,
                film.Director,
                film.Genre,
                film.ReleaseYear,
                AverageRating = averageRating,
                RatingsCount = film.Ratings.Count
            });
        }

        // POST: api/films 
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<FilmResponse>> PostFilm(Film film)
        {
            _context.Films.Add(film);
            await _context.SaveChangesAsync();

            var response = new FilmResponse
            {
                Id = film.Id,
                Title = film.Title,
                Director = film.Director,
                Genre = film.Genre,
                ReleaseYear = film.ReleaseYear
            };

            return CreatedAtAction(nameof(GetFilm), new { filmId = film.Id }, response);
        }

        // PUT: api/films/{id} 
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFilm(int id)
        {
            try
            {
                var film = await _context.Films.FindAsync(id);
                if (film == null)
                {
                    return NotFound(new { message = $"Film ID {id} bulunamadı. Silme işlemi başarısız." });
                }

                _context.Films.Remove(film);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Film ID {id} başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Film silme işlemi sırasında bir hata oluştu.",
                    error = ex.Message
                });
            }
        }


        // GET: api/films/filter 
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Film>>> FilterFilms(string? genre, string? director, int? releaseYear)
        {
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

            var films = await query.ToListAsync();

            if (!films.Any())
            {
                return NotFound(new { message = "Filtreye uygun film bulunamadı." });
            }

            return Ok(films);
        }

        // GET: api/films/search 
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Film>>> SearchFilms(string title)
        {
            var films = await _context.Films
                .Where(f => f.Title.ToLower().Contains(title.ToLower()))
                .ToListAsync();

            if (!films.Any())
            {
                return NotFound(new { message = $"'{title}' kelimesini içeren film bulunamadı." });
            }

            return Ok(films);
        }

        // POST: api/films/{filmId}/rate 
        [HttpPost("{filmId}/rate")]
        public async Task<IActionResult> RateFilm(int filmId, [FromBody] int rating)
        {
            if (rating < 1 || rating > 10)
            {
                return BadRequest(new { message = "Puan 1 ile 10 arasında olmalıdır." });
            }

            var film = await _context.Films.Include(f => f.Ratings).FirstOrDefaultAsync(f => f.Id == filmId);
            if (film == null)
            {
                return NotFound(new { message = "Film bulunamadı." });
            }

            var filmRating = new FilmRating
            {
                FilmId = filmId,
                Rating = rating
            };
            _context.FilmRatings.Add(filmRating);

            film.Ratings.Add(filmRating);
            var averageRating = film.Ratings.Average(r => r.Rating);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Puanlama başarılı.", averageRating });
        }
    }
}
