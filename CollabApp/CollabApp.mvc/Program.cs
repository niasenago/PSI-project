using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SignalRChat.Hubs;
using CollabApp.mvc.Controllers;
using CollabApp.mvc.Models;
using CollabApp.mvc.Services;
using CollabApp.mvc.Hubs;

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

        // Sets the JsonRepository to a PostController (IoC)
        builder.Services.AddSingleton<IDBAccess<Post>>(new JsonRepository<Post>("Data/postDB.json"));

        builder.Services.AddScoped<PostFilterService>();

        // Sets the JsonRepository to a MessageController (IoC)
        builder.Services.AddSingleton<IDBAccess<Message>>(new JsonRepository<Message>("Data/chatDB.json"));

        builder.Services.AddSingleton<NotificationService>();

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
        app.MapHub<NotificationHub>("/notificationHub");

        app.Run();
    }
}