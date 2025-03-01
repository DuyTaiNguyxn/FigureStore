using FigureStore.Data;
using FigureStore.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Thêm DbContext vào container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddSingleton<CloudinaryService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    string cloudName = configuration["CloudinarySettings:CloudName"];
    string apiKey = configuration["CloudinarySettings:ApiKey"];
    string apiSecret = configuration["CloudinarySettings:ApiSecret"];
    return new CloudinaryService(cloudName, apiKey, apiSecret);
});

// (Tùy chọn) Nếu frontend nằm ngoài domain, hãy kích hoạt CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Đổi cổng nếu React chạy khác
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Áp dụng CORS
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Nếu dùng MVC views:
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Nếu chỉ cung cấp API:
app.MapControllers();

app.Run();
