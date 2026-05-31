
using System.Text;
using LungDisease.Domain.IdentityModule;
using LungDisease.Service.Services;
using LungDisease.Service_Abstraction;
using LungDiseases.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.IdentityData.DbContext;

namespace LungDiseases
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(Option => Option.AddPolicy("AllowFrontend",
                policy =>
            {
                policy.WithOrigins(
                    "http://127.0.0.1:5501",
                    "http://localhost:5501")
                .AllowAnyHeader()
                .AllowAnyMethod();
            }));
          


            builder.Services.AddDbContext<LungIdentityDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<LungIdentityDbContext>();
            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {


                option.SaveToken = true;
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["JWTOptions:Issuer"],
                    ValidAudience = builder.Configuration["JWTOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:Secretkey"]))
                };
            });
            builder.Services.AddHttpClient<IAIClient, AIClient>();

            builder.Services.AddScoped<IAttachmentService, AttachmentService>();



            #endregion



            var app = builder.Build();
            app.UseCors("AllowFrontend");

            #region Data
            await app.MigrateIdentityDatabaseAsync();

            #endregion

            // Configure the HTTP request pipeline.
            #region Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

          await app.RunAsync(); 
            #endregion
        }
    }
}
