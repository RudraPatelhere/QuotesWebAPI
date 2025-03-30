using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using QuotesWebAPI;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure EF Core with SQL Server
builder.Services.AddDbContext<QuotesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuotesDB"))
);

// ✅ Configure controllers with circular reference support
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ignores loops during JSON serialization (fixes circular reference errors)
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ✅ Enable Swagger (for testing API endpoints)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Allow all CORS for development use
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ Swagger UI available only in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Serve static frontend files from QuotesSPA folder
var spaPath = Path.Combine(Directory.GetCurrentDirectory(), "QuotesSPA");

app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = new PhysicalFileProvider(spaPath),
    RequestPath = ""
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(spaPath),
    RequestPath = ""
});

// ✅ Middleware setup
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// ✅ Map controller routes
app.MapControllers();

app.Run();
