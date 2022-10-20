using BackEnd.Controllers;
using BackEnd.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BackEnd.Test
{
    public class TestSessionsController
    {
        private readonly DbContextOptions<ApplicationDbContext>? options;
        private readonly SessionsController? sessionsController;
        private readonly ApplicationDbContext? context;
        private readonly string? connectionString;
        
        public TestSessionsController()
        {
            connectionString = "Server=(localdb)\\mssqllocaldb;Database=aspnet-BackEnd-931E56BD-86CB-4A96-BD99-2C6A6ABB0829;Trusted_Connection=True;MultipleActiveResultSets=true";

            options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseSqlServer(connectionString)
                            .Options;
            
            context = new ApplicationDbContext(options);


            sessionsController = new SessionsController(context);

        }

        [Fact]
        public async void Test_GetSessions()
        {
            var actionResult = await sessionsController.Get();

            var sessions = actionResult.Value;

            Xunit.Assert.NotEmpty(sessions);
        }
    }
}
