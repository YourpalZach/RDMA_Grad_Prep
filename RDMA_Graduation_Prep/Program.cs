using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RDMA_Graduation_Prep.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure EF Core with Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add ASP.NET Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Add MVC & Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Required for Identity

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Identity authentication middleware
app.UseAuthorization();
app.UseSession();

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Student}/{action=Index}/{id?}");

app.MapRazorPages(); // Enables Identity UI routes like /Identity/Account/Login

app.Run();
