using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ThreadsASP.Models;

namespace ThreadsASP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<IdentityContext>(opts =>
                opts.UseSqlServer(builder.Configuration["ConnectionStrings:IdentityConnection"]));
            
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>();

            var app = builder.Build();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}