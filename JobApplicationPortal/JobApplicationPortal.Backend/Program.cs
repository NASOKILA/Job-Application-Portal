using FluentValidation;
using JobApplicationPortal.Backend.API.Mappers;
using JobApplicationPortal.Backend.Middlewares;
using JobApplicationPortal.DB;
using JobApplicationPortal.Models.Interfaces;
using JobApplicationPortal.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining<JobApplicantDtoValidator>();

string dbConnectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddDbContext<JobApplicationPortalDbContext>(options => options.UseSqlServer(dbConnectionString));

builder.Services.AddDbContext<JobApplicationPortalDbContext>(options =>
    options.UseSqlServer(dbConnectionString));

builder.Services.AddScoped<DatabaseInitializer>();

builder.Services.AddScoped<IBlobService, BlobService>();

builder.Services.AddAutoMapper(typeof(ApplicantMappingProfile));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var initializer = services.GetRequiredService<DatabaseInitializer>();
    initializer.Initialize();
}

app.Run();
