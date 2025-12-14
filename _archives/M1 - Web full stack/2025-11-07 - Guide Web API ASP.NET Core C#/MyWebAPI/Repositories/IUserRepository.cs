using MyWebAPI.Models;

namespace MyWebAPI.Repositories;

/// <summary>
/// Interface for User data access operations.
/// Defines the contract for user repository implementations.
/// </summary>
/// <remarks>
/// This follows the Repository Pattern, which abstracts data access
/// and allows for easy testing and swapping of data sources.
/// </remarks>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves all users from the data store.
    /// </summary>
    /// <returns>A list of all users</returns>
    IEnumerable<User> GetAll();

    /// <summary>
    /// Retrieves a specific user by their ID.
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <returns>The user if found, otherwise null</returns>
    User? GetById(int id);

    /// <summary>
    /// Adds a new user to the data store.
    /// </summary>
    /// <param name="user">The user to add</param>
    /// <returns>The added user with generated ID</returns>
    User Add(User user);

    /// <summary>
    /// Updates an existing user in the data store.
    /// </summary>
    /// <param name="user">The user with updated data</param>
    /// <returns>True if update succeeded, false if user not found</returns>
    bool Update(User user);

    /// <summary>
    /// Deletes a user from the data store.
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <returns>True if deletion succeeded, false if user not found</returns>
    bool Delete(int id);

    /// <summary>
    /// Checks if a user with the given email already exists.
    /// </summary>
    /// <param name="email">The email to check</param>
    /// <param name="excludeUserId">Optional user ID to exclude from the check (for updates)</param>
    /// <returns>True if email exists, false otherwise</returns>
    bool EmailExists(string email, int? excludeUserId = null);
}
