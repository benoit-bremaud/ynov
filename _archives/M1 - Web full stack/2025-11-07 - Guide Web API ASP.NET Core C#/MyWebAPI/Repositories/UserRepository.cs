using MyWebAPI.Models;

namespace MyWebAPI.Repositories;

/// <summary>
/// In-memory implementation of the User repository.
/// Stores users in a static list (data is lost on application restart).
/// In production, this would be replaced with a database implementation.
/// </summary>
public class UserRepository : IUserRepository
{
    // ═══════════════════════════════════════════════════════════════
    // IN-MEMORY DATA STORE
    // ═══════════════════════════════════════════════════════════════
    /// <summary>
    /// Static list to store users in memory.
    /// Static = shared across all repository instances
    /// </summary>
    private static List<User> _users = new()
    {
        new User { Id = 1, Name = "John Doe", Email = "john@example.com", Age = 30, CreatedAt = DateTime.UtcNow },
        new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Age = 25, CreatedAt = DateTime.UtcNow },
        new User { Id = 3, Name = "Bob Johnson", Email = "bob@example.com", Age = 35, CreatedAt = DateTime.UtcNow }
    };

    /// <summary>
    /// Counter for generating unique user IDs.
    /// </summary>
    private static int _nextId = 4;

    /// <summary>
    /// Lock object for thread-safe operations on the user list.
    /// </summary>
    private static readonly object _lock = new object();

    // ═══════════════════════════════════════════════════════════════
    // REPOSITORY METHODS
    // ═══════════════════════════════════════════════════════════════

    /// <inheritdoc />
    public IEnumerable<User> GetAll()
    {
        lock (_lock)
        {
            // Return a copy to prevent external modifications
            return _users.ToList();
        }
    }

    /// <inheritdoc />
    public User? GetById(int id)
    {
        lock (_lock)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }
    }

    /// <inheritdoc />
    public User Add(User user)
    {
        lock (_lock)
        {
            user.Id = _nextId++;
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = null;
            _users.Add(user);
            return user;
        }
    }

    /// <inheritdoc />
    public bool Update(User user)
    {
        lock (_lock)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser == null)
            {
                return false;
            }

            // Update properties
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Age = user.Age;
            existingUser.UpdatedAt = DateTime.UtcNow;

            return true;
        }
    }

    /// <inheritdoc />
    public bool Delete(int id)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return false;
            }

            _users.Remove(user);
            return true;
        }
    }

    /// <inheritdoc />
    public bool EmailExists(string email, int? excludeUserId = null)
    {
        lock (_lock)
        {
            return _users.Any(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                (excludeUserId == null || u.Id != excludeUserId.Value));
        }
    }
}
