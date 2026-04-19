using Microsoft.EntityFrameworkCore;
using TeacherWorkplace.Data;

var builder = WebApplication.CreateBuilder(args);

// Railway PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllersWithViews();

// Database
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    var uri      = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var pgConn   = $"Host={uri.Host};Port={uri.Port};" +
                   $"Database={uri.AbsolutePath.TrimStart('/')};" +
                   $"Username={userInfo[0]};Password={userInfo[1]};" +
                   $"SSL Mode=Require;Trust Server Certificate=true;";
    builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(pgConn));
}
else
{
    var conn = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(conn));
}

builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapGet("/health", () => "OK");

app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Create DB and seed
using (var scope = app.Services.CreateScope())
{
    var ctx    = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        ctx.Database.EnsureCreated();
        await DbSeeder.SeedAsync(ctx);
        logger.LogInformation("Database ready.");
    }
    catch (Exception ex)
    {
        logger.LogWarning("DB not ready at startup: {Msg}", ex.Message);
    }
}

app.Run();
