using Microsoft.EntityFrameworkCore;
using UtulkyApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Přidání MVC s runtime kompilací
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Konfigurace Entity Frameworku s SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=utulky.db"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();