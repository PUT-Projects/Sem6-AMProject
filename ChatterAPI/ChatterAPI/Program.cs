using ChatterAPI.Entities;
using ChatterAPI.Middlewares;
using ChatterAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

namespace ChatterAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.WebHost.ConfigureKestrel((context, serverOptions) =>
        {
            var ip = new IPAddress(new byte[] { 192, 168, 1, 75 });
            serverOptions.Listen(ip, 5000);
            serverOptions.Listen(ip, 5001, listenOptions =>
            {
                listenOptions.UseHttps();
            });
        });
        ConfigureService(builder.Services, builder.Configuration);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ErrorHandlingMiddleware>();

        // app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }

    private static void ConfigureService(IServiceCollection services, IConfiguration configuration)
    {
        var authenticationSettings = new AuthenticationSettings();
        configuration.GetSection("Authentication").Bind(authenticationSettings);

        services.AddAuthentication(option => {
            option.DefaultAuthenticateScheme = "Bearer";
            option.DefaultScheme = "Bearer";
            option.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(cfg => {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters() {
                ValidIssuer = authenticationSettings.JwtIssuer,
                ValidAudience = authenticationSettings.JwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
            };
        });

        services.AddSingleton(authenticationSettings);
        services.AddDbContext<ChatterContext>(
            opt => opt.UseSqlite("Data Source=chatter.db")
        );
        services.AddScoped<ErrorHandlingMiddleware>();
        services.AddScoped<AccountService>();
        services.AddScoped<ChattingService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    }
}
