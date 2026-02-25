using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MphokeToDoList.Api.Data;
using MphokeToDoList.Api.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ========================
// Add Services
// ========================

// Controllers with JSON options to avoid cycles
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ========================
// Swagger with JWT support
// ========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MphokeToDoList API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        Description = "Enter 'Bearer' followed by your JWT token.\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ========================
// EF Core SQL Server
// ========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========================
// JWT Authentication
// ========================
var jwtSecret = builder.Configuration["JwtSecret"] ?? "YourVeryStrongJWTSecretKey";
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("JWT Authentication Failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = context.Principal?.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine($"JWT Token Validated: UserId={userId}, Role={role}");
            return Task.CompletedTask;
        }
    };
});

// ========================
// CORS - ALLOW FRONTEND
// ========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ========================
// Build App
// ========================
var app = builder.Build();

// ========================
// Middleware
// ========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MphokeToDoList API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");  //  CORS MUST be BEFORE Authentication

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ========================
// Seed Admin User
// ========================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.Migrate();

    if (!context.Users.Any(u => u.Username == "mphoke@tippfocus.co.za"))
    {
        var adminPassword = "mphoke91@M";
        var admin = new User
        {
            Username = "mphoke@tippfocus.co.za",
            Role = "Admin",
            PasswordHash = Convert.ToBase64String(
                SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(adminPassword))
            )
        };
        context.Users.Add(admin);
        context.SaveChanges();

        Console.WriteLine("Admin user seeded: mphoke@tippfocus.co.za / " + adminPassword);
    }
}

app.Run();