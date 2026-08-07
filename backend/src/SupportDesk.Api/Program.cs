using System.Text.Json.Serialization;
using SupportDesk.Api.Middleware;
using SupportDesk.Api.Services;
using SupportDesk.Application.Abstractions;
using SupportDesk.Application.Tickets;
using SupportDesk.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlDocumentation = Path.Combine(AppContext.BaseDirectory, "SupportDesk.Api.xml");
    options.IncludeXmlComments(xmlDocumentation);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, HeaderCurrentUserService>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddInfrastructure(builder.Configuration);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    if (allowedOrigins.Length > 0)
    {
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    }
}));

var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}
app.UseCors();
app.MapControllers();
app.Run();

public partial class Program
{
}
