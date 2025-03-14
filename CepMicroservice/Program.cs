using CepMicroservice.Contracts.Services.Interfaces;
using CepMicroservice.Data;
using CepMicroservice.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<ICorreiosApiService, CorreiosApiService>();
builder.Services.AddScoped<IAdressService, AddressService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=cepdb.sqlite"));

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
