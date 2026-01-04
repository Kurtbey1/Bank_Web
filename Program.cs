using Bank_Project.Data;
using Bank_Project.Services;
using Bank_Project.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();

// 2. Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Register Services (Cleaned & Optimized)
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<ICustomerValidatorService, CustomerValidatorService>();
builder.Services.AddScoped<ILoanValidator, LoanValidator>();

// 2. تسجيل الواجهات (تأكد أن ICustomerServices و CustomerServices في نفس الـ Namespace)
builder.Services.AddScoped<ICustomerServices, CustomerServices>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// 3. تسجيل الخدمات المعقدة (التي تعتمد على ما سبق)
builder.Services.AddScoped<BankCoordinatorService>();
builder.Services.AddScoped<SignUpService>();
builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<EmployeeServices>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token only."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 4. JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsAVerySecretKeyForBankProject2025!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "BankApi",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "BankUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var app = builder.Build();

// 5. Database Auto-Migration (The Professional Way)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // استبدلنا EnsureCreated بـ Migrate لحل مشكلة الجداول الناقصة
        context.Database.Migrate();
        Console.WriteLine(">>> Database Migrated and Synced Successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($">>> DATABASE ERROR: {ex.Message}");
    }
}

// 6. Middlewares Order (Crucial!)
// حذفنا شرط IsDevelopment مؤقتاً لضمان فتح Swagger تحت أي ظرف
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank API v1");
    c.RoutePrefix = "swagger"; // سيعمل الآن على رابط /swagger
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAnyOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();