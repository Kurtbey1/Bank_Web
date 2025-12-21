using Bank_Project.Data;
using Bank_Project.Services;
using Bank_Project.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -----------------------
// Controllers (API only)
// -----------------------
builder.Services.AddControllers();

// -----------------------
// Database
// -----------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// -----------------------
// Dependency Injection
// -----------------------
builder.Services.AddScoped<ICustomerValidatorService, CustomerValidatorService>();
builder.Services.AddScoped<EmployeeServices>();
builder.Services.AddScoped<CustomerServices>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<ILoanValidator, LoanValidator>();
builder.Services.AddScoped<SignUpService>();
builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<BankCoordinatorService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();

// -----------------------
// Swagger (Dev only)
// -----------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bank API",
        Version = "v1"
    });
});

// -----------------------
// CORS Configuration
// -----------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policy =>
    {
        policy.AllowAnyOrigin()  // Allows requests from any origin
              .AllowAnyMethod()  // Allows any HTTP method (GET, POST, etc.)
              .AllowAnyHeader(); // Allows any headers
    });
});

// -----------------------
// JWT Configuration
// -----------------------
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey) ||
    string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException("JWT configuration is missing in appsettings.json");
}

// -----------------------
// Authentication
// -----------------------
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

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            )
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                return Task.CompletedTask;
            }
        };
    });

// -----------------------
// Build App
// -----------------------
var app = builder.Build();

// -----------------------
// Middleware
// -----------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank API v1");
        c.RoutePrefix = string.Empty;  // This makes Swagger the default page
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAnyOrigin"); // Apply CORS policy

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();