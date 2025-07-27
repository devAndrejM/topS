using Microsoft.EntityFrameworkCore;
using ClothingSearch.Api.Data;
using ClothingSearch.Api.Interfaces;
using ClothingSearch.Api.Services;
using ClothingSearch.Api.Providers;

Console.WriteLine("🚀 === CLOTHINGSEARCH API STARTUP === 🚀");
Console.WriteLine($"⏰ Startup Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
Console.WriteLine($"🌍 Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine("📦 Basic services configured (Controllers, Swagger)");

// Database - Use In-Memory for development if PostgreSQL not available
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", false);

Console.WriteLine($"🔍 Connection String: {connectionString ?? "NULL"}");
Console.WriteLine($"🔍 UseInMemory Flag: {useInMemory}");

if (useInMemory || string.IsNullOrEmpty(connectionString) || connectionString.Contains(":memory:"))
{
    Console.WriteLine("✅ Using In-Memory Database for development");
    builder.Services.AddDbContext<ClothingSearchContext>(options =>
        options.UseInMemoryDatabase("ClothingSearchDb"));
}
else
{
    Console.WriteLine("✅ Using PostgreSQL Database");
    builder.Services.AddDbContext<ClothingSearchContext>(options =>
        options.UseNpgsql(connectionString));
}

// Services
Console.WriteLine("🔧 Registering services...");
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IUserService, UserService>();
Console.WriteLine("✅ SearchService and UserService registered");

// Store Providers
Console.WriteLine("🏪 Registering store providers...");
builder.Services.AddHttpClient<HervisProvider>();
builder.Services.AddScoped<IStoreProvider, AmazonProvider>();
builder.Services.AddScoped<IStoreProvider, HervisProvider>();
Console.WriteLine("✅ AmazonProvider and HervisProvider registered");

// CORS - FIXED: More permissive for development
Console.WriteLine("🌐 Configuring CORS...");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:8100",
                    "https://localhost:8100",
                    "http://127.0.0.1:8100",
                    "https://127.0.0.1:8100",
                    "capacitor://localhost",
                    "ionic://localhost"
                  )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .SetIsOriginAllowedToAllowWildcardSubdomains();
        });

    // Add a development policy that's more permissive
    options.AddPolicy("Development",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
Console.WriteLine("✅ CORS policies configured");

var app = builder.Build();

Console.WriteLine("🏗️ Application built, configuring middleware...");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("🔧 Development mode - enabling Swagger and permissive CORS");
    app.UseSwagger();
    app.UseSwaggerUI();

    // Use more permissive CORS in development
    app.UseCors("Development");
}
else
{
    Console.WriteLine("🔧 Production mode - using restricted CORS");
    app.UseCors("AllowFrontend");
}

Console.WriteLine("🔐 Configuring HTTPS redirection and authorization...");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("🗺️ Controllers mapped");

// Auto-migrate/seed database on startup with error handling
Console.WriteLine("💾 Setting up database...");
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ClothingSearchContext>();
    try
    {
        if (context.Database.IsInMemory())
        {
            // Seed in-memory database
            context.Database.EnsureCreated();
            Console.WriteLine("✅ In-memory database created and seeded successfully.");

            // Count seeded data
            var countryCount = context.Countries?.Count() ?? 0;
            var storeCount = context.Stores?.Count() ?? 0;
            Console.WriteLine($"📊 Database contains: {countryCount} countries, {storeCount} stores");
        }
        else
        {
            // Try to migrate PostgreSQL
            context.Database.Migrate();
            Console.WriteLine("✅ PostgreSQL database migration completed successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database setup failed: {ex.Message}");
        Console.WriteLine($"❌ Exception type: {ex.GetType().Name}");
        Console.WriteLine("⚠️ The API will still run, but database features may not work.");
    }
}

Console.WriteLine("");
Console.WriteLine("🎉 === CLOTHINGSEARCH API READY === 🎉");
Console.WriteLine("📍 URLs will be shown below by ASP.NET Core...");
Console.WriteLine("📍 Expected Swagger: Check the 'Now listening on' URLs + /swagger");
Console.WriteLine("📍 Expected API Base: Check the 'Now listening on' URLs + /api");
Console.WriteLine($"🌐 CORS: Allowing ALL origins in development mode");
Console.WriteLine($"🔧 Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("");

// The app.Run() call will show the actual ports
app.Run();