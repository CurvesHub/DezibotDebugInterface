using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.Endpoints.GetDezibots;
using DezibotDebugInterface.Api.Endpoints.UpdateDezibot;
using DezibotDebugInterface.Api.SignalRHubs;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DezibotSQLite")
    ?? throw new InvalidOperationException("The connection string 'DezibotSQLite' was not found.");

builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddDbContext<DezibotDbContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.Map("/", () => Results.Redirect("/scalar/v1"));
app.Map("/api", () => Results.Redirect("/scalar/v1"));

app.MapGetDezibotEndpoints();
app.MapUpdateDezibotEndpoint();
app.MapHub<DezibotHub>("/dezibot-hub");

// TODO: On app start, check if the database is migrated and if not, migrate it

if (app.Environment.IsDevelopment())
{
    app.Map("/api/resetDatabase", async (DezibotDbContext dbContext) =>
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        return Results.NoContent();
    }).WithOpenApi();
    
    app.MapDelete("/api/dezibot/{ip}", async (DezibotDbContext dbContext, string ip) =>
    {
        var dezibot = await dbContext.Dezibots.FindAsync(ip);
        if (dezibot is null)
        {
            return Results.NotFound();
        }

        dbContext.Dezibots.Remove(dezibot);
        await dbContext.SaveChangesAsync();

        return Results.NoContent();
    }).WithOpenApi();
}

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext context) =>
{
    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;

    if (ex is null)
    {
        return Results.Problem(detail: "An error occurred.", statusCode: 500);
    }

    if (ex.InnerException is null)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
    
    var message = ex.Message;
    
    var innerEx = ex.InnerException;
    while (innerEx != null)
    {
        ex = innerEx;
        innerEx = ex.InnerException;
    }
    
    return Results.Problem(detail: message + $" Detailed Error: {ex.Message}", statusCode: 500);
});

app.Run();

namespace DezibotDebugInterface.Api
{
    public partial class Program;
}