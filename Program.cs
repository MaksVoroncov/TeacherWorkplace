var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

// Временная страница — проверяем что .NET стартует
app.MapGet("/", () => Results.Content(@"
<!DOCTYPE html><html><body>
<h1>TeacherWorkplace</h1>
<p>Сервер работает! База данных подключается...</p>
</body></html>", "text/html"));

app.MapGet("/health", () => "OK");

app.Run();
