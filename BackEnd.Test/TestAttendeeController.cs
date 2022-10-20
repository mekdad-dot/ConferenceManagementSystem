using BackEnd.Controllers;
using BackEnd.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BackEnd.Test
{
    public class TestAttendeeController
    {
        private readonly Mock<IWebHostEnvironment>? mockEnvironment;
        private readonly DbContextOptions<ApplicationDbContext>? options;
        private readonly AttendeesController? attendeesController;
        private readonly ApplicationDbContext? context;
        private readonly string? connectionString;


        public TestAttendeeController()
        {

            connectionString = "Server=(localdb)\\mssqllocaldb;Database=aspnet-BackEnd-931E56BD-86CB-4A96-BD99-2C6A6ABB0829;Trusted_Connection=True;MultipleActiveResultSets=true";

            options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            mockEnvironment = new Mock<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();

            mockEnvironment.Setup(m => m.EnvironmentName)
                   .Returns("Hosting:UnitTestEnvironment");

            context = new ApplicationDbContext(options);

            attendeesController = new AttendeesController(context, mockEnvironment.Object);
        }

        [Theory]
        [InlineData("mekdad442@gmail.com")]
        public async void getAttendeeAsyn_Test(string? userName)
        {
            var actionResult = await attendeesController.Get(userName);

            var actualAttendee = actionResult.Value;

            Xunit.Assert.Equal("mekdad442@gmail.com", actualAttendee?.UserName);
        }
    }
}
