using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Net;
using System.Text.Json;

namespace NoteSystem.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Произошла неожиданная ошибка");

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new
        {
            Message = "Произошла ошибка при обработке запроса",
            Detail = exception.Message
        };

        switch (exception)
        {
            case InvalidOperationException ioe:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse = new
                {
                    Message = "Операция не может быть выполнена",
                    Detail = ioe.Message
                };
                break;
            case ArgumentNullException ane:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new
                {
                    Message = "Неверный запрос",
                    Detail = ane.Message
                };
                break;
            case DbUpdateException dbEx when (dbEx.InnerException is PostgresException pgEx && pgEx.SqlState == "23503"):
                response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse = new
                {
                    Message = "Нарушение ограничения базы данных",
                    Detail = "Нельзя удалить запись, так как на нее есть ссылки в других таблицах"
                };
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var result = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(result);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}