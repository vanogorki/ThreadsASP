using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ThreadsASP.Models;
using SportsStore.Models;
using ThreadsASP.FileUploadService;

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

            builder.Services.AddScoped<IPostsRepository, EFPostsRepository>();

            builder.Services.AddScoped<IFileUploadService, LocalFileUploadService>();

            builder.Services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlServer(builder.Configuration["ConnectionStrings:AppConnection"]));
            
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.Configure<IdentityOptions>(opts => {
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
                opts.User.RequireUniqueEmail = true;
            });

            var app = builder.Build();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}