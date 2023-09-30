using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CollabApp.mvc.Data;
using SignalRChat.Hubs;
using CollabApp.mvc.Controllers;
using CollabApp.mvc.Models;


namespace CollabApp.mvc;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        /*------------------------------------------------------------------------------------*/
        /* In theory, we don't need this part; however, the project doesn't build without it. */
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddControllersWithViews();
        /*------------------------------------------------------------------------------------*/

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddSignalR();

        // Sets the JsonDbController to a PostController (IoC)
        builder.Services.AddSingleton<IDBAccess<Post>>(new JsonDbController<Post>("appDB.json"));

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

        app.Run();
    }
}