using ApiFootball.Extensions;
using LuckyShot.API.Services;
using LuckyShot.Domain.Services;
using LuckyShot.Infrastructure.ExternalServices;
using LuckyShot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using LuckyShot.Infrastructure.Data;

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
builder.Services.AddDbContext<CompetitionDataContext>(options =>
{
    options.UseNpgsql(connectionString!, b =>
    {
        b.MigrationsHistoryTable("__EFMigrationsHistory", CompetitionDataContext.DatabaseSchema);
    });
});
builder.Services.AddApiFootballServices(builder.Configuration);
builder.Services.AddScoped<CompetitionRepository>();
builder.Services.AddScoped<MatchRepository>();
builder.Services.AddScoped<SeasonRepository>();
builder.Services.AddScoped<TeamRepository>();
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