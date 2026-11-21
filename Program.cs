using LedgerPro.Data;
using LedgerPro.Repositories;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------
// 1. Database connections
//    IdentityConnection -> ASP.NET Core Identity tables (Entity Framework)
//    DefaultConnection  -> business tables (Dapper repositories)
// --------------------------------------
var identityConnection =
    builder.Configuration.GetConnectionString("IdentityConnection")
    ?? throw new InvalidOperationException("Connection string 'IdentityConnection' was not found.");

var businessConnection =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(identityConnection));

// --------------------------------------
// 2. ASP.NET Core Identity (login / signup are Identity Razor Pages in Areas/Identity)
// --------------------------------------
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// --------------------------------------
// 3. MVC + Razor Pages
// --------------------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// --------------------------------------
// 4. Dependency injection: Dapper connection factory + repositories (interface -> implementation)
// --------------------------------------
builder.Services.AddScoped<DbConnectionFactory>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ILedgerRepository, LedgerRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

var app = builder.Build();

// --------------------------------------
// 5. Error handling
// --------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// --------------------------------------
// 6. Middleware
// --------------------------------------
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// --------------------------------------
// 7. Routes: "/" -> HomeController, which sends people to Login or their dashboard
// --------------------------------------
app.MapGet("/", context =>
{
    context.Response.Redirect("/Identity/Account/Login");
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");

app.MapRazorPages();

// --------------------------------------
// 8. First-run setup: create/upgrade databases and seed the Admin and Staff roles
// --------------------------------------
await using (var scope = app.Services.CreateAsyncScope())
{
    // Identity tables (applies the EF migrations in Data/Migrations)
    var identityDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await identityDb.Database.MigrateAsync();

    // Business tables for the Dapper repositories (only runs when the Customers table is missing)
    await DatabaseInitializer.EnsureBusinessSchemaAsync(
        businessConnection,
        Path.Combine(app.Environment.ContentRootPath, "Database", "ledgerpro-schema.sql"));

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var role in new[] { "Admin", "Staff" })
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(role));

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create role '{role}': " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}

app.Run();
