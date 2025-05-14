using data.RemoteData.RemoteDataBase;
using data.Repository;
using domain.UseCase;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RemoteDatabaseContext>();

builder.Services.AddScoped<IUserRepository, SQLUserRepositoryImpl>()
                .AddScoped<IPresenceRepository, SQLPresenceRepositoryImpl>()
                .AddScoped<IGroupRepository, SQLGroupRepositoryImpl>()
                .AddScoped<PresenceUseCase>()
                .AddScoped<GroupUseCase>()
                .AddScoped<UserUseCase>()
                .AddScoped<APIUseCase>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole(options =>
    {
        options.LogToStandardErrorThreshold = LogLevel.Error;
    });
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Error);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
public class ErrorLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorLoggingMiddleware> _logger;

    public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (context.Response.StatusCode >= 400)
            {
                var logLevel = context.Response.StatusCode >= 500 ? LogLevel.Error : LogLevel.Warning;

                _logger.Log(logLevel, "Ошибка на сервере: {StatusCode} {Method} {Path}",
                    context.Response.StatusCode, context.Request.Method, context.Request.Path);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка в запросе {Method} {Path}: {Message}",
                context.Request.Method, context.Request.Path, ex.Message);

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Произошла ошибка на сервере.");
        }
    }
}

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("bad-request")]
    public IActionResult BadRequestExample()
    {
        return BadRequest("Вот пример ошибки 400.");
    }

    [HttpGet("internal-error")]
    public IActionResult InternalServerErrorExample()
    {
        throw new InvalidOperationException("Вот пример ошибки 500.");
    }
}
