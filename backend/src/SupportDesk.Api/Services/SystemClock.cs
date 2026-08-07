using SupportDesk.Application.Abstractions;

namespace SupportDesk.Api.Services;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
