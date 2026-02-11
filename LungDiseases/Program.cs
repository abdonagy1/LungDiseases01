
using LungDisease.Domain.IdentityModule;
using LungDisease.Service.Services;
using LungDisease.Service_Abstraction;
using LungDiseases.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


            builder.Services.AddDbContext<LungIdentityDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<LungIdentityDbContext>(); ;
            #endregion

            var app = builder.Build();

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

            app.UseAuthorization();


            app.MapControllers();

          await app.RunAsync(); 
            #endregion
        }
    }
}
