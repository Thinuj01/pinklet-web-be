using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using pinklet.data;
using pinklet.Models;
using System.Linq;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); 
    });
});
builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    //options.UseSqlServer(
    //    builder.Configuration.GetConnectionString("DefaultConnection"),
    //    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
    //        maxRetryCount: 5,
    //        maxRetryDelay: TimeSpan.FromSeconds(10),
    //        errorNumbersToAdd: null
    //    )
    //)
    options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorCodesToAdd: null
    )
    )
);

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
if (builder.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("GENERATE_SWAGGER") == "true")
{
    builder.WebHost.UseUrls("http://localhost:5000");
}
//builder.WebHost.UseUrls("http://0.0.0.0:5000");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CloudinaryService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


//if (app.Environment.IsDevelopment())
//{
//    app.UseHttpsRedirection();
//}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pinklet API V1");
    c.RoutePrefix = "swagger"; // or "" if you want it at root
});

//app.UseHttpsRedirection();

app.MapControllers();

app.Run();
