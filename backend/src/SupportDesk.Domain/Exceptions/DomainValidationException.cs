namespace SupportDesk.Domain.Exceptions;

public sealed class DomainValidationException : Exception
{
    public DomainValidationException(string field, string message) : base(message)
    {
        Field = field;
    }

    public string Field { get; }
}
