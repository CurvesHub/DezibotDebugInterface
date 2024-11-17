using DezibotDebugInterface.Api.Broadcast;
using DezibotDebugInterface.Api.Broadcast.DezibotHubs;
using DezibotDebugInterface.Api.Common.DataAccess;
using DezibotDebugInterface.Api.GetDezibots;

using Microsoft.AspNetCore.Diagnostics;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IDezibotRepository, DezibotRepositoryInMemory>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.Map("/", () => Results.Redirect("/scalar/v1"));
app.Map("/api", () => Results.Redirect("/scalar/v1"));

app.MapGetDezibotEndpoints();
app.MapBroadcastEndpoint();

app.MapHub<DezibotHub>("/dezibot-hub");

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext context) => Results.Problem(
    detail: context.Features.Get<IExceptionHandlerFeature>()?.Error.Message ?? "An error occurred.",
    statusCode: 500));

app.Run();

public partial class Program;