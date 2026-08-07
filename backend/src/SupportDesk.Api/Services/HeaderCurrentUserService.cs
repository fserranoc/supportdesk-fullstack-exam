using SupportDesk.Application.Abstractions;

namespace SupportDesk.Api.Services;

public sealed class HeaderCurrentUserService : ICurrentUserService
{
    private const string HeaderName = "X-User";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public HeaderCurrentUserService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public string Email
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            var header = context?.Request.Headers[HeaderName].FirstOrDefault();
            var fallback = _configuration["CurrentUser:DevelopmentEmail"];
            var email = string.IsNullOrWhiteSpace(header) ? fallback : header;

            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@') || email.Length > 254)
            {
                throw new BadHttpRequestException($"Debe enviar un encabezado {HeaderName} válido.");
            }

            return email.Trim();
        }
    }
}
