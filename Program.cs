using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using StudentJob.Data;
using StudentJob.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ===== DbContext Registration (Scoped) =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ===== Repository Registration (Scoped) =====
builder.Services.AddScoped<IVaiTroRepository, VaiTroRepository>();
builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
builder.Services.AddScoped<ISinhVienRepository, SinhVienRepository>();
builder.Services.AddScoped<INhaTuyenDungRepository, NhaTuyenDungRepository>();
builder.Services.AddScoped<INganhNgheRepository, NganhNgheRepository>();
builder.Services.AddScoped<IKhuVucRepository, KhuVucRepository>();
builder.Services.AddScoped<IBaiTuyenDungRepository, BaiTuyenDungRepository>();
builder.Services.AddScoped<IHoSoUngTuyenRepository, HoSoUngTuyenRepository>();
builder.Services.AddScoped<ILuuTinRepository, LuuTinRepository>();

// ===== Cookie Authentication =====
builder
    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/tai-khoan/dang-nhap";
        options.LogoutPath = "/tai-khoan/dang-xuat";
        options.AccessDeniedPath = "/loi/403";
        options.Cookie.Name = "StudentJob.Auth";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// ===== MVC =====
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.Run();
