using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShopDemo.Data;
using WebShopDemo.Domain;

namespace WebShopDemo.Infrastructure
{
    public static class ApplicationBuilderExtension
    {
        public static async Task<IApplicationBuilder> PrepareDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var services = serviceScope.ServiceProvider;
            await RoleSeeder(services);
            await SeedAdministrator(services);

            var dataCategory = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedCategories(dataCategory);

            var dataBrand = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedBrands(dataBrand);


            return app;

        }

        private static void SeedBrands(ApplicationDbContext dataBrand)
        {
            if (dataBrand.Categories.Any())
            {
                return;
            }
            dataBrand.Brands.AddRange(new[]
            {
               new Brand{BrandName="aser"},
               new Brand{BrandName="asus"},
               new Brand{BrandName="apple"},
               new Brand{BrandName="aell"},
               new Brand{BrandName="hp"},
               new Brand{BrandName="samsung"},
               new Brand{BrandName="lenovo"}
           });
            dataBrand.SaveChanges();
        }

        private static void SeedCategories(ApplicationDbContext dataCategory)
        {
           if (dataCategory.Categories.Any())
            {
                return;
            }
            dataCategory.Categories.AddRange(new[]
            {
               new Category{CategoryName="laptop"},
               new Category{CategoryName="computer"},
               new Category{CategoryName="monitor"},
               new Category{CategoryName="acessory"},
               new Category{CategoryName="tv"},
               new Category{CategoryName="gsm"},
               new Category{CategoryName="watch"}
           });
            dataCategory.SaveChanges();
        }

      

        private static async Task RoleSeeder(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Administrator", "Client" };
            IdentityResult roleResult;
            foreach (var role in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);

                if(!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdministrator(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (await userManager.FindByNameAsync("admin")==null)
            {
                ApplicationUser user = new ApplicationUser();
                user.FirstName = "admin";
                user.LastName = "admin";
                user.PhoneNumber = "0888888888";
                user.UserName = "admin";
                user.Email = "admin@admin.com";

                var result = await userManager.CreateAsync
                    (user, "Admin123456");

                if(result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
        }

    }
}
