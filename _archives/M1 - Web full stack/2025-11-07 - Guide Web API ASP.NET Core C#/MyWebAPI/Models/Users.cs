using System.ComponentModel.DataAnnotations;

namespace MyWebAPI.Models;

/// <summary>
/// Represents a user in the system.
/// Contains user identification, contact information, and metadata.
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Full name of the user.
    /// </summary>
    /// <example>John Doe</example>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the user.
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Age of the user in years.
    /// </summary>
    /// <example>25</example>
    [Required(ErrorMessage = "Age is required")]
    [Range(1, 150, ErrorMessage = "Age must be between 1 and 150")]
    public int Age { get; set; }

    /// <summary>
    /// UTC timestamp when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp when the user was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
