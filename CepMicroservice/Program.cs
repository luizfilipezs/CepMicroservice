using CepMicroservice.Contracts.Services.Interfaces;
using CepMicroservice.Data;
using CepMicroservice.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<ICorreiosApiService, CorreiosApiService>();
builder.Services.AddScoped<IAdressService, AddressService>();

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseInMemoryDatabase("TestDatabase");
    });
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=cepdb.sqlite"));
}

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

public partial class Program { }
