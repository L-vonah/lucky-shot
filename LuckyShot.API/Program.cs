using ApiFootball.Extensions;
using LuckyShot.Domain.Services;
using LuckyShot.Infrastructure;
using LuckyShot.Infrastructure.ExternalServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using LuckyShot.API.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy =>
        {
            policy.WithOrigins("http://localhost:7212")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lucky Shot API", Version = "v1" });
});

// Add services to the container.
builder.Services.AddDbContext<LuckyShotContext>(options =>
{
    options.UseSqlite(connectionString!);
});
builder.Services.AddApiFootballServices(builder.Configuration);
builder.Services.AddScoped<ICompetitionInfoProvider, ApiFootballService>();
builder.Services.AddScoped<ICompetitionSyncService, CompetitionSyncService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowLocalhost");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lucky Shot API v1");
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();