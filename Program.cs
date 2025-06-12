using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Vedect.Data;
using Vedect.Models.Domain;
using Vedect.Repositories.Implementations;
using Vedect.Repositories.Interfaces;
using Vedect.Services.BackgroundServices;
using Vedect.Services.Core;
using Vedect.Services.Implementations;
using Vedect.Services.Interfaces;
using Vedect.Shared;

var builder = WebApplication.CreateBuilder(args);

// Initialize Firebase Admin SDK
var firebaseKey = builder.Configuration["Firebase:ServiceAccountKey"];
if (string.IsNullOrEmpty(firebaseKey))
{
    throw new InvalidOperationException("Firebase Service Account Key is not set in the configuration. Please set the 'Firebase:ServiceAccountKey' user secret.");
}

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromJson(firebaseKey),
});

builder.Services.AddHttpClient();

builder.Services.AddRazorPages();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VdectDbConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddScoped<ICameraRepo, CameraRepo>();


builder.Services.AddHttpClient();

builder.Services.AddScoped<JWTService>();

builder.Services.Configure<List<AdminAccount>>(builder.Configuration.GetSection("AdminAccounts"));

builder.Services.AddSession();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});


builder.Services.AddTransient<IEmailSender, EmailSender>(); 
builder.Services.AddTransient<IUserRegistrationService, UserRegistrationService>();
builder.Services.AddScoped<INotificationService, FirebaseNotificationService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Vedect API", Version = "v1" });

    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.  
                        Enter 'Bearer' [space] and then your token in the text input below.  
                        Example: 'Bearer abc123xyz'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2", // Required but not used for ApiKey type
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Add services to the container.
builder.Services.AddHttpClient("AiPipelineClient", client =>
{
    // Optional: Set a timeout
    // client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddSingleton<Vedect.Services.Interfaces.IAiProcessingService, Vedect.Services.Implementations.AiProcessingService>();

// Register the background service for consuming RabbitMQ notifications
builder.Services.AddHostedService<NotificationConsumerService>();

var app = builder.Build();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    await RoleSeeder.SeedRoles(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// UseDefaultFiles must be called before UseStaticFiles to serve the default file (e.g., index.html) for requests to the root URL.
app.UseDefaultFiles(); 
app.UseStaticFiles(); // This will serve files from wwwroot

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();

app.UseSession();

app.Run();
