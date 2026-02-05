using ApiFootball.Extensions;
using LuckyShot.API.Services;
using LuckyShot.Domain.Entities;
using LuckyShot.Domain.Services;
using LuckyShot.Infrastructure.Data;
using LuckyShot.Infrastructure.ExternalServices;
using LuckyShot.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

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
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add services to the container.
builder.Services.AddDbContext<CompetitionDataContext>(options =>
{
    options.UseNpgsql(connectionString!, b =>
    {
        b.MigrationsHistoryTable("__EFMigrationsHistory", CompetitionDataContext.DatabaseSchema);
    });
});
builder.Services.AddDbContext<AuthContext>(options =>
{
    options.UseNpgsql(connectionString!, b =>
    {
        b.MigrationsHistoryTable("__EFMigrationsHistory", AuthContext.DatabaseSchema);
    });
});

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var publicKeyPem = builder.Configuration["Jwt:PublicKeyPem"];
if (string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException("Jwt:Issuer and Jwt:Audience must be configured.");
}
if (string.IsNullOrWhiteSpace(publicKeyPem))
{
    throw new InvalidOperationException("Jwt:PublicKeyPem must be configured.");
}
var rsa = RSA.Create();
rsa.ImportFromPem(publicKeyPem);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddApiFootballServices(builder.Configuration);
builder.Services.AddScoped<CompetitionRepository>();
builder.Services.AddScoped<MatchRepository>();
builder.Services.AddScoped<SeasonRepository>();
builder.Services.AddScoped<TeamRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccessTokenIssuer, JwtService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ICompetitionInfoProvider, ApiFootballService>();
builder.Services.AddScoped<ICompetitionSyncService, CompetitionSyncService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowLocalhost");
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lucky Shot API v1");
        c.InjectJavascript("/swagger-auth.js");
        c.InjectStylesheet("/swagger-auth.css");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();