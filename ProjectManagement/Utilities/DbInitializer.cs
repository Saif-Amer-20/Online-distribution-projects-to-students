using Microsoft.AspNetCore.Identity;
using ProjectManagement.Data;
using ProjectManagement.DataContextModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Utilities
{
    public class DbInitializer
    {
        private  UserManager<ApplicationUser> _userManager;
        private  RoleManager<IdentityRole> _roleManager;


        // Ensure database creation
        public  DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            SeedRoles().Wait();
            SeedUsers().Wait();
        }

        // Ensure roles creation
        public async Task SeedRoles()
        {
            if (!_roleManager.Roles.Any())
            {
                foreach (var role in Enum.GetNames(typeof(Roles)))
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            }
        }

        // Ensure admin user 
        public async Task SeedUsers()
        {
            if (!_userManager.Users.Any())
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "SuperAdmin@outlook.com",
                    Email = "SuperAdmin@outlook.com",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin@2010");

                if (result != IdentityResult.Success)
                {
                    throw new Exception($"Unable to create '{adminUser.UserName}' account: {result}");
                }


                var adminRoles = await _userManager.GetRolesAsync(adminUser);
                foreach (var role in Enum.GetNames(typeof(Roles)))
                {
                    if (!adminRoles.Any(r => string.Equals(r, role)))
                        await _userManager.AddToRoleAsync(adminUser, role);
                }
           }

        }
    }

    public enum Roles
    {
       SystemAdmin,
       Professor,
       Student
    }
}
