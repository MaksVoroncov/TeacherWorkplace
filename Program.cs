using Microsoft.EntityFrameworkCore;
using TeacherWorkplace.Data;

// Фикс DateTime для PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Строка подключения: Railway даёт DATABASE_URL, локально — appsettings
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")!;

// Преобразование postgres://... в формат Npgsql
if (connectionString.StartsWith("postgres://") || connectionString.StartsWith("postgresql://"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};" +
                       $"Database={uri.AbsolutePath.TrimStart('/')};" +
                       $"Username={userInfo[0]};Password={userInfo[1]};" +
                       $"SSL Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (connectionString.Contains("localdb") || connectionString.Contains("Server="))
        options.UseSqlServer(connectionString);
    else
        options.UseNpgsql(connectionString);
});

builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Миграции + Seed при старте
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        db.Database.Migrate();
        await DbSeeder.SeedAsync(db);
        Console.WriteLine("✅ БД инициализирована успешно");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Ошибка БД: {ex.Message}");
    }
}

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapGet("/health", () => "OK");

app.Run();