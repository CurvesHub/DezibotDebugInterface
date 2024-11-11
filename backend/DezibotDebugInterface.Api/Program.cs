using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.Endpoints;
using Microsoft.AspNetCore.Diagnostics;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IDezibotRepository, DezibotInMemoryRepository>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.Map("/", () => Results.Redirect("/scalar/v1"));
app.Map("/api", () => Results.Redirect("/scalar/v1"));

app.MapGetDezibotEndpoints();
app.MapPutDezibotEndpoints();

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext context) => Results.Problem(
    detail: context.Features.Get<IExceptionHandlerFeature>()?.Error.Message ?? "An error occurred.",
    statusCode: 500));

app.Run();
