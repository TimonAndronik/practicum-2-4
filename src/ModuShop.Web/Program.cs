using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using ModuShop.Users;
using Serilog;
using ModuShop.Catalog;

var logger = Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, config) =>
    config.ReadFrom.Configuration(builder.Configuration));

builder.AddServiceDefaults();

builder.Services.AddFastEndpoints()
    .AddAuthenticationJwtBearer(s =>
    {
        s.SigningKey = builder.Configuration["Auth:JwtSecret"];
    })
    .AddAuthorization()
    .SwaggerDocument();

builder.AddUsersModuleServices(logger);
builder.AddCatalogModuleServices(logger);
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints()
   .UseSwaggerGen();

await app.EnsureUsersModuleDatabaseAsync();
await app.EnsureCatalogModuleDatabaseAsync();
app.Run();