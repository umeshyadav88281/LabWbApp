using LabWbApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// Configure GitHub Authentication
builder.Services.AddAuthentication().AddGitHub(options =>
{
    // Directly setting the ClientId and ClientSecret
    options.ClientId = "Ov23liGqLx7MTHYjcvzM";
    options.ClientSecret = "c349f07b7aff0a6c00233897e537c219754ab6a9";
    options.CallbackPath = "/signin-github"; // Ensure this matches the callback URL provided in GitHub OAuth settings
    options.Scope.Add("read:user");
    options.Events.OnCreatingTicket = context =>
    {
        if (context.AccessToken is { })
        {
            context.Identity?.AddClaim(new Claim("access_token", context.AccessToken));
        }
        return Task.CompletedTask;
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
