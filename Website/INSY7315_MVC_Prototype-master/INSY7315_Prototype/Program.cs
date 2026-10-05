using INSY7315_Prototype.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace INSY7315_Prototype
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            });

            builder.Services.AddHttpClient<AuthApiService>(client =>
            {
                client.BaseAddress = new Uri("https://apiinsy7315-latest.onrender.com/");
            });

            // Named client used by LoanController
            builder.Services.AddHttpClient("LoanApi", client =>
            {
                client.BaseAddress = new Uri("https://apiinsy7315-latest.onrender.com/");
            });

            // Session support (fixes the InvalidOperationException)
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();          // after UseRouting, before auth
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.UseStaticFiles();
            app.Run();
        }
    }
}