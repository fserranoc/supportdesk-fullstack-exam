namespace SupportDesk.Domain.Exceptions;

public sealed class BusinessConflictException : Exception
{
    public BusinessConflictException(string message) : base(message)
    {
    }
}
