using Microsoft.EntityFrameworkCore;
using Pinewood.Customers.Db;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string dbConnectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddDbContext<JobApplicationPortalDbContext>(options => options.UseSqlServer(dbConnectionString));

builder.Services.AddDbContext<JobApplicationPortalDbContext>(options =>
    options.UseSqlServer(dbConnectionString));

builder.Services.AddScoped<DatabaseInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
