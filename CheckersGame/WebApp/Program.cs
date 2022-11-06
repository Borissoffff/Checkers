
using DAL.DB;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


// ============================= PIPELINE SETUP ====================================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

/*dotnet aspnet-codegenerator razorpage -m CheckersGame -dc AppDbContext -udl -outDir Pages/CheckersGames --referenceScriptLibraries -f
 dotnet aspnet-codegenerator razorpage -m CheckersOption -dc AppDbContext -udl -outDir Pages/CheckersOptions --referenceScriptLibraries -f
 dotnet aspnet-codegenerator razorpage -m CheckersGameState -dc AppDbContext -udl -outDir Pages/CheckersGameStates --referenceScriptLibraries -f
 
 
 
 
 */
