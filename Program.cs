using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ThreadsASP.Models;
using ThreadsASP.FileUploadService;
using ThreadsASP.Models.Repositories;
using ThreadsASP.Services;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;

namespace ThreadsASP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();

            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IPostsRepository, PostsRepository>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<IFollowsRepository, FollowsRepository>();

            builder.Services.AddScoped<IFileService, LocalFileService>();

            builder.Services.AddScoped<EmailService>();

            builder.Services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlServer(builder.Configuration["ConnectionStrings:AppConnection"]));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(opts =>
            {
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
                opts.User.RequireUniqueEmail = true;
                opts.SignIn.RequireConfirmedEmail = true;
            });

            var app = builder.Build();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapDefaultControllerRoute();

            var context = app.Services.CreateScope().ServiceProvider
                .GetRequiredService<AppDbContext>();

            context.Database.Migrate();

            app.Run();
        }
    }
}