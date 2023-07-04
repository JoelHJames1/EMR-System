using EMRDataLayer.DataContext;
using EMRDataLayer.Repository.IRepository;
using EMRDataLayer.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using EMRWebAPI.Services.IServices;
using EMRWebAPI.Services;
using EMRDataLayer.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Generate a unique key for signing the tokens
var key = new byte[32];
RandomNumberGenerator.Create().GetBytes(key);
var signingKey = Convert.ToBase64String(key);

// Set issuer and audience
string issuer = "http://www.3dlogico.com";
string audience = "http://www.3dlogico.com";

// Add services to the container.
var services = builder.Services;

services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<EMRDBContext>();

services.AddDbContext<EMRDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Repository Here
services.AddScoped<IRepository<User>, UserRepository>();
services.AddScoped<IRepository<Address>, AddressRepository>();

services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IAddressRepository, AddressRepository>();

// Add Services Here
services.AddScoped<IUserService, UserService>();

// Add JWT Bearer authentication
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
        };
    });

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddAutoMapper(typeof(Program));

// Add CORS
services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder.WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add NLog logging to the container
services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
    loggingBuilder.AddNLog();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Use CORS
app.UseCors("AllowReactApp");

app.MapControllers();

app.Run();
