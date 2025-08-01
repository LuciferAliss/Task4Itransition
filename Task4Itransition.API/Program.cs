using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Task4Itransition.API.Handlers;
using Task4Itransition.Application.Abstracts.Processor;
using Task4Itransition.Application.Abstracts.Services;
using Task4Itransition.Application.Mapping;
using Task4Itransition.Application.Services;
using Task4Itransition.Domain.Abstracts.Repository;
using Task4Itransition.Domain.Entities;
using Task4Itransition.Domain.Processors;
using Task4Itransition.Infrastructure.Data;
using Task4Itransition.Infrastructure.Options;
using Task4Itransition.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var url = $"http://0.0.0.0:{port}";

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.JWT_OPTIONS_KEY));

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", options =>
    {
        options.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://dynamic-granita-a926e2.netlify.app");
    });
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>(opt =>
{ 
    opt.Password.RequiredLength = 1;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireLowercase = false; 
    opt.Password.RequireUppercase = false;
    opt.Password.RequireDigit = false; 
    opt.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddDbContext<AppDbContext>((opt) =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DbContextString"));
});

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IAuthTokenProcessor, AuthTokenProcessor>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

var jwtOptions = builder.Configuration.GetSection("JwtOptions")
    .Get<JwtOptions>() ?? throw new InvalidOperationException("JwtOptions configuration is missing.");

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme; 
}).AddCookie().AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
    };

    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context => 
        {
            context.Token = context.Request.Cookies["ACCESS_TOKEN"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
}
app.MapOpenApi();
app.MapScalarApiReference(opt =>
{
    opt.WithTitle("JWT + Refresh Token Auth API");
});

app.UseCors("CorsPolicy");

app.UseExceptionHandler(_ => { });
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run(url);