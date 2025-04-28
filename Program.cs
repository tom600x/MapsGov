var builder = WebApplication.CreateBuilder(args);

// Add secrets.json support
builder.Configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

// Explicitly add Environment Variables to ensure they're loaded
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Map}/{action=Index}/{id?}");

app.Run();
