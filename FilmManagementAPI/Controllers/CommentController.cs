using FilmManagementAPI.Data;
using FilmManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmManagementAPI.Controllers
{
    [Route("api/films/{filmId}/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int filmId, Comment comment)
        {
            comment.FilmId = filmId;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetComments), new { filmId = filmId }, comment);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int filmId)
        {
            var comments = await _context.Comments.Where(c => c.FilmId == filmId).ToListAsync();
            return Ok(comments);
        }
    }

}
