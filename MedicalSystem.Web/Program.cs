// MedicalSystem.Web/Program.cs - ISPRAVKA

using MedicalSystem.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register API Service
builder.Services.AddScoped<IApiService, ApiService>();

// Add HttpClient for API calls
builder.Services.AddHttpClient("MedicalSystemAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5226/"); // API port
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// **KLJUČNO: MVC ROUTING KONFIGURACIJA**
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// **DODAJTE SPECIFIČNE RUTE AKO TREBA**
app.MapControllerRoute(
    name: "patients",
    pattern: "Patients/{action=Index}/{id?}",
    defaults: new { controller = "Patients" });

// **DEBUG RUTE - dodajte za testiranje**
app.MapGet("/debug/routes", (IServiceProvider services) =>
{
    var routes = new List<string>
    {
        "/",
        "/Home",
        "/Home/Index",
        "/Patients",
        "/Patients/Index",
        "/Patients/Create"
    };
    return Results.Json(routes);
});

Console.WriteLine("🚀 Web application starting...");
Console.WriteLine($"🌐 Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"📡 API Base URL: http://localhost:5226/");

app.Run();