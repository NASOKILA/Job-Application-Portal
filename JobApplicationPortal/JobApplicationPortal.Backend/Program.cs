using JobApplicationPortal.Backend.API.Mappers;
using JobApplicationPortal.DB;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddAutoMapper(typeof(ApplicantMappingProfile));

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
