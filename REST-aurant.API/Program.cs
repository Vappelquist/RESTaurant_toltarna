
using Microsoft.EntityFrameworkCore;
using REST_aurant.API.Data;
using Restaurant.Models;

namespace REST_aurant.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<RestaurantDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
            var app = builder.Build();
            
            //builder.Services.AddDbContext<RestaurantDbContext>(options =>
            //options.UseSqlServer(
            //builder.Configuration.GetConnectionString("Default"),
            //b => b.MigrationsAssembly("REST-aurant") 
            //));

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
