
using Microsoft.EntityFrameworkCore;
using SignalRChat.Hubs;
using CollabApp.mvc.Services;
using CollabApp.mvc.Hubs;
using CollabApp.mvc.Context;
using CollabApp.mvc.Utilities;
using CollabApp.mvc.Repo;

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
        }, ServiceLifetime.Scoped);
        builder.Services.AddScoped<IPostRepository, PostRepository>();
        builder.Services.AddScoped<IBoardRepository, BoardRepository>();
        builder.Services.AddScoped<ICommentRepository, CommentRepository>();
        builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        builder.Services.AddScoped<PostFilterService>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSingleton<NotificationService>();
        //set properties for GCSConfigOptions from appsettings.json
        builder.Services.Configure<GCSConfigOptions>(builder.Configuration);
        builder.Services.AddSingleton<ICloudStorageService, CloudStorageService>();

        builder.Services.AddHttpClient("Api", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
            // Add any additional configuration if needed
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });

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
        //mb we don't need this
        if (app.Environment.IsDevelopment())
        {
            // Trust the SSL certificate for development
            app.UseDeveloperExceptionPage();
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }

        app.UseSession();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Login}/{action=Login}/{id?}");
        app.MapRazorPages();
        app.MapHub<ChatHub>("/chatHub");
        app.MapHub<NotificationHub>("/notificationHub");

        //this code is responsible for seeding sample data into the db
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
/*            try
            {
                // Get the ApplicationDbContext instance
                var dbContext = services.GetRequiredService<ApplicationDbContext>();

                // Create an instance of the DatabaseSeeder
                var databaseSeeder = new DatabaseSeeder(dbContext);

                // Call the SeedSampleData method to seed the data

                // databaseSeeder.SeedSampleData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while seeding the database: " + ex.Message);
            }*/
        }

        app.Run();
    }
}