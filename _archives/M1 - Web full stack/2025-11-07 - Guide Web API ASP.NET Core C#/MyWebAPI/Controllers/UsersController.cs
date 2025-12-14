using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Models;

namespace MyWebAPI.Controllers;

/// <summary>
/// API Controller for managing users.
/// Provides CRUD operations (Create, Read, Update, Delete) for user resources.
/// </summary>
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    // IN-MEMORY DATA STORE
    private static List<User> _users = new()
    {
        new User { Id = 1, Name = "John Doe", Email = "john@example.com", Age = 30, CreatedAt = DateTime.UtcNow },
        new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Age = 25, CreatedAt = DateTime.UtcNow },
        new User { Id = 3, Name = "Bob Johnson", Email = "bob@example.com", Age = 35, CreatedAt = DateTime.UtcNow }
    };

    private static int _nextId = 4;

    /// <summary>
    /// Retrieves all users from the system.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        return Ok(_users);
    }

    /// <summary>
    /// Retrieves a specific user by their unique identifier.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<User> GetUser(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        
        if (user == null)
        {
            return NotFound(new { message = $"User with ID {id} not found" });
        }

        return Ok(user);
    }

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<User> CreateUser([FromBody] User user)
    {
        if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return BadRequest(new { message = "A user with this email already exists" });
        }

        user.Id = _nextId++;
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = null;
        _users.Add(user);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    /// <summary>
    /// Updates an existing user with new data.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
    {
        if (id != updatedUser.Id)
        {
            return BadRequest(new { message = "ID mismatch between URL and request body" });
        }

        var existingUser = _users.FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
        {
            return NotFound(new { message = $"User with ID {id} not found" });
        }

        if (_users.Any(u => u.Id != id && u.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return BadRequest(new { message = "Another user with this email already exists" });
        }

        existingUser.Name = updatedUser.Name;
        existingUser.Email = updatedUser.Email;
        existingUser.Age = updatedUser.Age;
        existingUser.UpdatedAt = DateTime.UtcNow;

        return NoContent();
    }

    /// <summary>
    /// Deletes a user from the system.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteUser(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        
        if (user == null)
        {
            return NotFound(new { message = $"User with ID {id} not found" });
        }

        _users.Remove(user);
        return NoContent();
    }

    /// <summary>
    /// Calculates and returns the average age of all users in the system.
    /// </summary>
    [HttpGet("average-age")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<object> GetAverageAge()
    {
        if (!_users.Any())
        {
            return Ok(new
            {
                averageAge = 0,
                totalUsers = 0,
                message = "No users in the system"
            });
        }

        var averageAge = _users.Average(u => u.Age);
        var totalUsers = _users.Count;

        return Ok(new
        {
            averageAge = Math.Round(averageAge, 2),
            totalUsers = totalUsers,
            message = $"Average calculated from {totalUsers} users"
        });
    }
}
