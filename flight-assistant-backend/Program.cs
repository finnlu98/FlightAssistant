using flight_assistant_backend.Api.Data;
using flight_assistant_backend.Api.Service;
using flight_assistant_backend.Hubs;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using flight_assistant_backend.Data;
using flight_assistant_backend.Api.Settings;


namespace flight_assistant_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseUrls("http://0.0.0.0:5208");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalNetwork", policy =>
                {
                    policy.SetIsOriginAllowed(origin =>
                    {
                        return origin == "http://localhost:3000" || 
                            origin.StartsWith("http://192.168.") ||
                            origin.StartsWith("http://flightassistant");
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            builder.Services.AddControllers(); 

            builder.Services.AddHttpClient<FlightFinderService>();
            builder.Services.AddScoped<FlightFinderService>();

            builder.Services.AddHttpClient<HomeAssistantService>();
            builder.Services.AddScoped<HomeAssistantService>();

            builder.Services.AddHostedService<FlightDataScheduler>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSignalR();

            builder.Services.AddTransient<DatabaseInitializer>();

            builder.Services.Configure<QuerySettings>(builder.Configuration.GetSection("QuerySettings"));
            builder.Services.Configure<HomeAssistantSettings>(builder.Configuration.GetSection("HomeAssistantSettings"));
            
            var app = builder.Build();

            app.MapMethods("/", ["GET", "HEAD"], () => "Flight Assistant is recieving requests");

            app.UseCors("AllowLocalNetwork");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();

            app.MapHub<MapHub>("/mapHub");

            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
                initializer.Initialize();
            }

            app.Run();
        }
    }
}
