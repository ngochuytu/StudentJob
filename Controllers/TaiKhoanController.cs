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
    [ValidateAntiForgeryToken]
    public IActionResult DangNhap(DangNhapViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var taiKhoan = _taiKhoanRepository.GetByEmail(model.sEmail);

        if (taiKhoan == null || !BCrypt.Net.BCrypt.Verify(model.sMatKhau, taiKhoan.sMatKhau))
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

        return taiKhoan.VaiTro.sTenVaiTro switch
        {
            "Admin" => RedirectToAction("QuanLyTin", "Admin"),
            "Nhà tuyển dụng" => RedirectToAction("QuanLyTin", "NhaTuyenDung"),
            _ => RedirectToAction("Index", "Home"),
        };
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
    [ValidateAntiForgeryToken]
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
            if (model.CvFile != null && model.CvFile.Length > 0)
            {
                var allowedCvExtensions = new[] { ".pdf", ".doc", ".docx" };
                var ext = Path.GetExtension(model.CvFile.FileName).ToLower();
                if (!allowedCvExtensions.Contains(ext))
                {
                    ModelState.AddModelError("CvFile", "Định dạng file CV không hợp lệ. Chỉ chấp nhận các định dạng: .pdf, .doc, .docx");
                }
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
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var allowedImgExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif" };
                var ext = Path.GetExtension(model.ImageFile.FileName).ToLower();
                if (!allowedImgExtensions.Contains(ext))
                {
                    ModelState.AddModelError("ImageFile", "Định dạng ảnh logo không hợp lệ. Chỉ chấp nhận: .png, .jpg, .jpeg, .gif");
                }
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
            sMatKhau = BCrypt.Net.BCrypt.HashPassword(model.sMatKhau),
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

            if (model.CvFile != null && model.CvFile.Length > 0)
            {
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs", "default");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                var ext = Path.GetExtension(model.CvFile.FileName);
                var fileName = $"cv_{sinhVien.PK_IdSinhVien}{ext}";
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.CvFile.CopyTo(stream);
                }

                sinhVien.sDuongDanCVMacDinh = $"/uploads/cvs/default/{fileName}";
                _sinhVienRepository.Update(sinhVien);
            }
        }
        else
        {
            var nhaTuyenDung = new NhaTuyenDung
            {
                FK_IdTaiKhoan = taiKhoan.PK_IdTaiKhoan,
                sTenDoanhNghiep = model.sTenDoanhNghiep!,
                sDiaChiVanPhong = model.sDiaChiVanPhong!,
                sMoTaTongQuan = model.sMoTaTongQuan
            };
            _nhaTuyenDungRepository.Add(nhaTuyenDung);

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "logos");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                var ext = Path.GetExtension(model.ImageFile.FileName);
                var fileName = $"logo_{nhaTuyenDung.PK_IdNhaTuyenDung}{ext}";
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(stream);
                }

                nhaTuyenDung.sDuongDanAnhLogo = $"/uploads/logos/{fileName}";
                _nhaTuyenDungRepository.Update(nhaTuyenDung);
            }
        }

        taiKhoan.VaiTro = vaiTro;
        SignInUser(taiKhoan);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost("dang-xuat")]
    [ValidateAntiForgeryToken]
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
