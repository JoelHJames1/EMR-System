using EMRDataLayer.DataContext;
using EMRDataLayer.Repository.IRepository;
using EMRDataLayer.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EMRWebAPI.Services.IServices;
using EMRWebAPI.Services;
using EMRDataLayer.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

// Configure Identity
services.AddIdentity<User, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<EMRDBContext>()
.AddDefaultTokenProviders();

// Configure Database
services.AddDbContext<EMRDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Repository Here - Generic Repository
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Add Specific Repositories
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IAddressRepository, AddressRepository>();
services.AddScoped<IPatientRepository, PatientRepository>();
services.AddScoped<IAppointmentRepository, AppointmentRepository>();
services.AddScoped<IEncounterRepository, EncounterRepository>();
services.AddScoped<IDiagnosisRepository, DiagnosisRepository>();
services.AddScoped<IProcedureRepository, ProcedureRepository>();
services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
services.AddScoped<IMedicationRepository, MedicationRepository>();
services.AddScoped<ILabOrderRepository, LabOrderRepository>();
services.AddScoped<ILabResultRepository, LabResultRepository>();
services.AddScoped<IAllergyRepository, AllergyRepository>();
services.AddScoped<IImmunizationRepository, ImmunizationRepository>();
services.AddScoped<IObservationRepository, ObservationRepository>();
services.AddScoped<IClinicalNoteRepository, ClinicalNoteRepository>();
services.AddScoped<ICarePlanRepository, CarePlanRepository>();
services.AddScoped<IReferralRepository, ReferralRepository>();
services.AddScoped<IProviderRepository, ProviderRepository>();
services.AddScoped<IBillingRepository, BillingRepository>();
services.AddScoped<IInsuranceRepository, InsuranceRepository>();

// Add Services Here
services.AddScoped<IUserService, UserService>();
services.AddScoped<JwtService>();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "EMRSystem";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "EMRSystemUsers";

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Authorization
services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("DoctorOnly", policy => policy.RequireRole("Doctor", "Administrator"));
    options.AddPolicy("NurseOnly", policy => policy.RequireRole("Nurse", "Doctor", "Administrator"));
    options.AddPolicy("LabTechOnly", policy => policy.RequireRole("Lab Technician", "Doctor", "Administrator"));
    options.AddPolicy("BillingOnly", policy => policy.RequireRole("Billing Staff", "Administrator"));
});

services.AddControllers();
services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EMR System API",
        Version = "v1",
        Description = "Comprehensive Electronic Medical Records System API with JWT Authentication",
        Contact = new OpenApiContact
        {
            Name = "EMR System Support",
            Email = "support@emrsystem.com"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

services.AddAutoMapper(typeof(Program));

// Add CORS
services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add NLog logging
builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EMR System API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
