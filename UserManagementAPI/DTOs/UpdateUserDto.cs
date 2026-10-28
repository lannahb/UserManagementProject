
using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.DTOs;

public class UpdateUserDto : IValidatableObject
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(FirstName))
            yield return new ValidationResult(
                "First name cannot be blank.",
                new[] { nameof(FirstName) });

        if (string.IsNullOrWhiteSpace(LastName))
            yield return new ValidationResult(
                "Last name cannot be blank.",
                new[] { nameof(LastName) });
    }
}
