using MyWebAPI.Models;
using MyWebAPI.Repositories;

namespace MyWebAPI.Services;

/// <summary>
/// Implementation of user business logic.
/// Coordinates between the controller and repository layers.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    /// <param name="userRepository">The user repository instance</param>
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public IEnumerable<User> GetAllUsers()
    {
        return _userRepository.GetAll();
    }

    /// <inheritdoc />
    public User? GetUserById(int id)
    {
        return _userRepository.GetById(id);
    }

    /// <inheritdoc />
    public ServiceResult<User> CreateUser(User user)
    {
        // Business validation: Check for duplicate email
        if (_userRepository.EmailExists(user.Email))
        {
            return ServiceResult<User>.Fail("A user with this email already exists");
        }

        // Add user via repository
        var createdUser = _userRepository.Add(user);
        return ServiceResult<User>.Ok(createdUser);
    }

    /// <inheritdoc />
    public ServiceResult<User> UpdateUser(int id, User user)
    {
        // Validate ID match
        if (id != user.Id)
        {
            return ServiceResult<User>.Fail("ID mismatch between URL and request body");
        }

        // Check if user exists
        var existingUser = _userRepository.GetById(id);
        if (existingUser == null)
        {
            return ServiceResult<User>.Fail($"User with ID {id} not found");
        }

        // Business validation: Check for duplicate email (excluding current user)
        if (_userRepository.EmailExists(user.Email, id))
        {
            return ServiceResult<User>.Fail("Another user with this email already exists");
        }

        // Update via repository
        var success = _userRepository.Update(user);
        if (!success)
        {
            return ServiceResult<User>.Fail("Update failed");
        }

        return ServiceResult<User>.Ok(user);
    }

    /// <inheritdoc />
    public ServiceResult<bool> DeleteUser(int id)
    {
        var success = _userRepository.Delete(id);
        if (!success)
        {
            return ServiceResult<bool>.Fail($"User with ID {id} not found");
        }

        return ServiceResult<bool>.Ok(true);
    }

    /// <inheritdoc />
    /// <summary>
    /// Calculates the average age of all users.
    /// This is the NEW business logic method you requested!
    /// </summary>
    public double GetAverageAge()
    {
        var users = _userRepository.GetAll();
        
        // If no users exist, return 0
        if (!users.Any())
        {
            return 0;
        }

        // Calculate average age
        return users.Average(u => u.Age);
    }
}
