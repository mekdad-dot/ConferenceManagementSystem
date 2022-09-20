using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrontEnd.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FrontEnd.Data;
using FrontEnd.Areas.Identity;
using FrontEnd.Middleware;
using FrontEnd.HealthChecks;
using Microsoft.AspNetCore.Authentication.Google;
using Serilog;
using Microsoft.Extensions.Logging.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json").Build();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("IdentityDbContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityDbContextConnection' not found.");

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(connectionString));


// Add ClientId , ClientSecret to enable googleUthentication 
/*builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{    
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});*/

builder.Services.AddDefaultIdentity<User>()
        .AddEntityFrameworkStores<IdentityDbContext>()
        .AddClaimsPrincipalFactory<ClaimsPrincipalFactory>();

builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrl"]);
});

// Add services to the container.
builder.Services.AddRazorPages(options =>
        {
            options.Conventions.AuthorizeFolder("/Admin", "Admin");
        });

builder.Services.AddSingleton<IAdminService, AdminService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireAuthenticatedUser()
              .RequireIsAdminClaim();
    });
});

builder.Services.AddHealthChecks()
                .AddCheck<BackendHealthCheck>("backend")
                .AddDbContextCheck<IdentityDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    //app.UseDatabaseErrorPage();
}
else
{
    app.UseExceptionHandler("/Error");
    
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSerilogRequestLogging();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.UseMiddleware<RequireLoginMiddleware>();

app.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorPages();
        endpoints.MapHealthChecks("/health");
    });


app.Run();