namespace StudentJob.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StudentJob.Models;
using StudentJob.Repositories;

[Route("tai-khoan")]
public class TaiKhoanController : Controller
{
    private readonly ITaiKhoanRepository _taiKhoanRepository;
    private readonly ISinhVienRepository _sinhVienRepository;
    private readonly INhaTuyenDungRepository _nhaTuyenDungRepository;
    private readonly IVaiTroRepository _vaiTroRepository;
    private readonly INganhNgheRepository _nganhNgheRepository;

    public TaiKhoanController(
        ITaiKhoanRepository taiKhoanRepository,
        ISinhVienRepository sinhVienRepository,
        INhaTuyenDungRepository nhaTuyenDungRepository,
        IVaiTroRepository vaiTroRepository,
        INganhNgheRepository nganhNgheRepository)
    {
        _taiKhoanRepository = taiKhoanRepository;
        _sinhVienRepository = sinhVienRepository;
        _nhaTuyenDungRepository = nhaTuyenDungRepository;
        _vaiTroRepository = vaiTroRepository;
        _nganhNgheRepository = nganhNgheRepository;
    }

    [HttpGet("dang-nhap")]
    public IActionResult DangNhap(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost("dang-nhap")]
    public IActionResult DangNhap(DangNhapViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var taiKhoan = _taiKhoanRepository.GetByEmail(model.sEmail);

        if (taiKhoan == null || taiKhoan.sMatKhau != model.sMatKhau)
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        if (!taiKhoan.bTrangThaiHoatDong)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
            return View(model);
        }

        SignInUser(taiKhoan);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("dang-ky")]
    public IActionResult DangKy()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }
        ViewBag.DsNganhNghe = _nganhNgheRepository.GetAll();
        return View(new DangKyViewModel());
    }

    [HttpPost("dang-ky")]
    public IActionResult DangKy(DangKyViewModel model)
    {
        if (model.LoaiTaiKhoan == "SinhVien")
        {
            if (string.IsNullOrWhiteSpace(model.sHoTen))
            {
                ModelState.AddModelError("sHoTen", "Họ và tên không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(model.sChuyenNganhHoc))
            {
                ModelState.AddModelError("sChuyenNganhHoc", "Vui lòng chọn chuyên ngành học.");
            }
        }
        else if (model.LoaiTaiKhoan == "NhaTuyenDung")
        {
            if (string.IsNullOrWhiteSpace(model.sTenDoanhNghiep))
            {
                ModelState.AddModelError("sTenDoanhNghiep", "Tên doanh nghiệp không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(model.sDiaChiVanPhong))
            {
                ModelState.AddModelError("sDiaChiVanPhong", "Địa chỉ văn phòng không được để trống.");
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.DsNganhNghe = _nganhNgheRepository.GetAll();
            return View(model);
        }

        var existingAccount = _taiKhoanRepository.GetByEmail(model.sEmail);
        if (existingAccount != null)
        {
            ModelState.AddModelError("sEmail", "Email này đã được sử dụng.");
            ViewBag.DsNganhNghe = _nganhNgheRepository.GetAll();
            return View(model);
        }

        string tenVaiTro = model.LoaiTaiKhoan == "SinhVien" ? "Sinh viên" : "Nhà tuyển dụng";
        var vaiTro = _vaiTroRepository.GetAll().FirstOrDefault(v => v.sTenVaiTro == tenVaiTro);
        if (vaiTro == null)
        {
            ModelState.AddModelError(string.Empty, "Lỗi hệ thống: không tìm thấy vai trò. Vui lòng liên hệ quản trị viên.");
            ViewBag.DsNganhNghe = _nganhNgheRepository.GetAll();
            return View(model);
        }

        var taiKhoan = new TaiKhoan
        {
            sEmail = model.sEmail,
            sMatKhau = model.sMatKhau,
            sSoDienThoai = model.sSoDienThoai,
            FK_IdVaiTro = vaiTro.PK_IdVaiTro,
            bTrangThaiHoatDong = true,
            dNgayTaoTaiKhoan = DateTime.Now
        };
        _taiKhoanRepository.Add(taiKhoan);

        if (model.LoaiTaiKhoan == "SinhVien")
        {
            var sinhVien = new SinhVien
            {
                FK_IdTaiKhoan = taiKhoan.PK_IdTaiKhoan,
                sHoTen = model.sHoTen!,
                sChuyenNganhHoc = model.sChuyenNganhHoc!
            };
            _sinhVienRepository.Add(sinhVien);
        }
        else
        {
            var nhaTuyenDung = new NhaTuyenDung
            {
                FK_IdTaiKhoan = taiKhoan.PK_IdTaiKhoan,
                sTenDoanhNghiep = model.sTenDoanhNghiep!,
                sDiaChiVanPhong = model.sDiaChiVanPhong!
            };
            _nhaTuyenDungRepository.Add(nhaTuyenDung);
        }

        taiKhoan.VaiTro = vaiTro;
        SignInUser(taiKhoan);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost("dang-xuat")]
    public IActionResult DangXuat()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
        return RedirectToAction("Index", "Home");
    }

    private void SignInUser(TaiKhoan taiKhoan)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, taiKhoan.PK_IdTaiKhoan.ToString()),
            new Claim(ClaimTypes.Email, taiKhoan.sEmail),
            new Claim(ClaimTypes.Role, taiKhoan.VaiTro.sTenVaiTro)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            }).Wait();
    }
}
