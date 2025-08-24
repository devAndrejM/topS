using ClothingSearch.Api.Interfaces;
using ClothingSearch.Api.Services;
using ClothingSearch.Api.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<HervisProvider>();
builder.Services.AddTransient<IStoreProvider, HervisProvider>();
builder.Services.AddTransient<IStoreProvider, AmazonProvider>();
builder.Services.AddTransient<IProductService, ProductService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:8100", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => new { Message = "ClothingSearch API is running!" });

app.Run();
