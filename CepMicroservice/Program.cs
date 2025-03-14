using CepMicroservice.Data;
using CepMicroservice.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<CorreiosApiService>();
builder.Services.AddScoped<AddressService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=cepdb.sqlite"));

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
