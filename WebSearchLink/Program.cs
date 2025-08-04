using Microsoft.EntityFrameworkCore;
using WebSearchLink.Models;
using WebSearchLink.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(40); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("client", c =>
{
    c.BaseAddress = new Uri("http://amsernest-001-site1.ptempurl.com/");
});
builder.Services.AddDbContext<DbAba3d6Amsernest1234567Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InformationMeetingContext")));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ZoomService>();
builder.Services.AddHostedService<ScheduledUploadService>();
builder.WebHost.UseWebRoot("wwwroot");
// client for YouTube API and Zoom API
builder.Services.AddHttpClient("Zoom", c =>
{
    c.BaseAddress = new Uri("https://zoom.us/");
});
builder.Services.AddHttpClient("ZoomDownload", client =>
{
    client.Timeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddHttpClient("YouTube", c =>
{
    c.BaseAddress = new Uri("https://www.googleapis.com/");
});

builder.Services.AddScoped<IZoomService, ZoomService>();
builder.Services.AddScoped<IYouTubeService, YouTubeUploadService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WhenToMeet}/{action=LoginWhenToMeet}/{id?}");

app.Run();
