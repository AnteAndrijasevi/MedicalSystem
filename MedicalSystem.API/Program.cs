using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MedicalSystem.Infrastructure.Data;
using MedicalSystem.Infrastructure.Repositories;
using MedicalSystem.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

try
{
    Console.WriteLine("Starting application configuration...");

    // Add services to the container
    builder.Services.AddControllers();

    // Register File Service
    builder.Services.AddScoped<IFileService, LocalFileService>();

    Console.WriteLine("Configuring database...");

    // Configure PostgreSQL
    builder.Services.AddDbContext<MedicalSystemContext>(options =>
        options.UseLazyLoadingProxies()
        .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    Console.WriteLine("Registering repositories...");

    // Register Repository Factory
    builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
    });

    // Add Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    Console.WriteLine("Building application...");
    var app = builder.Build();

    Console.WriteLine("Configuring middleware...");

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Test endpoints
    app.MapGet("/", () => "Medical System API is running!");
    app.MapGet("/api/test", () => "API Test successful!");

    //app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseAuthorization();
    app.MapControllers();

    Console.WriteLine("Testing database connection...");

    /*
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<MedicalSystemContext>();
        try
        {
            await context.Database.CanConnectAsync();
            Console.WriteLine("Database connection successful!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database connection failed: {ex.Message}");
            // Ne prekidamo aplikaciju zbog database problema
        }
    }
    */

    Console.WriteLine("Starting application...");
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Application startup failed: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}