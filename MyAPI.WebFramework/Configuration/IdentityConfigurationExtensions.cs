using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyAPI.Data;
using MyAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAPI.WebFramework.Configuration
{
    public static class IdentityConfigurationExtensions
    {
        public static void AddCustomIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>(identityOption =>
            {
                //Password Settings
                identityOption.Password.RequireDigit = true;
                identityOption.Password.RequiredLength = 6;
                identityOption.Password.RequireNonAlphanumeric = false;
                identityOption.Password.RequireLowercase = false;
                identityOption.Password.RequireUppercase = false;

                //UserName Settings
                identityOption.User.RequireUniqueEmail = true;

                //Singing Settings
                //identityOption.SignIn.RequireConfirmedEmail = false;
                //identityOption.SignIn.RequireConfirmedPhoneNumber = false;

                //Lockout Settings
                //identityOption.Lockout.MaxFailedAccessAttempts = 5;
                //identityOption.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(5);
                //identityOption.Lockout.AllowedForNewUsers = false;

            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}
