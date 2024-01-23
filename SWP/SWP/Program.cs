using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SWP.Dao;
using SWP.Models;

namespace SWP
{
	public class Program
	{
		public static void Main(string[] args)
		{
            var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<UsersDao>();
            builder.Services.AddHttpContextAccessor();
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var service = scope.ServiceProvider;
                var context = new SWPContext();
                UsersDao usersDao = new UsersDao();
                if (context.Users.FirstOrDefault(x=>x.Email.Equals("phucnvhe1@gmail.com"))==null)
                {
                    Dto.RegisterModel registerModel = new Dto.RegisterModel
                    {
                        Email = "phucnvhe1@gmail.com",
                        Password = "Admin123@",
                        FirstName = "Admin",
                        LastName = "Shop",
                        PhoneNumber = "0912345678",
                        RoleId = 1,
                    };

                    usersDao.Register(registerModel);
                }
            }
            
			

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

            app.UseSession();   
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}