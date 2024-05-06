﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using WorldCitiesAPI.Controllers;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;
using Xunit;

namespace WorldCitiesAPI.Tests
{
    public class SeedController_Test
    {
        [Fact]
        public async Task CreateDefaultUsers()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "WorldCities")
                .Options;
            // create a IWebHost environment mock instance
            var mockEnv = Mock.Of<IWebHostEnvironment>();
            // create a IConfiguration mock instance
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "DefaultPasswords:RegisteredUser")]).Returns("M0ckP$$word");
            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "DefaultPasswords:Administrator")]).Returns("M0ckP$$word");

            // create a ApplicationDbContext instance using the
            // in-memory DB
            using var context = new ApplicationDbContext(options);
            // create a RoleManager instance
            var roleManager = IdentityHelper.GetRoleManager(
            new RoleStore<IdentityRole>(context));
            // create a UserManager instance
            var userManager = IdentityHelper.GetUserManager(
            new UserStore<ApplicationUser>(context));
            // create a SeedController instance
            var controller = new SeedController(
            context,
            roleManager,
            userManager,
            mockEnv,
            mockConfiguration.Object
            );
            // define the variables for the users we want to test
            ApplicationUser user_Admin = null!;
            ApplicationUser user_User = null!;
            ApplicationUser user_NotExisting = null!;
            // Act
            // execute the SeedController's CreateDefaultUsers()
            // method to create the default users (and roles)
            await controller.CreateDefaultUsers();
            // retrieve the users
            user_Admin = await userManager.FindByEmailAsync(
            "admin@email.com");
            user_User = await userManager.FindByEmailAsync(
            "user@email.com");
            user_NotExisting = await userManager.FindByEmailAsync(
            "notexisting@email.com");

            // Assert
            Assert.NotNull(user_Admin);
            Assert.NotNull(user_User);
            Assert.Null(user_NotExisting);

        }
    }
}
