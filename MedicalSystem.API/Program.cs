using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MedicalSystem.Infrastructure.Data;
using MedicalSystem.Infrastructure.Repositories;
using MedicalSystem.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Register File Service
builder.Services.AddScoped<IFileService, LocalFileService>();

// Configure PostgreSQL with connection string from appsettings.json
builder.Services.AddDbContext<MedicalSystemContext>(options =>
    options.UseLazyLoadingProxies() // Enable lazy loading
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repository Factory
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();