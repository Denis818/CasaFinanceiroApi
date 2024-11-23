using Application.Helpers;
using AspNetCoreRateLimit;
using Web.Extensios.Application;
using Web.Extensios.DependencyManagers;
using Web.Extensios.Swagger;
using Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureApplication();

var app = builder.Build();

ServiceLocator.Configure(app.Services);

app.ConfigureSwaggerUI();

app.UseCorsPolicy(builder.Environment);

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<IdentificadorDataBaseMiddleware>();

app.UseIpRateLimiting();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapGet(
    "/{*path}",
    async context =>
    {
        context.Response.Redirect("/doc");
        await Task.CompletedTask;
    }
);

app.Run();
