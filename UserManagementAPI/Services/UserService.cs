
using System.ComponentModel.DataAnnotations;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public class UserService : IUserService
{
    private readonly Dictionary<int, User> _users = new();
    private readonly HashSet<string> _emails =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly object _lock = new();
    private int _nextId = 1;

    public Task<IEnumerable<User>> GetAllUsersAsync()
    {
        lock (_lock)
        {
            IEnumerable<User> users = _users.Values
                .Select(CloneUser)
                .ToList();

            return Task.FromResult(users);
        }
    }

    public Task<User?> GetUserByIdAsync(int id)
    {
        lock (_lock)
        {
            User? user = _users.TryGetValue(id, out var found)
                ? CloneUser(found)
                : null;

            return Task.FromResult(user);
        }
    }

    public Task<User> CreateUserAsync(User user)
    {
        lock (_lock)
        {
            ValidateUser(user);

            string email = user.Email.Trim();

            if (_emails.Contains(email))
                throw new InvalidOperationException(
                    "A user with this email already exists.");

            if (_nextId == int.MaxValue)
                throw new InvalidOperationException(
                    "No more user IDs are available.");

            var newUser = CloneUser(user);
            newUser.Id = _nextId++;
            newUser.Email = email;

            _users.Add(newUser.Id, newUser);
            _emails.Add(email);

            return Task.FromResult(CloneUser(newUser));
        }
    }

    public Task<bool> UpdateUserAsync(int id, User user)
    {
        lock (_lock)
        {
            if (!_users.TryGetValue(id, out var existing))
                return Task.FromResult(false);

            ValidateUser(user);

            string newEmail = user.Email.Trim();

            if (!string.Equals(
                    existing.Email,
                    newEmail,
                    StringComparison.OrdinalIgnoreCase)
                && _emails.Contains(newEmail))
            {
                throw new InvalidOperationException(
                    "A user with this email already exists.");
            }

            var updated = CloneUser(user);
            updated.Id = id;
            updated.Email = newEmail;

            _users[id] = updated;

            _emails.Remove(existing.Email);
            _emails.Add(newEmail);

            return Task.FromResult(true);
        }
    }

    public Task<bool> DeleteUserAsync(int id)
    {
        lock (_lock)
        {
            if (!_users.Remove(id, out var removed))
                return Task.FromResult(false);

            _emails.Remove(removed.Email);

            return Task.FromResult(true);
        }
    }

    private static void ValidateUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrWhiteSpace(user.FirstName) ||
            string.IsNullOrWhiteSpace(user.LastName) ||
            string.IsNullOrWhiteSpace(user.Email))
        {
            throw new ArgumentException(
                "First name, last name, and email are required.");
        }

        if (user.FirstName.Length > 100 ||
            user.LastName.Length > 100 ||
            user.Email.Length > 254 ||
            (user.Department?.Length ?? 0) > 100)
        {
            throw new ArgumentException(
                "One or more fields exceed the allowed length.");
        }

        if (!new EmailAddressAttribute()
            .IsValid(user.Email.Trim()))
        {
            throw new ArgumentException(
                "A valid email address is required.");
        }
    }

    private static User CloneUser(User user)
    {
        return new User
        {
            Id = user.Id,
            FirstName = user.FirstName.Trim(),
            LastName = user.LastName.Trim(),
            Email = user.Email.Trim(),
            Department = user.Department?.Trim() ?? string.Empty
        };
    }
}
