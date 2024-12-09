using flight_assistant_backend.Api.Data;
using flight_assistant_backend.Hubs;
using Microsoft.EntityFrameworkCore;


namespace flight_assistant_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
{
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:3000") // Specify the exact origin
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); // Allow credentials like cookies and authentication headers
                });
            });

            builder.Services.AddControllers(); 
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSignalR();
            
            var app = builder.Build();

            app.UseCors("AllowReactApp");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            app.MapHub<MapHub>("/mapHub");

            app.Run();
        }
    }
}
