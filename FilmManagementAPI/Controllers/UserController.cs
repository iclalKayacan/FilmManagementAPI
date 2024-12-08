using Microsoft.AspNetCore.Mvc;
using FilmManagementAPI.Models;
using FilmManagementAPI.Data;

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
}
