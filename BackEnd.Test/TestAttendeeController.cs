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
        [Theory]
        [InlineData("username")]
        public async void getAttendeeAsyn_Test(string? userName)
        {
            string connectionString = string.Empty;
            
            
            var option = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            using(var context = new ApplicationDbContext(option))
            {
                var mockEnvironment = new Mock<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
                
                mockEnvironment.Setup(m => m.EnvironmentName)
                    .Returns("Hosting:UnitTestEnvironment");
                
                var controllerInVariable = new AttendeesController(context, mockEnvironment.Object);

                var actionResult = await controllerInVariable.Get(userName);

                var actualAttendee = actionResult.Value;

                Xunit.Assert.Equal("mekdad442@gmail.com", actualAttendee?.UserName);
            }
        }
    }
}
