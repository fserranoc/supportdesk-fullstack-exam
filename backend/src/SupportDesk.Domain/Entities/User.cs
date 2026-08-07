using SupportDesk.Domain.Exceptions;

namespace SupportDesk.Domain.Entities;

public sealed class User
{
    private User()
    {
    }

    public User(Guid id, string email, string displayName)
    {
        if (id == Guid.Empty)
        {
            throw new DomainValidationException(nameof(id), "El identificador del usuario es obligatorio.");
        }

        Email = RequireText(email, nameof(email), 254);
        DisplayName = RequireText(displayName, nameof(displayName), 120);
        Id = id;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;

    private static string RequireText(string value, string field, int maximumLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length == 0 || normalized.Length > maximumLength)
        {
            throw new DomainValidationException(field, $"{field} es obligatorio y admite hasta {maximumLength} caracteres.");
        }

        return normalized;
    }
}
