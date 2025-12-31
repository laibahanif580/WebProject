using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalTaskManager.Data;
using System.Security.Claims;
using PersonalTaskManager.Models.Repositories;
using PersonalTaskManager.Models;
using PersonalTaskManager.Hubs;
using Microsoft.AspNetCore.Authorization; // for IAuthorizationHandler
using PersonalTaskManager.Authorization; // for TaskPermissionHandler and TaskPermissionRequirement


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, PersonalTaskManager.Models.EmailSender>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddSingleton<IAuthorizationHandler, TaskPermissionHandler>();

builder.Services.AddSignalR();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TaskAccess", policy =>
        policy.Requirements.Add(new TaskPermissionRequirement("Viewer", "Editor")));

    options.AddPolicy("AdminOnly", policy =>
        policy.Requirements.Add(new TaskPermissionRequirement("Admin")));

    options.AddPolicy("TaskViewer", policy =>
        policy.Requirements.Add(new TaskPermissionRequirement("Viewer")));

    options.AddPolicy("TaskEditor", policy =>
        policy.Requirements.Add(new TaskPermissionRequirement("Editor")));
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add Identity with roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure cookies for login/logout
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Add MVC and Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Seed roles and default admin user
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Create default admin
    string adminEmail = "admin@taskmanager.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        string adminPassword = "Admin@123"; // set a secure password
                var createAdmin = await userManager.CreateAsync(newAdmin, adminPassword);

                if (createAdmin.Succeeded)
                {
                    adminUser = newAdmin; // assign newly created user to adminUser
                }
                
        }
        
            // Now make sure the Admin has the claim
            var claims = await userManager.GetClaimsAsync(adminUser);
            if (!claims.Any(c => c.Type == "TaskPermission" && c.Value == "Admin"))
            {
                await userManager.AddClaimAsync(adminUser, new Claim("TaskPermission", "Admin"));
            }
    }


// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Must be before Authorization
app.UseAuthorization();


// Map hub
app.MapHub<AnnouncementHub>("/announcementhub");

// Map routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

public partial class Program { } // for integration testing
