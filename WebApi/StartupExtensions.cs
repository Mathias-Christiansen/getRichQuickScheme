using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using Contracts.Errors;
using Contracts.Errors.Interfaces;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

public static class StartupExtensions
{
    public static async Task RunMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (dbContext.Database.IsSqlite())
            {
                await dbContext.Database.MigrateAsync();
            }
            else
            {
                throw new ApplicationException("Database is not sqlite, could not migrate lmao");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static void HandleFluentValidationException(this WebApplication app)
    {
        app.UseExceptionHandler(x =>
        {
            x.Run(async context =>
            {
                var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
                var exception = exceptionHandler?.Error;
                if (exception is null) return;
                if (exception is not ValidationException ve) throw exception;
                var errors = ve.Errors
                    .Select(y => new GenericError(y.ErrorMessage))
                    .Cast<IError>()
                    .ToList();
                var jsonE = JsonSerializer.Serialize(errors);
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(jsonE);

            });
        });
    }
    
    public static void HandleAuthorizationException(this WebApplication app)
    {
        app.UseExceptionHandler(x =>
        {
            x.Run(async context =>
            {
                var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
                var exception = exceptionHandler?.Error;
                if (exception is null) return;
                if (exception is not AuthenticationException ve) throw exception;
                var errors = (IError)new GenericError(ve.Message);
                var jsonE = JsonSerializer.Serialize(errors);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(jsonE);

            });
        });
    }
}