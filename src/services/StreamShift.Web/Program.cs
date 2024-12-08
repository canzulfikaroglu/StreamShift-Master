using Microsoft.EntityFrameworkCore;
using StreamShift.Domain.Entities;
using StreamShift.Infrastructure.Context;
using StreamShift.Infrastructure.Services.TransferService.Abstract;
using StreamShift.Infrastructure.Services.TransferService.Concrete;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDb>(options =>
{
    options.UseNpgsql(connectionString);
}, ServiceLifetime.Scoped, ServiceLifetime.Scoped);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequiredLength = 5;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
}).AddRoles<AppRole>().AddEntityFrameworkStores<AppDb>();

builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddScoped<StreamShift.Infrastructure.ContextFactories.Abstract.IDbContextFactory, StreamShift.Infrastructure.ContextFactories.Concrete.DbContextFactory>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
