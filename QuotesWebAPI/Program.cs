using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using QuotesWebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add DB context
builder.Services.AddDbContext<QuotesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuotesDB"))
);

// Add controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
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

// Serve Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Set up SPA default static file routing from QuotesSPA folder
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

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();
