using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SignalRChat.Hubs;
using CollabApp.mvc.Controllers;
using CollabApp.mvc.Models;
using CollabApp.mvc.Services;
using CollabApp.mvc.Context;
using CollabApp.mvc.Utilities;


namespace CollabApp.mvc;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddRazorPages();

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddSignalR();

        builder.Services.AddDbContext<ApplicationDbContext>(options => 
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        // Sets the JsonRepository to a PostController (IoC)
        builder.Services.AddSingleton<IDBAccess<Post>>(new JsonRepository<Post>("Data/postDB.json"));

        builder.Services.AddScoped<PostFilterService>();

        // Sets the JsonRepository to a MessageController (IoC)
        builder.Services.AddSingleton<IDBAccess<Message>>(new JsonRepository<Message>("Data/chatDB.json"));

        //set properties for GCSConfigOptions from appsettings.json
        builder.Services.Configure<GCSConfigOptions>(builder.Configuration);
        builder.Services.AddSingleton<ICloudStorageService, CloudStorageService>();

        var app = builder.Build();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSession();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Login}/{action=Login}/{id?}");
        app.MapRazorPages();
        app.MapHub<ChatHub>("/chatHub");

        //this code is responsible for seeding sample data into the db
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                // Get the ApplicationDbContext instance
                var dbContext = services.GetRequiredService<ApplicationDbContext>();

                // Create an instance of the DatabaseSeeder
                var databaseSeeder = new DatabaseSeeder(dbContext);

                // Call the SeedSampleData method to seed the data
                //!! IT SEEDS THE SAMPLE DATA EVERY TIME WHEN APP IS LAUNCHED
                //FIXME
                //databaseSeeder.SeedSampleData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while seeding the database: " + ex.Message);
            }
        }

        app.Run();
    }
}