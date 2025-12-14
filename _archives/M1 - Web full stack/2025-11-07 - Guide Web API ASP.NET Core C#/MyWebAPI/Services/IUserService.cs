using MyWebAPI.Models;

namespace MyWebAPI.Services;

/// <summary>
/// Interface for User business logic operations.
/// Defines the contract for user service implementations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <returns>A list of all users</returns>
    IEnumerable<User> GetAllUsers();

    /// <summary>
    /// Retrieves a specific user by ID.
    /// </summary>
    /// <param name="id">The user ID</param>
    /// <returns>The user if found, otherwise null</returns>
    User? GetUserById(int id);

    /// <summary>
    /// Creates a new user with validation.
    /// </summary>
    /// <param name="user">The user to create</param>
    /// <returns>Result containing the created user or error message</returns>
    ServiceResult<User> CreateUser(User user);

    /// <summary>
    /// Updates an existing user with validation.
    /// </summary>
    /// <param name="id">The ID of the user to update</param>
    /// <param name="user">The updated user data</param>
    /// <returns>Result indicating success or failure with message</returns>
    ServiceResult<User> UpdateUser(int id, User user);

    /// <summary>
    /// Deletes a user by ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <returns>Result indicating success or failure with message</returns>
    ServiceResult<bool> DeleteUser(int id);

    /// <summary>
    /// Calculates the average age of all users.
    /// </summary>
    /// <returns>The average age as a double, or 0 if no users exist</returns>
    double GetAverageAge();
}

/// <summary>
/// Generic result wrapper for service operations.
/// Encapsulates success/failure status, data, and error messages.
/// </summary>
/// <typeparam name="T">The type of data returned on success</typeparam>
public class ServiceResult<T>
{
    /// <summary>
    /// Indicates whether the operation succeeded.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The data returned by the operation (null if failed).
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error message if operation failed (null if succeeded).
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates a successful result with data.
    /// </summary>
    public static ServiceResult<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static ServiceResult<T> Fail(string errorMessage) => new()
    {
        Success = false,
        ErrorMessage = errorMessage
    };
}
